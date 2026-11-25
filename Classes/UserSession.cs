namespace AMS.Classes
{
    internal sealed class AuthenticatedUser
    {
        public string Role { get; }
        public int Id { get; }
        public string Name { get; }
        public int CourseId { get; }
        public string CourseName { get; }

        public AuthenticatedUser(string role, int id, string name, int courseId, string courseName)
        {
            Role = role;
            Id = id;
            Name = name;
            CourseId = courseId;
            CourseName = courseName;
        }
    }

    internal static class UserSession
    {
        public static AuthenticatedUser? Current { get; private set; }

        public static void SignIn(AuthenticatedUser user)
        {
            Current = user;
        }

        public static void SignOut()
        {
            Current = null;
        }
    }
}
