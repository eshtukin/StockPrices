using System.Runtime.Serialization;

namespace StockPrices.Models
{
    [DataContract]
    public class ColumnJson
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
