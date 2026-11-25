using System.Net.Mail;

namespace AMS.Classes
{
    internal static class InputValidation
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                return new MailAddress(email).Address.Equals(email, StringComparison.OrdinalIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsValidPassword(string password)
        {
            return password.Length >= 8;
        }
    }
}
