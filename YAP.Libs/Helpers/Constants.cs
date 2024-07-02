namespace YAP.Libs.Helpers
{
    public static class Constants
    {
        public const double DefaultBoxSize = 50.0;
        public const double DefaultDotSize = 20.0;
        public const double DefaultBoxSpacing = 5.0;
        public const int DefaultPINLength = 4;

        public static Color DefaultColor = Colors.Black;
        public static Color DefaultBoxBackgroundColor = Colors.Transparent;

        public const string DatabaseFilename = "BaseSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}
