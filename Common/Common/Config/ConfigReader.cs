using System.Configuration;

namespace Common.Config
{
    public  class ConfigReader
    {
        public static string GetAppSettingsValue(string key)
        {
            ConfigurationManager.RefreshSection("appSettings");
            return ConfigurationManager.AppSettings[key] ?? string.Empty;
        }
    }
}
