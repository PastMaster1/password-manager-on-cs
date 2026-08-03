using DotNetEnv;

namespace Password_Manager.Models
{
    internal static class Base
    {
        static public int user_id;
        static public string? connection_string;
        static public byte[]? encryption_key;
        public static void Connection()
        {
            Env.Load();
            string dbPassword = Env.GetString("DB_PASSWORD");
            connection_string = $"Server=localhost;Port=5432;User Id=postgres;Password={dbPassword};Database=Test";
        }
    }
}
