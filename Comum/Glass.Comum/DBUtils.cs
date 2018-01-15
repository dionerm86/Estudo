using System;
using GDA.Provider;
using MySql.Data.MySqlClient;

namespace Glass
{
    public static class DBUtils
    {
        #region DBUtils

        private static object _syncRoot;
        private static ProviderConfiguration _dbProvider;
        private static string _connString;

        static DBUtils()
        {
            _syncRoot = new object();
        }

        private static string GetDBValue(string key)
        {
            string connString = GetConnString.Replace(" ", "").ToLower();
            connString = connString.Substring(connString.IndexOf(key + "="));
            return connString.Split('=')[1].Split(';')[0];
        }

        public static ProviderConfiguration DbProvider
        {
            get
            {
                if (_dbProvider == null)
                {
                    Provider prov = new Provider("MYSQL", typeof(MySqlConnection), typeof(MySqlDataAdapter), typeof(MySqlCommand), typeof(MySqlParameter), "?", "SELECT @@IDENTITY;", true);
                    prov.ExecuteCommandsOneAtATime = false;

                    _dbProvider = new ProviderConfiguration(GetConnString, prov);
                }

                return _dbProvider;
            }
        }

        public static string GetConnString
        {
            get
            {
                return GDA.GDASettings.DefaultProviderConfiguration.ConnectionString;
            }
        }

        internal static string GetAppSetting(string appSetting)
        {
            return System.Configuration.ConfigurationSettings.AppSettings[appSetting];
        }

        public static string GetDBName
        {
            get { return GetDBValue("initialcatalog"); }
        }

        public static string GetDBUser
        {
            get { return GetDBValue("uid"); }
        }

        public static string GetDBPassword
        {
            get { return GetDBValue("pwd"); }
        }

        public static string GetDBSource
        {
            get { return GetDBValue("datasource"); }
        }

        public static string GetDBServer
        {
            get { return "mysql.syncsoftwares.com.br"; }
        }

        #endregion
    }
}
