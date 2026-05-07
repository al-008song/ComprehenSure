namespace comprehensure.DataBaseControl
{
    public static class UserCache
    {
        
        private const string KeyUid         = "SavedUserUid";
        private const string KeyEmail       = "SavedUserEmail";
        private const string KeyUsername    = "CachedUsername";
        private const string KeyHasUsername = "CachedHasUsername";
        private const string KeyModules     = "CachedModuleFinished";
        private const string KeyScore       = "CachedScoreOfTotal";

       
        public static void SaveUser(string uid, string email, string username,
                                    int moduleFinished = 0, int scoreOfTotal = 0)
        {
            Preferences.Default.Set(KeyUid,         uid);
            Preferences.Default.Set(KeyEmail,       email);
            Preferences.Default.Set(KeyUsername,    username);
            Preferences.Default.Set(KeyHasUsername, true);
            Preferences.Default.Set(KeyModules,     moduleFinished);
            Preferences.Default.Set(KeyScore,       scoreOfTotal);
        }

        public static void SetHasUsername(bool value) =>
            Preferences.Default.Set(KeyHasUsername, value);
         

        
        public static string Uid         => Preferences.Default.Get(KeyUid,          string.Empty);
        public static string Email       => Preferences.Default.Get(KeyEmail,         string.Empty);
        public static string Username    => Preferences.Default.Get(KeyUsername,      string.Empty);
        public static bool HasUsername   => Preferences.Default.Get(KeyHasUsername,   false);
        public static int ModuleFinished => Preferences.Default.Get(KeyModules,       0);
        public static int ScoreOfTotal   => Preferences.Default.Get(KeyScore,         0);

       
        public static bool IsCached(string uid) =>
            !string.IsNullOrEmpty(Uid) && Uid == uid && HasUsername;

        
        public static void Clear() => Preferences.Default.Clear();
    }
}
