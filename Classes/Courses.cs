using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Classes
{
    internal class Courses
    {
        string course;

        public string Course { get => course; set => course = value; }

        public DataTable readCourse()
        {
            DbConnection connection = new DbConnection();
            const string query = @"
                SELECT Cid,name FROM Course
                UNION
                SELECT C_id,Cname FROM ARecord
                UNION
                SELECT CourseId,course FROM Teacher WHERE CourseId > 0 AND TRIM(course) <> ''
                ORDER BY name COLLATE NOCASE";
            return connection.ExecuteQuery(query, false);
        }

        public DataTable readCourse(int courseId)
        {
            DbConnection connection = new DbConnection();
            return connection.ExecuteQuery(
                @"SELECT Cid,name FROM (
                      SELECT Cid,name FROM Course
                      UNION
                      SELECT C_id,Cname FROM ARecord
                      UNION
                      SELECT CourseId,course FROM Teacher WHERE CourseId > 0 AND TRIM(course) <> ''
                  )
                  WHERE Cid = @courseId
                  ORDER BY name COLLATE NOCASE",
                new Dictionary<string, object> { ["@courseId"] = courseId });
        }
    }
}
