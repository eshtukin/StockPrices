'use strict';

angular.module('PricesController', [])


.controller('PricesCtrl', ['$scope', '$location', '$http', '$window', 
    function ($scope, $location, $http, $window) {

        angular.extend($scope, {
        getPrices : getPrices,
        maxDailyProfit: maxDailyProfit,
        canFetch: canFetch
    });

    $scope.dataLoading = true;
    $scope.securityList = ["COF", "GOOGL", "MSFT"];
    $scope.startDate = null;
    $scope.endDate = null;
    $scope.enableMaxDailyProfit = false;
    $scope.showMonthlyAveragesTable = false;
    $scope.showMaxDailyProfit = false;

        //var loserDays {

        //}
    function canFetch() {
        return $scope.security !== "" && $scope.startDate !== null && $scope.endDate !== null;
    }

    function getPrices() {

        $scope.dataLoading = true;
        $scope.showMaxDailyProfit = false;
        $scope.showMonthlyAveragesTable = false;
        $scope.securitySummary = {};

        var startDate = formatDate($scope.startDate);
        var endDate = formatDate($scope.endDate)

        $http({
          method: 'GET', url: $location.protocol() + '://' + $location.host() + ':' + $location.port() + '/api/prices/' +  $scope.security + '/' + startDate + '/' + endDate })
            .then(function (response) {
                $scope.showMonthlyAveragesTable = true;
                $scope.securitySummary = response.data;
                $scope.enableMaxDailyProfit = true;
            }, function (response) {
                var message = 'No data found for security ' + $scope.security + ' , Start Date ' + startDate + ' End Date ' + endDate;
                alert(message);
                console.log(message);
            }).finally(function () {
                $scope.dataLoading = false;
            });;
    }

    function formatDate(date) {
            return date.getFullYear() + '-' + (date.getMonth() + 1).toString()  + '-' + date.getDate();
    }

    function maxDailyProfit() {
        $scope.showMaxDailyProfit = true;
    }





}]);