using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFxJournal.Common.Services;

namespace SmartFxJournal.Configuration.Services
{
    public class JournalSettingsServiceImpl : IJournalSettingsService
    {
        private Dictionary<string, string>? _userSettings;

        string IJournalSettingsService.connectionString => JournalSettings.Default.DbConnection;

        public event PropertyChangedEventHandler? PropertyChanged;

        public IDictionary<string, string> LoadSettings()
        {
            if (_userSettings is null)
            {
                _userSettings = new Dictionary<string, string>();
                var appSettings = JournalSettings.Default.Properties;

                if (appSettings.Count > 0)
                {
                    foreach (SettingsProperty prop in appSettings)
                    {
                        _userSettings.Add(prop.Name, JournalSettings.Default[prop.Name].ToString() ?? "");
                    }
                }

            }

            // Return a copy
            return new SortedDictionary<String, String>(_userSettings);
        }

        public void SaveSettings(IDictionary<string, string> settings)
        {
            foreach (var key in settings)
            {
                JournalSettings.Default[key.Key] = settings[key.Value];
                if (_userSettings is not null)
                    _userSettings[key.Key] = key.Value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key.Key));

            }

            JournalSettings.Default.Save();
        }


    }
}
