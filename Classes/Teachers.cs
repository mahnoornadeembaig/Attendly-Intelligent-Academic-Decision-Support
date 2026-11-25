using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Classes
{
    internal class Teachers:Person
    {
        string course;
        string qualification;
        int courseId;
        DbConnection connection;
        public string Course { get => course; set => course = value; }
        public string Qualification { get => qualification; set => qualification = value; }
        public int CourseId { get => courseId; set => courseId = value; }

        public override void create()
        {
            connection = new DbConnection();
            string query = "INSERT INTO Teacher(Tid,name,email,password,qualification,course,CourseId) VALUES(@id,@name,@email,@password,@qualification,@course,@courseId)";
            connection.ExecuteNonQuery(query, new Dictionary<string, object>
            {
                ["@id"] = Id,
                ["@name"] = Name,
                ["@email"] = Email,
                ["@password"] = PasswordSecurity.Hash(Password),
                ["@qualification"] = Qualification,
                ["@course"] = Course,
                ["@courseId"] = CourseId
            });
        }
        public DataTable read()
        {
            connection = new DbConnection();
            string query = "SELECT Tid,name,email,qualification,course,CourseId FROM Teacher";
            DataTable dt = connection.ExecuteQuery(query, false);
            return dt;
        }
        public override DataTable read(int id)
        {
            connection = new DbConnection();
            string query = "SELECT Tid,name,email,qualification,course,CourseId FROM Teacher WHERE Tid = @id";
            DataTable dt = connection.ExecuteQuery(query, new Dictionary<string, object> { ["@id"] = id });
            return dt;
        }
        public override void update(int id)
        {
            connection = new DbConnection();
            string query = "UPDATE Teacher SET name=@name,email=@email,password=CASE WHEN @password='' THEN password ELSE @password END,qualification=@qualification,course=@course,CourseId=@courseId WHERE Tid=@id";
            connection.ExecuteNonQuery(query, new Dictionary<string, object>
            {
                ["@name"] = Name,
                ["@email"] = Email,
                ["@password"] = string.IsNullOrWhiteSpace(Password) ? string.Empty : PasswordSecurity.Hash(Password),
                ["@qualification"] = Qualification,
                ["@course"] = Course,
                ["@courseId"] = CourseId,
                ["@id"] = id
            });
        }
        public override DataTable delete(int id)
        {
            connection = new DbConnection();
            string query = $"delete from Teacher where Tid = {id}";
            DataTable dt = (DataTable)connection.ExecuteQuery(query, false);
            return dt;
        }
        public DataTable readTeacher()
        {
            DbConnection connection = new DbConnection();
            string query = $"select name from Teacher";
            DataTable dt = (DataTable)connection.ExecuteQuery(query, false);
            return dt;
        }

        public DataTable readAssignedTeachers()
        {
            DbConnection connection = new DbConnection();
            return connection.ExecuteQuery(
                "SELECT Tid,name,CourseId,course FROM Teacher WHERE CourseId > 0 ORDER BY name COLLATE NOCASE");
        }

    }
}
