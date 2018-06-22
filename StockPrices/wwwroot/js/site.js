angular.module('myApp', [
    'PricesController',
    'ngRoute'
])

    .config(['$routeProvider', function ($routeProvider) {
        $routeProvider.when('', {
            templateUrl: 'index.html',
            controller: 'PricesCtrl'
        }).otherwise({
            redirectTo: ''
        });
    }]);
