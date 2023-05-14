using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartFxJournal.CTrader.Models
{
    public class OnBoardingResult
    {
        public OnBoardingResult(String cTraderId)
        {
            CTraderID = cTraderId;
            OnBoardingStatus = OnBoardStatus.Unknown;
        }

        public OnBoardingResult(String cTraderId, OnBoardStatus status)
        {
            CTraderID = cTraderId;
            OnBoardingStatus = status;
        }

        public String CTraderID {  get; set; }
        public OnBoardStatus OnBoardingStatus { get; set; }
        public String ErrorDescription { get; set; } = "";

        public OnBoardingResult Copy() {
            return new(this.CTraderID, this.OnBoardingStatus);
        }
    }

    public enum OnBoardStatus
    {
        Unknown,
        Failed,
        Success,
        Pending
    }
}
