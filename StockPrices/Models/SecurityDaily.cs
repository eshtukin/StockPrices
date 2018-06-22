using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace StockPrices.Models
{
    [DataContract(Name = "prices")]
    public class SecurityDaily
    {
        [DataMember(Name = "ticker")]
        public String Ticker { get; set; }

        [DataMember(Name = "date")]
        public String Date{ get; set; }

        [DataMember(Name = "open")]
        public decimal Open { get; set; }

        [DataMember(Name = "close")]
        public decimal Close { get; set; }

        [DataMember(Name = "low")]
        public decimal Low { get; set; }

        [DataMember(Name = "high")]
        public decimal High { get; set; }

        [DataMember(Name = "volume")]
        public decimal Volume { get; set; }
    }
}