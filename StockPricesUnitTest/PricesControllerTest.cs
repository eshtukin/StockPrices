
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
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-04", Open = (decimal)762.2, High = (decimal)762.2, Low = (decimal)747.54, Close = (decimal)759.44, Volume = (decimal)3369068.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-05", Open = (decimal)764.1, High = (decimal)769.2, Low = (decimal)755.65, Close = (decimal)761.53, Volume = (decimal)2260795.0 } },
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-06", Open = (decimal)750.37, High = (decimal)765.73, Low = (decimal)748.0, Close = (decimal)759.33, Volume = (decimal)2410301.0 } }
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
            securitySummary._monthlyAverages.First().As<PricesController.MonthlyAverage>()._open.Should().Be((decimal)758.89);
            securitySummary._monthlyAverages.First().As<PricesController.MonthlyAverage>()._close.Should().Be((decimal)760.1);
        }

        [Fact]
        public void GetPrices_FourMonths()
        {
            // Arrange
            List<SecurityDaily> dataFromQuandl = new List<SecurityDaily>()
            {
                { new SecurityDaily { Ticker = "GOOGL", Date = "2016-01-04", Open = (decimal)762.2, High = (decimal)762.2, Low = (decimal)747.54, Close = (decimal)759.44, Volume = (decimal)3369068.0 } }
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

    }
}
