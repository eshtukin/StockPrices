using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace StockPrices.Models
{
    [DataContract]
    public class ResponseJson
    {
        [DataMember]
        public DataTableJson datatable { get; set; }
    }
}
