using System.Web;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public static class SessionHelper
    {
        private const string USER_ID_KEY = "UserId";
        private const string USER_EMAIL_KEY = "UserEmail";
        private const string USER_NAME_KEY = "UserName";
        private const string IS_ADMIN_KEY = "IsAdmin";

        public static void SetUserSession(int userId, string email, string name, bool isAdmin)
        {
            HttpContext.Current.Session[USER_ID_KEY] = userId;
            HttpContext.Current.Session[USER_EMAIL_KEY] = email;
            HttpContext.Current.Session[USER_NAME_KEY] = name;
            HttpContext.Current.Session[IS_ADMIN_KEY] = isAdmin;
        }

        public static int? GetUserId()
        {
            return HttpContext.Current.Session[USER_ID_KEY] as int?;
        }

        public static string GetUserEmail()
        {
            return HttpContext.Current.Session[USER_EMAIL_KEY] as string;
        }

        public static string GetUserName()
        {
            return HttpContext.Current.Session[USER_NAME_KEY] as string;
        }

        public static bool IsAdmin()
        {
            return HttpContext.Current.Session[IS_ADMIN_KEY] as bool? ?? false;
        }

        public static bool IsLoggedIn()
        {
            return GetUserId().HasValue;
        }

        public static void ClearSession()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}

