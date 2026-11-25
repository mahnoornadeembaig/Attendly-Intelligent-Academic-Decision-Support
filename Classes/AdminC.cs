using Microsoft.VisualBasic.Devices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Classes
{
    internal class AdminC:Person
    {
        DbConnection connection = new DbConnection();

        public override void create()
        {

        }
        public DataTable read()
        {
            connection = new DbConnection();
            string query = "SELECT Aid,name,email FROM Admin";
            DataTable dt = connection.ExecuteQuery(query, false);
            return dt;
        }


        public override DataTable read(int id)
        {
            connection = new DbConnection();
            string query = "SELECT Aid,name,email FROM Admin WHERE Aid = @id";
            DataTable dt = connection.ExecuteQuery(query, new Dictionary<string, object> { ["@id"] = id });
            return dt;
        }
        public override void update(int id)
        {
            connection = new DbConnection();
            string query = "UPDATE Admin SET name=@name,email=@email,password=CASE WHEN @password='' THEN password ELSE @password END WHERE Aid=@id";
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
            string query = "DELETE FROM Admin WHERE Aid=@id";
            return connection.ExecuteQuery(query, new Dictionary<string, object>
            {
                ["@id"] = id
            });
        }
    }
}
