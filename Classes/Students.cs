using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AMS.Classes;
using static System.Collections.Specialized.BitVector32;

namespace AMS.Classes
{
    internal class Students:Person
    {
        string course;
        DbConnection connection;
        public string Course { get => course; set => course = value; }

        public override void create()
        {
            connection = new DbConnection();
            string query = "INSERT INTO Student(Sid,name,email,password) VALUES(@id,@name,@email,@password)";
            connection.ExecuteNonQuery(query, new Dictionary<string, object>
            {
                ["@id"] = Id,
                ["@name"] = Name,
                ["@email"] = Email,
                ["@password"] = PasswordSecurity.Hash(Password)
            });
        }
        public DataTable read()
        {
            connection = new DbConnection();
            string query = "SELECT Sid,name,email FROM Student ORDER BY name COLLATE NOCASE";
            DataTable dt = connection.ExecuteQuery(query, false);
            return dt;
        }
        
        
        public override DataTable read(int id)
        {
            connection = new DbConnection();
            string query = "SELECT Sid,name,email FROM Student WHERE Sid = @id";
            DataTable dt = connection.ExecuteQuery(query, new Dictionary<string, object> { ["@id"] = id });
            return dt;
        }
        public override void update(int id)
        {
            connection = new DbConnection();
            string query = "UPDATE Student SET name=@name,email=@email,password=CASE WHEN @password='' THEN password ELSE @password END WHERE Sid=@id";
            connection.ExecuteNonQuery(query, new Dictionary<string, object>
            {
                ["@name"] = Name,
                ["@email"] = Email,
                ["@password"] = string.IsNullOrWhiteSpace(Password) ? string.Empty : PasswordSecurity.Hash(Password),
                ["@id"] = id
            });
        }
        public override DataTable delete(int id)
        {
            connection = new DbConnection();
            string query = $"delete from Student where Sid = {id}";
            DataTable dt = (DataTable) connection.ExecuteQuery(query, false);
            return dt;
        }
    }
}
