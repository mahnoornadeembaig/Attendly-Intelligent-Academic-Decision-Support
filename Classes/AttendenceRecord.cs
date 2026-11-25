using System.Data;
using System.Globalization;

namespace AMS.Classes
{
    internal class AttendenceRecord
    {
        private readonly DbConnection connection = new();
        private int courseId;
        private int studentId;
        private string studentName = string.Empty;
        private string courseName = string.Empty;
        private string teacherName = string.Empty;
        private string date = string.Empty;
        private string status = string.Empty;

        public string Date { get => date; set => date = value; }
        public int C_id { get => courseId; set => courseId = value; }
        public string Status { get => status; set => status = value; }
        public int S_id { get => studentId; set => studentId = value; }
        public string S_name { get => studentName; set => studentName = value; }
        public string C_name { get => courseName; set => courseName = value; }
        public string T_name { get => teacherName; set => teacherName = value; }

        public bool Create()
        {
            if (courseId <= 0 || studentId <= 0 ||
                string.IsNullOrWhiteSpace(studentName) ||
                string.IsNullOrWhiteSpace(courseName) ||
                string.IsNullOrWhiteSpace(teacherName) ||
                !DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ||
                !IsPresentStatus(status) && !IsAbsentStatus(status))
            {
                throw new ArgumentException("Attendance details are incomplete or invalid.");
            }

            DataTable students = connection.ExecuteQuery(
                "SELECT 1 FROM Student WHERE Sid = @studentId",
                new Dictionary<string, object> { ["@studentId"] = studentId });
            if (students.Rows.Count == 0)
            {
                throw new ArgumentException("Select a student that exists in the student records.");
            }

            const string query = @"
                INSERT INTO ARecord(Tname,Cname,Sname,Date,status,C_id,S_id)
                SELECT @teacher,@course,@student,@date,@status,@courseId,@studentId
                WHERE NOT EXISTS (
                    SELECT 1 FROM ARecord
                    WHERE S_id=@studentId AND C_id=@courseId AND Date=@date
                )";
            int inserted = connection.ExecuteNonQuery(query, new Dictionary<string, object>
            {
                ["@teacher"] = teacherName.Trim(),
                ["@course"] = courseName.Trim(),
                ["@student"] = studentName.Trim(),
                ["@date"] = date,
                ["@status"] = NormalizeStatus(status),
                ["@courseId"] = courseId,
                ["@studentId"] = studentId
            });

            return inserted > 0;
        }

        public DataTable ReadReport(
            int? allowedStudentId = null,
            int? allowedCourseId = null,
            int? courseIdFilter = null,
            DateTime? from = null,
            DateTime? to = null,
            string? statusFilter = null,
            string? search = null,
            string? studentNameFilter = null)
        {
            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (allowedStudentId.HasValue)
            {
                conditions.Add("S_id = @allowedStudentId");
                parameters["@allowedStudentId"] = allowedStudentId.Value;
            }
            if (allowedCourseId.HasValue)
            {
                conditions.Add("C_id = @allowedCourseId");
                parameters["@allowedCourseId"] = allowedCourseId.Value;
            }
            if (courseIdFilter.HasValue)
            {
                conditions.Add("C_id = @courseId");
                parameters["@courseId"] = courseIdFilter.Value;
            }
            if (from.HasValue)
            {
                conditions.Add("Date >= @fromDate");
                parameters["@fromDate"] = from.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            if (to.HasValue)
            {
                conditions.Add("Date <= @toDate");
                parameters["@toDate"] = to.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "All")
            {
                conditions.Add(statusFilter == "Present"
                    ? "LOWER(status) IN ('p','present')"
                    : "LOWER(status) IN ('a','absent')");
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                conditions.Add("(CAST(S_id AS TEXT) LIKE @search OR Sname LIKE @search OR Cname LIKE @search OR Tname LIKE @search)");
                parameters["@search"] = $"%{search.Trim()}%";
            }
            if (!string.IsNullOrWhiteSpace(studentNameFilter))
            {
                conditions.Add("LOWER(Sname) LIKE LOWER(@studentName)");
                parameters["@studentName"] = $"%{studentNameFilter.Trim()}%";
            }

            string where = conditions.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", conditions)}";
            string query = $@"
                SELECT S_id AS StudentID,Sname AS Student,C_id AS CourseID,Cname AS Course,
                       Tname AS Teacher,Date,status AS Status
                FROM ARecord {where}
                ORDER BY Date DESC,Sname COLLATE NOCASE";
            return connection.ExecuteQuery(query, parameters);
        }

        public DataTable ReadReportCourses(int? allowedStudentId = null, int? allowedCourseId = null)
        {
            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();
            if (allowedStudentId.HasValue)
            {
                conditions.Add("S_id = @studentId");
                parameters["@studentId"] = allowedStudentId.Value;
            }
            if (allowedCourseId.HasValue)
            {
                conditions.Add("C_id = @courseId");
                parameters["@courseId"] = allowedCourseId.Value;
            }

            string where = conditions.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", conditions)}";
            return connection.ExecuteQuery(
                $"SELECT DISTINCT C_id AS CourseID,Cname AS Course FROM ARecord {where} ORDER BY Cname COLLATE NOCASE",
                parameters);
        }

        public DataTable ReadReportStudents(int? allowedStudentId = null, int? allowedCourseId = null)
        {
            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();
            string source = "Student s";
            if (allowedCourseId.HasValue)
            {
                source += " INNER JOIN ARecord a ON a.S_id = s.Sid";
                conditions.Add("a.C_id = @courseId");
                parameters["@courseId"] = allowedCourseId.Value;
            }
            if (allowedStudentId.HasValue)
            {
                conditions.Add("s.Sid = @studentId");
                parameters["@studentId"] = allowedStudentId.Value;
            }

            string where = conditions.Count == 0 ? string.Empty : $"WHERE {string.Join(" AND ", conditions)}";
            return connection.ExecuteQuery(
                $"SELECT DISTINCT s.Sid AS StudentID,s.name AS Student FROM {source} {where} ORDER BY Student COLLATE NOCASE",
                parameters);
        }

        public DataTable read(string date)
        {
            return ReadReport(from: DateTime.Parse(date, CultureInfo.CurrentCulture).Date,
                to: DateTime.Parse(date, CultureInfo.CurrentCulture).Date);
        }

        public DataTable read(int id)
        {
            return ReadReport(allowedStudentId: id);
        }

        public DataTable read()
        {
            return ReadReport();
        }

        public DataTable readStudents()
        {
            return connection.ExecuteQuery("SELECT Sid,name FROM Student ORDER BY name COLLATE NOCASE");
        }

        public DataTable readCourse()
        {
            return new Courses().readCourse();
        }

        public DataTable readCourse(int courseId)
        {
            return new Courses().readCourse(courseId);
        }

        public DataTable readDate()
        {
            return connection.ExecuteQuery("SELECT DISTINCT Date FROM ARecord ORDER BY Date DESC");
        }

        public DataTable delete(int id)
        {
            connection.ExecuteNonQuery(
                "DELETE FROM ARecord WHERE S_id = @studentId",
                new Dictionary<string, object> { ["@studentId"] = id });
            return new DataTable();
        }

        private static bool IsPresentStatus(string value)
        {
            return value.Equals("P", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Present", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAbsentStatus(string value)
        {
            return value.Equals("A", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Absent", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeStatus(string value)
        {
            return IsPresentStatus(value) ? "Present" : "Absent";
        }
    }
}
