using System;
using System.Configuration;
using Protocolo.Interfaces;

namespace Protocolo
{
    public partial class SettingsManager : ISettingsManager
    {
        string ISettingsManager.ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key] ?? string.Empty;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                return string.Empty;
            }
        }
    }
}