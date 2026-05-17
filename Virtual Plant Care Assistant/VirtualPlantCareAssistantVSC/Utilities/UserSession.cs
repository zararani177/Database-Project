using VirtualPlantCareAssistantVSC.Models;

namespace VirtualPlantCareAssistantVSC.Utilities
{
    public static class UserSession
    {
        public static User? CurrentUser { get; private set; }

        public static void Login(User user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static bool IsLoggedIn => CurrentUser != null;
    }
}
