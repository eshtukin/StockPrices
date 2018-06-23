using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using StockPrices.Consumers;
using StockPrices.Models;

namespace StockPricesUnitTest
{
    public class QuadlConsumerTest
    {
        QuandlConsumer quandl = new QuandlConsumer();

        [Fact]
        public void ParseDailyPrices_Test()
        {
            // Arrange
            String goodResponseString = "{\"datatable\":{\"data\":[" +
                "[\"GOOGL\",\"2016-01-04\",762.2,762.2,747.54,759.44,3369068.0,0.0,1.0,762.2,762.2,747.54,759.44,3369068.0]," +
                "[\"GOOGL\",\"2016-01-05\",764.1,769.2,755.65,761.53,2260795.0,0.0,1.0,764.1,769.2,755.65,761.53,2260795.0]," +
                "[\"GOOGL\",\"2016-01-06\",750.37,765.73,748.0,759.33,2410301.0,0.0,1.0,750.37,765.73,748.0,759.33,2410301.0]]," +
                "\"columns\":[{\"name\":\"ticker\",\"type\":\"String\"},{\"name\":\"date\",\"type\":\"Date\"},{\"name\":\"open\",\"type\":\"BigDecimal(34,12)\"},{\"name\":\"high\",\"type\":\"BigDecimal(34,12)\"},{\"name\":\"low\",\"type\":\"BigDecimal(34,12)\"},{\"name\":\"close\",\"type\":\"BigDecimal(34,12)\"},{\"name\":\"volume\",\"type\":\"BigDecimal(37,15)\"},{\"name\":\"ex-dividend\",\"type\":\"BigDecimal(42,20)\"},{\"name\":\"split_ratio\",\"type\":\"double\"},{\"name\":\"adj_open\",\"type\":\"BigDecimal(50,28)\"},{\"name\":\"adj_high\",\"type\":\"BigDecimal(50,28)\"},{\"name\":\"adj_low\",\"type\":\"BigDecimal(50,28)\"},{\"name\":\"adj_close\",\"type\":\"BigDecimal(50,28)\"},{\"name\":\"adj_volume\",\"type\":\"double\"}]},\"meta\":{\"next_cursor_id\":null}}";

            // Act
            QuandlConsumer quandl = new QuandlConsumer();
            var securityList = quandl.ParseDailyPrices(new System.IO.MemoryStream(Encoding.UTF8.GetBytes(goodResponseString)));

            // Assert
            securityList.Should().BeAssignableTo<IEnumerable<SecurityDaily>>();
            securityList.Count.Should().Be(3);
            securityList[0].As<SecurityDaily>().Ticker.Should().Be("GOOGL");
            securityList[0].As<SecurityDaily>().Date.Should().Be("2016-01-04");
            securityList[0].As<SecurityDaily>().Open.Should().Be((Decimal)762.2);
            securityList[1].As<SecurityDaily>().High.Should().Be((Decimal)769.2);
            securityList[1].As<SecurityDaily>().Low.Should().Be((Decimal)755.65);
            securityList[1].As<SecurityDaily>().Close.Should().Be((Decimal)761.53);
            securityList[2].As<SecurityDaily>().Ticker.Should().Be("GOOGL");
            securityList[2].As<SecurityDaily>().Date.Should().Be("2016-01-06");
            securityList[2].As<SecurityDaily>().Volume.Should().Be((Decimal)2410301.0);
        }
    }
}
