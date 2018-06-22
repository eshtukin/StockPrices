using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockPrices.Models;

namespace StockPrices.Interfaces
{
    public interface IQuandlConsumer
    {
        Task<List<SecurityDaily>> GetDailyPrices(String ticker, String startDate, String endDateExclusive);
    }
}
