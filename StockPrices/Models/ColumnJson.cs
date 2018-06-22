using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StockPrices.Models
{
    [DataContract]
    public class ColumnJson
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string type { get; set; }
    }
}
