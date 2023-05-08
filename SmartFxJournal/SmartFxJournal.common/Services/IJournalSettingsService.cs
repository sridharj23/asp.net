using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.Common.Services
{
    public interface IJournalSettingsService : INotifyPropertyChanged
    {
        String connectionString { get; }

        public IDictionary<String, String> LoadSettings();

        public void SaveSettings(IDictionary<String, String> settings);
    }
}
