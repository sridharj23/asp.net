using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.CTrader.Models
{
    public class AccountEntry
    {
        public string CTraderId {get; set; } = null!;

        public string AccountId { get; set; } = null!;

        public string BrokerName { get; set; } = null!;

        public bool IsLive { get; set; } = false;

        public bool IsImported { get; set; } = false;

        public double Balance { get; set; }
    }
}
