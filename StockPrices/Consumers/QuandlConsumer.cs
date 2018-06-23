using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using StockPrices.Interfaces;
using StockPrices.Models;

namespace StockPrices.Consumers
{
    public class QuandlConsumer : IQuandlConsumer
    {
        private readonly HttpClient _quandlHttpClient = new HttpClient();
        private static readonly String _api_key = "s-GMZ_xkw6CrkGYUWs1p";      // TODO: move credentials to some config

        public QuandlConsumer()
        {
            _quandlHttpClient.DefaultRequestHeaders.Accept.Clear();
            _quandlHttpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }
        /// <summary>
        ///  Bring List of daily data from Quandl for the given period of time 
        /// </summary>
        /// <param name="ticker">Ticker code of the security - part of Quandl query</param>
        /// <param name="startDate">Inclusive start of time period - part of Quandl query</param>
        /// <param name="endDateExclusive">Exclusive end of time period - part of Quandl query</param>
        /// <returns>List<SecurityDaily> with data converted from Quandl response</returns>
        public async Task<List<SecurityDaily>> GetDailyPrices(String ticker, String startDate, String endDateExclusive)
        {
            String url = "https://www.quandl.com/api/v3/datatables/wiki/prices?ticker=" 
                         + ticker
                         + "&date.gte=" + startDate + "&date.lt=" + endDateExclusive + "&api_key=" + _api_key;

            var serializedData = new List<SecurityDaily>();

            var response = await _quandlHttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            serializedData.AddRange(ParseDailyPrices(await response.Content.ReadAsStreamAsync()));

            return serializedData;
        }
        /// <summary>
        ///  Serialize Quandl response stream to the List<SecurityDaily>
        /// </summary>
        /// <param name="stream">Quandl stream</param>
        /// <returns>List<SecurityDaily> with data converted from Quandl response</returns>
        public List<SecurityDaily> ParseDailyPrices(System.IO.Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(ResponseJson));
            var obj = serializer.ReadObject(stream) as ResponseJson;
            var securityList = ConvertResponseToSecurityDailyList(obj);
            return securityList;
        }

        private List<SecurityDaily> ConvertResponseToSecurityDailyList(ResponseJson responseJson)
        {
            List<SecurityDaily> securityList = new List<SecurityDaily>();
            foreach (var day in responseJson.Datatable.Data)
            {
                securityList.Add(new SecurityDaily
                {
                    Ticker = (String)day[0],
                    Date = (String)day[1],
                    Open = (Decimal)day[2],
                    High = (Decimal)day[3],
                    Low = (Decimal)day[4],
                    Close = (Decimal)day[5],
                    Volume = (Decimal)day[6]
                });
            }
            return securityList;
        }
    }
}
