'use strict';

angular.module('PricesController', [])


.controller('PricesCtrl', ['$scope', '$location', '$http', '$window', 
    function ($scope, $location, $http, $window) {

    angular.extend($scope, {
        getPrices: getPrices,
        maxDailyProfit: maxDailyProfit,
        getBusyDays: getBusyDays,
        getBiggestLoser: getBiggestLoser,
        canFetch: canFetch
    });

    $scope.dataLoading = false;
    $scope.securityList = ["COF", "GOOGL", "MSFT"];
    $scope.startDate = null;
    $scope.endDate = null;
    $scope.enableRestOfButtons = false;
    $scope.showMonthlyAveragesTable = false;
    $scope.showMaxDailyProfit = false;
    $scope.biggestLoser = {
        ticker: {},
        daysOfLoss: -1
    }; 

    function canFetch() {
//        return $scope.security !== "" && $scope.startDate !== null && $scope.endDate !== null;
        if ($scope.security === "" || $scope.startDate === null || $scope.endDate === null) {
            $scope.enableRestOfButtons = false;
            return false;
        }
        return true;
    }

    function getPrices() {
        $scope.dataLoading = true;
        $scope.showMaxDailyProfit = false;
        $scope.showMonthlyAveragesTable = false;
        $scope.showBusyDaysList = false;
        $scope.showBiggestLoser = false;
        $scope.securitySummary = {};
        $scope.busyDaysList = {};

        var startDate = formatDate($scope.startDate);
        var endDate = formatDate($scope.endDate)

        $http({
            method: 'GET', url: $location.protocol() + '://' + $location.host() + ':' + $location.port()
                                            + '/api/Prices/GetPrices/' + $scope.security + '/' + startDate + '/' + endDate
        }).then(function (response) {
            $scope.showMonthlyAveragesTable = true;
            $scope.securitySummary = response.data;
            if ($scope.securitySummary._daysOfLoss > $scope.biggestLoser.daysOfLoss) {
                $scope.biggestLoser.ticker = $scope.security;
                $scope.biggestLoser.daysOfLoss = $scope.securitySummary._daysOfLoss;
            }
            $scope.enableRestOfButtons = true;
        }, function (response) {
            var message = 'No data found for security ' + $scope.security + ' , Start Date ' + startDate + ' End Date ' + endDate;
            alert(message);
            console.log(message);
        }).finally(function () {
            $scope.dataLoading = false;
        });
    }

    function formatDate(date) {
        return date.getFullYear() + '-' + (date.getMonth() + 1).toString()  + '-' + date.getDate();
    }

    function maxDailyProfit() {
        $scope.showMaxDailyProfit = true;
    }

    function getBusyDays() {
        $scope.dataLoading = true;
        $scope.showBusyDaysList = false;
        $scope.busyDaysList = {};

        var startDate = formatDate($scope.startDate);
        var endDate = formatDate($scope.endDate)

        $http({
            method: 'GET', url: $location.protocol() + '://' + $location.host() + ':' + $location.port() + '/api/Prices/GetBusyDays/' 
                                        + $scope.security + '/' + startDate + '/' + endDate + '/' + $scope.securitySummary._averageVolume
        }).then(function (response) {
            $scope.showBusyDaysList = true;
            $scope.busyDaysList = response.data;
        }, function (response) {
            var message = 'Something went wrong';
            alert(message);
            console.log(message);
        }).finally(function () {
            $scope.dataLoading = false;
        });
    }

    function getBiggestLoser() {
        $scope.showBiggestLoser = true;
    }

}]);