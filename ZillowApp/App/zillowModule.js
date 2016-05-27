(function () {
    'use strict';
    angular.module("zillowApp", ['ui.router']).config(["$stateProvider", "$urlRouterProvider", function ($stateProvider, $urlRouterProvider) {

        $urlRouterProvider.otherwise('/home');

        $stateProvider
            .state('home', {
                url: '/home',
                templateUrl: 'App/Home/home.html',
                controller: 'HomeController as homevm'
            })
            .state('error', {
                url: '/error',
                templateUrl: 'App/Home/error.html',
                controller: 'ExceptionController as exceptionvm',
                params: {errObj: null}
            });
    }]);
})();