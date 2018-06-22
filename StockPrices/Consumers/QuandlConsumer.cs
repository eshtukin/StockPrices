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
            // HttpClient initialization parameters 
            _quandlHttpClient.DefaultRequestHeaders.Accept.Clear();
            _quandlHttpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

        }


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

        public List<SecurityDaily> ParseDailyPrices(System.IO.Stream stream)
        {
            var serializer = new DataContractJsonSerializer(typeof(ResponseJson));
            var obj = serializer.ReadObject(stream) as ResponseJson;
            var securityList = ConertResponseToSecurityDailyList(obj);
            return securityList;
        }

        private List<SecurityDaily> ConertResponseToSecurityDailyList(ResponseJson responseJson)
        {
            List<SecurityDaily> securityList = new List<SecurityDaily>();
            foreach (var day in responseJson.datatable.data)
            { 
                securityList.Add(new SecurityDaily { 
                                    Ticker = (String)day[0],
                                    Date = (String) day[1],
                                    Open = (decimal) day[2],
                                    High = (decimal) day[3], 
                                    Low = (decimal) day[4],
                                    Close = (decimal) day[5],
                                    Volume = (decimal) day[6]
                               });
            }
            return securityList;
        }
    }
}
