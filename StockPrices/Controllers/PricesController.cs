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
            public Decimal _open;
            public Decimal _close;
            public MonthlyAverage(String m, Decimal o, Decimal c) { _month = m; _open = o; _close = c; }
        }
        public class DailyProfit
        {
            public String _date;
            public Decimal _profit;
        }
        public class DailyVolume
        {
            public String _date;
            public Decimal _volume;
            public DailyVolume(String d, Decimal v) { _date = d; _volume = v; }
        }
        public class SecuritySummary
        {
            public String _ticker;
            public IList<MonthlyAverage> _monthlyAverages = new List<MonthlyAverage>();
            public DailyProfit _maxDailyProfit;
            public Decimal _averageVolume;
            public int _daysOfLoss = 0;
        }

        private readonly IQuandlConsumer _quandlConsumer;

        public PricesController(IQuandlConsumer quandlConsumer)
        {
            _quandlConsumer = quandlConsumer ?? throw new ArgumentNullException("quandlConsumer");
        }

        /// <summary>
        ///  GET /api/Prices/GetPrices - to bring SecuritySummmary for given security and time period
        /// </summary>
        /// <param name="ticker">Security ticker code</param>
        /// <param name="start">Start of the time period</param>
        /// <param name="end">End of the time period</param>
        /// <returns>SecuritySummmary object with data from Quandl</returns>
        [HttpGet("{GetPrices}/{ticker}/{start}/{end}")]
        public IActionResult GetPrices(String ticker, String start, String end)
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(start, out startDate) || !DateTime.TryParse(end, out endDate))
                return BadRequest();

            try
            {
                SecuritySummary securitySummary = new SecuritySummary
                {
                    _ticker = ticker,
                    _maxDailyProfit = new DailyProfit()
                };

                int totalDays = 0;
                Decimal totalVolume = 0;

                DateTime startMonth = new DateTime(startDate.Year, startDate.Month, 1);
                DateTime startNextMonth = startMonth;

                // Loop through months - from 1st day of the month (inclusively) till 1st day of the next month (exclusively)
                while (startNextMonth.CompareTo(endDate) <= 0)
                {
                    startNextMonth = startNextMonth.AddMonths(1);

                    // Bring collection of daily data for the whole month
                    var securityData = _quandlConsumer.GetDailyPrices(ticker, startMonth.ToString("yyyy-MM-dd"), startNextMonth.ToString("yyyy-MM-dd")).Result;

                    Decimal totalMonthlyOpen = 0;
                    Decimal totalMonthlyClose = 0;

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

                        // 4. Update number of "days of loss"
                        if (day.Close < day.Open)
                            securitySummary._daysOfLoss++;
                    }

                    totalDays += securityData.Count;

                    var averageOpen = Decimal.Round(totalMonthlyOpen / securityData.Count, 3);
                    var averageClose = Decimal.Round(totalMonthlyClose / securityData.Count, 3);

                    // Add new entry to the list of monthly averages
                    securitySummary._monthlyAverages.Add(new MonthlyAverage(startMonth.ToString("yyyy-MM"), averageOpen, averageClose));

                    startMonth = startNextMonth;
                }

                // Keep security average volume for potential future use
                securitySummary._averageVolume = Decimal.Round(totalVolume / totalDays, 0);

                return Ok(securitySummary);

            }  catch (Exception) {
                return NotFound();
            }
        }

        /// <summary>
        ///  GET api/Prices/GetBusyDays - to bring "busy days" with Volume 10% higher than security average volume for a given period
        /// </summary>
        /// <param name="ticker">Security ticker code</param>
        /// <param name="start">Start of the time period</param>
        /// <param name="end">End of the time period</param>
        /// <param name="aveVolume">Average volume calculated before</param>
        /// <returns>List<DailyVolume> with "busy days"</returns>
        [HttpGet("{GetBusyDays}/{ticker}/{start}/{end}/{aveVolume}")]
        public IActionResult GetBusyDays(String ticker, String start, String end, Decimal aveVolume)
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(start, out startDate) || !DateTime.TryParse(end, out endDate))
                return BadRequest();

            try
            {
                IList<DailyVolume> busyDays = new List<DailyVolume>();

                DateTime startMonth = new DateTime(startDate.Year, startDate.Month, 1);
                DateTime startNextMonth = startMonth;

                // Loop through months
                while (startNextMonth.CompareTo(endDate) <= 0)
                {
                    startNextMonth = startNextMonth.AddMonths(1);

                    // Bring collection of daily data for the whole month
                    var securityData = _quandlConsumer.GetDailyPrices(ticker, startMonth.ToString("yyyy-MM-dd"), startNextMonth.ToString("yyyy-MM-dd")).Result;

                    foreach (var day in securityData)
                    {
                        // Gather days with Volume 10% higher than security average volume
                        if (day.Volume - aveVolume > aveVolume / 10)
                        {
                            busyDays.Add(new DailyVolume(day.Date, day.Volume));
                        }
                    }
                    startMonth = startNextMonth;
                }

                return Ok(busyDays);

            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
