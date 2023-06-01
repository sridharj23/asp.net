namespace SmartFxJournal.Common.Model
{
    public class SummaryAggregate 
    {
        public SummaryAggregate(string key) {
            AggregateKey = key;
        }
        
        public string AggregateKey {get; set;} = null!;

        public decimal TotalPL {get; set;} = decimal.Zero;

        public decimal PLFromShorts {get; set;} = decimal.Zero;

        public decimal PLFromLongs {get; set;} = decimal.Zero;
    }
}