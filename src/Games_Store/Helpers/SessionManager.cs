using Games_Store.Models;

namespace Games_Store.Helpers
{
    public static class SessionManager
    {
        public static User? CurrentUser { get; set; }

        public static bool IsAdmin => CurrentUser?.Role == "Admin";

        public static void Clear() => CurrentUser = null;
    }
}
