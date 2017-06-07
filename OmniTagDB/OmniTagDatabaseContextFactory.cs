namespace OmniTagDB
{
    public static class OmniTagDatabaseContextFactory
    {
        #if PORTABLE
        public const bool IsAppPortable = true;
        #else
        public const bool IsAppPortable = false;
        #endif

        private static OmniTagContext ContextInstance { get; set; }

        public static OmniTagContext GetContextInstance()
        {
            if (IsAppPortable)
            {
                if (ContextInstance == null)
                    ContextInstance = (OmniTagContext)new SQLiteDatabaseContext();
                return ContextInstance;
            }
            else
            {
                if (ContextInstance == null)
                    ContextInstance = (OmniTagContext)new SQLServerDatabaseContext();
                return ContextInstance;
            }
        }

        public static OmniTagContext GetNewContext()
        {
            return IsAppPortable 
                ? (OmniTagContext)new SQLiteDatabaseContext() 
                : (OmniTagContext)new SQLServerDatabaseContext();
        }
    }
}
