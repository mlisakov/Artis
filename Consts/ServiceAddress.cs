namespace Artis.Consts
{
    public static class ServiceAddress
    {
//        public static string ArtisAdminServiceAddress = "http://localhost:55601/";
//        public static string ArtisAdminServiceAddress = "http://92.53.105.145:8110/";

        /// <summary>
        /// Строка подключения к базе
        /// </summary>
        public static string ArtisConnectionString { get; private set; }

        public static void SetConnectionString(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                ArtisConnectionString = connectionString;
            }
        }
    }
}
