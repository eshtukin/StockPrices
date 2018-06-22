using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace StockPrices.Models
{
    [DataContract]
    public class DataTableJson
    {
        [DataMember]
        public List<List<Object>> data { get; set; }

        [DataMember]
        public List<ColumnJson> columns { get; set; }
    }
}
