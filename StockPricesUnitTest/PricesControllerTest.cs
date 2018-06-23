
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockPrices.Controllers;
using StockPrices.Interfaces;
using StockPrices.Models;
using Xunit;
    

namespace StockPricesUnitTest
{
    public class PricesControllerTest
    {
        readonly Mock<IQuandlConsumer> _mockQuandlConsumer = new Mock<IQuandlConsumer>();

        [Fact]
        public void GetPrices_MonthlyAverage()
        {
            // Arrange
            List<SecurityDaily> dataFromQuandl = new List<SecurityDaily>()
            {
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-04", Open = (Decimal)762.2, High = (Decimal)762.2, Low = (Decimal)747.54, Close = (Decimal)759.44, Volume = (Decimal)3369068.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-05", Open = (Decimal)764.1, High = (Decimal)769.2, Low = (Decimal)755.65, Close = (Decimal)761.53, Volume = (Decimal)2260795.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-06", Open = (Decimal)750.37, High = (Decimal)765.73, Low = (Decimal)748.0, Close = (Decimal)759.33, Volume = (Decimal)2410301.0 } }
            };

            String ticker = "GOOGL";
            // Quandl should be called only once: for 2016-01
            String start = "2016-01-05";
            String end = "2016-01-23";

            String startMonth = "2016-01-01";
            String startNextMonth = "2016-02-01";

            // Act
            _mockQuandlConsumer.Setup(x => x.GetDailyPrices(ticker, startMonth, startNextMonth))
                .Returns(Task.FromResult(dataFromQuandl));

            var controller = new PricesController(_mockQuandlConsumer.Object);
            var result = controller.GetPrices(ticker, start, end);

            // Assert
            _mockQuandlConsumer.Verify(x => x.GetDailyPrices(ticker, startMonth, startNextMonth), Times.Once());
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var securitySummary = okResult.Value.Should().BeAssignableTo<PricesController.SecuritySummary>().Subject;
            securitySummary._monthlyAverages.Count().Should().Be(1);
            securitySummary._monthlyAverages.First().As<PricesController.MonthlyAverage>()._month.Should().Be("2016-01");
            securitySummary._monthlyAverages.First().As<PricesController.MonthlyAverage>()._open.Should().Be((Decimal)758.89);
            securitySummary._monthlyAverages.First().As<PricesController.MonthlyAverage>()._close.Should().Be((Decimal)760.1);
        }

        [Fact]
        public void GetPrices_FourMonths()
        {
            // Arrange
            List<SecurityDaily> dataFromQuandl = new List<SecurityDaily>()
            {
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-04", Open = (Decimal)762.2, High = (Decimal)762.2, Low = (Decimal)747.54, Close = (Decimal)759.44, Volume = (Decimal)3369068.0 } }
            };

            // Quandl should be called 4 times: for 2016-01, 2016-02, 2016-03, 2016-04
            String ticker = "GOOGL";
            String start = "2016-01-12";
            String end = "2016-04-23";

            // Act
            _mockQuandlConsumer.Setup(x => x.GetDailyPrices(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()))
                .Returns(Task.FromResult(dataFromQuandl));

            var controller = new PricesController(_mockQuandlConsumer.Object);
            var result = controller.GetPrices(ticker, start, end);

            // Assert
            _mockQuandlConsumer.Verify(x => x.GetDailyPrices(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<String>()), Times.Exactly(4));
        }


        [Fact]
        public void GetBusyDays()
        {
            // Arrange
            List<SecurityDaily> dataFromQuandl = new List<SecurityDaily>()
            {
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-04", Open = (Decimal)762.2, High = (Decimal)762.2, Low = (Decimal)747.54, Close = (Decimal)759.44, Volume = (Decimal)3369068.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-05", Open = (Decimal)764.1, High = (Decimal)769.2, Low = (Decimal)755.65, Close = (Decimal)761.53, Volume = (Decimal)2260795.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-06", Open = (Decimal)745.3, High = (Decimal)749.7, Low = (Decimal)745.0, Close = (Decimal)751.44, Volume = (Decimal)2561354.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-07", Open = (Decimal)755.7, High = (Decimal)758.8, Low = (Decimal)749.0, Close = (Decimal)757.21, Volume = (Decimal)2865324.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-08", Open = (Decimal)760.4, High = (Decimal)762.5, Low = (Decimal)751.0, Close = (Decimal)759.38, Volume = (Decimal)2410301.0 } }
            };

            String ticker = "GOOGL";
            String start = "2016-01-05";
            String end = "2016-01-23";

            // Average Volume is close to 1 of 5 mock days volumes, so we should collect only 4 days
            Decimal averageVolume = 2154683.35m;
            String startMonth = "2016-01-01";
            String startNextMonth = "2016-02-01";

            // Act
            _mockQuandlConsumer.Setup(x => x.GetDailyPrices(ticker, startMonth, startNextMonth))
                .Returns(Task.FromResult(dataFromQuandl));

            var controller = new PricesController(_mockQuandlConsumer.Object);
            var result = controller.GetBusyDays(ticker, start, end, averageVolume);

            // Assert
            _mockQuandlConsumer.Verify(x => x.GetDailyPrices(ticker, startMonth, startNextMonth), Times.Once());
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var busyDaysList = okResult.Value.Should().BeAssignableTo<List<PricesController.DailyVolume>>().Subject;

            busyDaysList.Count().Should().Be(4);
            busyDaysList.First().As<PricesController.DailyVolume>()._date.Should().Be("2016-01-04");
            busyDaysList.First().As<PricesController.DailyVolume>()._volume.Should().Be(3369068.0m);
        }
    }
}
