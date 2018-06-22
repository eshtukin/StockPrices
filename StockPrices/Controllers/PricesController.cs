using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StockPrices.Interfaces;

namespace StockPrices.Controllers
{
    [Route("api/[controller]")]
    public class PricesController : Controller
    {
        public class MonthlyAverage
        {
            public String _month;
            public decimal _open;
            public decimal _close;
            public MonthlyAverage(String m, decimal o, decimal c) { _month = m; _open = o; _close = c; }
        }
        public class DailyProfit
        {
            public String _date;
            public decimal _profit = 0;
        }
        public class SecuritySummary
        {
            public String _ticker;
            public IList<MonthlyAverage> _monthlyAverages = new List<MonthlyAverage>();
            public DailyProfit _maxDailyProfit;
            public decimal _averageVolume = 0;
            public int _daysOfLost = 0;
        }

        private readonly IQuandlConsumer _quandlConsumer;

        public PricesController(IQuandlConsumer quandlConsumer)
        {
            _quandlConsumer = quandlConsumer ?? throw new ArgumentNullException("quandlConsumer");
        }

        // GET api/repos
        [HttpGet("{ticker}/{start}/{end}")]
        public IActionResult GetPrices(String ticker, String start, String end)
        {
            SecuritySummary securitySummary = new SecuritySummary();

            DateTime startDate, endDate;
            if (!DateTime.TryParse(start, out startDate) || !DateTime.TryParse(end, out endDate))
                return BadRequest();

            try
            {
                securitySummary._ticker = ticker;
                securitySummary._maxDailyProfit = new DailyProfit();

                int totalDays = 0;
                decimal totalVolume = 0;

                DateTime startMonth = new DateTime(startDate.Year, startDate.Month, 1);
                DateTime startNextMonth = startMonth;


                while (startNextMonth.CompareTo(endDate) <= 0)
                {
                    startNextMonth = startNextMonth.AddMonths(1);

                    var securityData = _quandlConsumer.GetDailyPrices(ticker, startMonth.ToString("yyyy-MM-dd"), startNextMonth.ToString("yyyy-MM-dd")).Result;

                    decimal totalMonthlyOpen = 0;
                    decimal totalMonthlyClose = 0;

                    foreach (var day in securityData)
                    {
                        // 1. Gather Open/Close for monthly average calculations
                        totalMonthlyOpen += day.Open;
                        totalMonthlyClose += day.Close;

                        // 2. Find day with highest daily profit
                        var dayProfit = day.High - day.Low;
                        if (dayProfit > securitySummary._maxDailyProfit._profit) {
                            securitySummary._maxDailyProfit._profit = dayProfit;
                            securitySummary._maxDailyProfit._date = day.Date;
                        }

                        // 3. Gather Volume for average volume calculation
                        totalVolume += day.Volume;

                        // 4. Calculate number of "days of lost"
                        if (day.Close < day.Open)
                            securitySummary._daysOfLost++;
                    }

                    totalDays += securityData.Count;

                    var averageOpen = Decimal.Round(totalMonthlyOpen / securityData.Count, 3);
                    var averageClose = Decimal.Round(totalMonthlyClose / securityData.Count, 3);

                    securitySummary._monthlyAverages.Add(new MonthlyAverage(startMonth.ToString("yyyy-MM"), averageOpen, averageClose));

                    startMonth = startNextMonth;
                }

                securitySummary._averageVolume = totalVolume / totalDays;

                return Ok(securitySummary);

            }  catch (Exception) {
                return NotFound();
            }
        }
    }

}
