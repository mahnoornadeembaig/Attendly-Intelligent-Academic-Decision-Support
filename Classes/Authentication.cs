using System;
using System.Data;

namespace AMS.Classes
{
    static internal class Authentication
    {
        public static AuthenticatedUser? Login(string email, string password, string user)
        {
            (string Table, string IdColumn, string CourseExpression, string CourseNameExpression)? accountTable = user switch
            {
                "Admin" => ("Admin", "Aid", "0", "''"),
                "Teacher" => ("Teacher", "Tid", "COALESCE(CourseId, 0)", "COALESCE(course, '')"),
                "Student" => ("Student", "Sid", "0", "''"),
                _ => null
            };
            if (accountTable == null)
            {
                return null;
            }

            DbConnection db = new DbConnection();
            string query = $"SELECT {accountTable.Value.IdColumn} AS UserId, name AS UserName, password AS PasswordHash, {accountTable.Value.CourseExpression} AS CourseId, {accountTable.Value.CourseNameExpression} AS CourseName FROM {accountTable.Value.Table} WHERE email = @email COLLATE NOCASE LIMIT 1";
            DataTable accounts = db.ExecuteQuery(query, new Dictionary<string, object>
            {
                ["@email"] = email.Trim()
            });
            if (accounts.Rows.Count == 0)
            {
                return null;
            }

            DataRow account = accounts.Rows[0];
            string storedPassword = Convert.ToString(account["PasswordHash"]) ?? string.Empty;
            if (!PasswordSecurity.Verify(storedPassword, password, out bool needsRehash))
            {
                return null;
            }

            int id = Convert.ToInt32(account["UserId"]);
            if (needsRehash)
            {
                db.ExecuteNonQuery(
                    $"UPDATE {accountTable.Value.Table} SET password = @password WHERE {accountTable.Value.IdColumn} = @id",
                    new Dictionary<string, object>
                    {
                        ["@password"] = PasswordSecurity.Hash(password),
                        ["@id"] = id
                    });
            }

            var userSession = new AuthenticatedUser(
                user,
                id,
                Convert.ToString(account["UserName"]) ?? string.Empty,
                Convert.ToInt32(account["CourseId"]),
                Convert.ToString(account["CourseName"]) ?? string.Empty);
            UserSession.SignIn(userSession);
            return userSession;
        }
    }
}
