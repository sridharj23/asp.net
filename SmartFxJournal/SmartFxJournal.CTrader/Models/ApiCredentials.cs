namespace SmartFxJournal.CTrader.Models
{
    public class ApiCredentials
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }

        public bool IsValid()
        {
            return ClientId != null && Secret != null;
        }
    }
}