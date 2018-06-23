using System;

namespace StockPrices.Models
{
    public class SecurityDaily
    {
        public String Ticker { get; set; }
        public String Date{ get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Low { get; set; }
        public decimal High { get; set; }
        public decimal Volume { get; set; }
    }
}