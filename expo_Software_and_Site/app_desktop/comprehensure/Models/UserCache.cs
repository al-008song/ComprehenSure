namespace comprehensure.DataBaseControl
{
    public static class UserCache
    {
        private const string KeyUid                  = "SavedUserUid";
        private const string KeyEmail                = "SavedUserEmail";
        private const string KeyUsername             = "CachedUsername";
        private const string KeyHasUsername          = "CachedHasUsername";
        private const string KeyIsFirstLogin         = "IsFirstLogin";
        private const string KeyAccountComprehension = "CachedAccountComprehension";
        private const string KeyScoreOfTotal         = "CachedScoreOfTotal";
        private const string KeyMinigameLocked = "CachedMinigameLocked";
        private const string KeyModules = "CachedModuleFinished";


   
        public static void SaveUser(string uid, string email, string username, int scoreOfTotal = 0, int moduleFinished = 0)
        {
            Preferences.Default.Set(KeyUid,          uid);
            Preferences.Default.Set(KeyEmail,        email);
            Preferences.Default.Set(KeyUsername,     username);
            Preferences.Default.Set(KeyHasUsername,  true);
            Preferences.Default.Set(KeyScoreOfTotal, scoreOfTotal);
            Preferences.Default.Set(KeyModules, moduleFinished);
        }

        public static void SetHasUsername(bool value)        => Preferences.Default.Set(KeyHasUsername,         value);
        public static void SetUsername(string value)         => Preferences.Default.Set(KeyUsername,            value);
        public static void SetFirstLogin(bool value)         => Preferences.Default.Set(KeyIsFirstLogin,        value);
        public static void SetAccountComprehension(double v) => Preferences.Default.Set(KeyAccountComprehension, v);
        public static void SetScoreOfTotal(int value)        => Preferences.Default.Set(KeyScoreOfTotal,        value);
        public static void SetMinigameLocked(bool value) => Preferences.Default.Set(KeyMinigameLocked, value);
        public static int ModuleFinished => Preferences.Default.Get(KeyModules, 0);

        public static void SetModuleLocked(int module, bool locked)
        {
            if (module < 1 || module > 8) return;
            Preferences.Default.Set($"CachedModuleLocked{module}", locked);
        }

        public static void SetModuleProgress(int module, int calculatedProg)
        {
            if (module < 1 || module > 8) return;
            Preferences.Default.Set($"CachedCalcProg{module}", calculatedProg);
        }

    
        public static void SaveModuleSnapshot(bool[] locks, bool minigameLocked, int[] progress)
        {
            if (locks.Length != 8 || progress.Length != 8)
                throw new ArgumentException("locks and progress must each have exactly 8 elements.");

            for (int i = 0; i < 8; i++)
            {
                Preferences.Default.Set($"CachedModuleLocked{i + 1}", locks[i]);
                Preferences.Default.Set($"CachedCalcProg{i + 1}",     progress[i]);
            }
            Preferences.Default.Set(KeyMinigameLocked, minigameLocked);
        }

       

        public static string Uid                  => Preferences.Default.Get(KeyUid,                  string.Empty);
        public static string Email                => Preferences.Default.Get(KeyEmail,                string.Empty);
        public static string Username             => Preferences.Default.Get(KeyUsername,             string.Empty);
        public static bool   HasUsername          => Preferences.Default.Get(KeyHasUsername,          false);
        public static bool   IsFirstLogin         => Preferences.Default.Get(KeyIsFirstLogin,         false);
        public static double AccountComprehension => Preferences.Default.Get(KeyAccountComprehension, 0.0);
        public static int    ScoreOfTotal         => Preferences.Default.Get(KeyScoreOfTotal,         0);
        public static bool   MinigameLocked       => Preferences.Default.Get(KeyMinigameLocked,       true);

        public static bool GetModuleLocked(int module)
        {
            if (module < 1 || module > 8) return true;
            return Preferences.Default.Get($"CachedModuleLocked{module}", module > 1);
        }

        public static int GetModuleProgress(int module)
        {
            if (module < 1 || module > 8) return 0;
            return Preferences.Default.Get($"CachedCalcProg{module}", 0);
        }

       
        public static bool IsCached(string uid) =>
            !string.IsNullOrEmpty(Uid) && Uid == uid && HasUsername;

        public static void Clear() => Preferences.Default.Clear();
    }
}
