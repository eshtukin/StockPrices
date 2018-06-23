# StockPrices
Gather and show stock prices information


Package.
1.	The task implemented using  C# Asp.Net Core 2.0 (backend) / AngularJS (frontend).
2.	Asp.Net Core 2.0 is Microsoft latest framework replacing Web API, which is fully supported only in VS 2017.
3.	This GitHub repository contains VS 2017 StockPrices.sln with 2 projects – StockPrices and StockPricesUnitTest.
4.  Client assets (js, html, css) are in wwwroot folder distributed together with the server. It also includes AngularJS
    in lib/bower_components folder.
                                                                      


Deploy and Run on local Windows machine.
Here are the steps to run the app locally on Windows machine:
1.	Server can be started either from VS, or from local IIS. For the latter you should add a site to your local IIS pointing to the root
    of built StockPrices project, and start it via Application Pools.
2.	To start the client in the Web browser of your choice run command   http://localhost:49254/


Implementation details and Usage.
1.	PricesController has two REST Api GET methods – GetPrices and GetBusyDays
2.	It uses IQuandlConsumer as dependency injection
3.	QuandlConsumer brings stock price data from Quandl WIKI Stock Price API 
4.	StockPricesUnitTests comprises 2 test classes which use xUnit and Moq test frameworks
5.	On Html client users should pick-up a security from the drop-down list, and dates.
    Standard date-picker control is used, which is unfortunately browser-dependent: it has attached calendar in Chrome, 
    selection box in FireFox, and free-text in IE/Edge. 
6.  You can choose dates starting from 2014-01-01.
7.  Any chosen date will include the whole month to queries and calculations. For example, if 2016-02-15 was chosen as Start Date, 
    and 2016-06-03 was chosen as End Date, the time period for Quandl queries will be from 2016-02-01 till 2016-06-30.
