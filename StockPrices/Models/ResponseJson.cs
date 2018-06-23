using System.Runtime.Serialization;


namespace StockPrices.Models
{
    [DataContract]
    public class ResponseJson
    {
        [DataMember(Name = "datatable")]
        public DataTableJson Datatable { get; set; }
    }
}
