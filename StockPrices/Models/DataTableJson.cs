using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StockPrices.Models
{
    [DataContract]
    public class DataTableJson
    {
        [DataMember(Name = "data")]
        public List<List<Object>> Data { get; set; }

        [DataMember(Name = "columns")]
        public List<ColumnJson> Columns { get; set; }
    }
}
