using Microsoft.Data.Sqlite;
using System.Data;
using System.Collections.Generic;
using System.Globalization;

namespace AMS.Classes
{
    sealed internal class DbConnection
    {
        private static readonly object InitializationLock = new();
        private static readonly string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AMS",
            "ams.db");
        private static readonly string ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            ForeignKeys = true
        }.ToString();
        private static bool initialized;

        public DbConnection()
        {
            EnsureInitialized();
        }

        public DataTable ExecuteQuery(string query)
        {
            return ExecuteQuery(query, false);
        }

        public DataTable ExecuteQuery(string query, IReadOnlyDictionary<string, object> parameters)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            using var reader = command.ExecuteReader();
            var result = new DataTable();
            if (reader.FieldCount > 0)
            {
                result.Load(reader);
            }

            return result;
        }

        public int ExecuteNonQuery(string query, IReadOnlyDictionary<string, object> parameters)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = new SqliteCommand(query, connection);
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            return command.ExecuteNonQuery();
        }

        public DataTable ExecuteQuery(string query, bool flag)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = new SqliteCommand(query, connection);

            if (flag)
            {
                command.ExecuteNonQuery();
                NormalizeAttendanceRows(connection);
                return null;
            }

            using var reader = command.ExecuteReader();
            var result = new DataTable();
            if (reader.FieldCount > 0)
            {
                result.Load(reader);
            }

            return result;
        }

        private static void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            lock (InitializationLock)
            {
                if (initialized)
                {
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(DatabasePath)!);
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Admin (
                        Aid INTEGER PRIMARY KEY,
                        name TEXT NOT NULL,
                        email TEXT NOT NULL UNIQUE,
                        password TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Student (
                        Sid INTEGER PRIMARY KEY,
                        name TEXT NOT NULL,
                        email TEXT NOT NULL UNIQUE,
                        password TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS Teacher (
                        Tid INTEGER PRIMARY KEY,
                        name TEXT NOT NULL,
                        email TEXT NOT NULL UNIQUE,
                        password TEXT NOT NULL,
                        qualification TEXT NOT NULL,
                        course TEXT NOT NULL,
                        CourseId INTEGER NOT NULL DEFAULT 0
                    );
                    CREATE TABLE IF NOT EXISTS Course (
                        Cid INTEGER PRIMARY KEY,
                        name TEXT NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS CourseRegistration (
                        CRid INTEGER PRIMARY KEY,
                        Cid INTEGER NOT NULL,
                        Sid INTEGER NOT NULL
                    );
                    CREATE TABLE IF NOT EXISTS ARecord (
                        Tname TEXT NOT NULL,
                        Cname TEXT NOT NULL,
                        Sname TEXT NOT NULL,
                        Date TEXT NOT NULL,
                        status TEXT NOT NULL,
                        C_id INTEGER NOT NULL,
                        S_id INTEGER NOT NULL
                    );
                    INSERT INTO Admin (Aid, name, email, password)
                    SELECT 1, 'Administrator', 'admin@ams.com', 'admin123'
                    WHERE NOT EXISTS (SELECT 1 FROM Admin);";
                command.ExecuteNonQuery();

                using var teacherColumns = new SqliteCommand("PRAGMA table_info(Teacher)", connection);
                using var reader = teacherColumns.ExecuteReader();
                bool hasCourseId = false;
                while (reader.Read())
                {
                    if (string.Equals(reader["name"].ToString(), "CourseId", StringComparison.OrdinalIgnoreCase))
                    {
                        hasCourseId = true;
                        break;
                    }
                }

                reader.Close();
                if (!hasCourseId)
                {
                    using var migration = new SqliteCommand(
                        "ALTER TABLE Teacher ADD COLUMN CourseId INTEGER NOT NULL DEFAULT 0",
                        connection);
                    migration.ExecuteNonQuery();
                }

                initialized = true;
            }
        }

        private static void NormalizeAttendanceRows(SqliteConnection connection)
        {
            var rows = new List<(long Id, string Date, string Status)>();
            using (var readCommand = new SqliteCommand("SELECT rowid,Date,status FROM ARecord", connection))
            using (var reader = readCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    rows.Add((reader.GetInt64(0), reader.GetString(1), reader.GetString(2)));
                }
            }

            using var updateCommand = new SqliteCommand(
                "UPDATE ARecord SET Date=@date,status=@status WHERE rowid=@id",
                connection);
            updateCommand.Parameters.Add("@date", SqliteType.Text);
            updateCommand.Parameters.Add("@status", SqliteType.Text);
            updateCommand.Parameters.Add("@id", SqliteType.Integer);

            foreach (var row in rows)
            {
                string normalizedDate = row.Date;
                if (DateTime.TryParse(row.Date, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime currentDate) ||
                    DateTime.TryParse(row.Date, CultureInfo.InvariantCulture, DateTimeStyles.None, out currentDate))
                {
                    normalizedDate = currentDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                string normalizedStatus = row.Status.Equals("P", StringComparison.OrdinalIgnoreCase)
                    ? "Present"
                    : row.Status.Equals("A", StringComparison.OrdinalIgnoreCase)
                        ? "Absent"
                        : row.Status;
                if (normalizedDate != row.Date || normalizedStatus != row.Status)
                {
                    updateCommand.Parameters["@date"].Value = normalizedDate;
                    updateCommand.Parameters["@status"].Value = normalizedStatus;
                    updateCommand.Parameters["@id"].Value = row.Id;
                    updateCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
