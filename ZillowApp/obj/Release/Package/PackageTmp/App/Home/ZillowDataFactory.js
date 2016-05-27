(function (zillowApp) {
    'use strict';
    zillowApp.factory("ZillowDataFactory", _zillowDataFactory);
    _zillowDataFactory.$inject = ["$http","$q"];

    function _zillowDataFactory($http,$q) {
        var factoryObj = {
            GetZillowSearchResults : _getZillowSearchResults
        };
        return factoryObj;

        function _getZillowSearchResults(address) {
            var httpConfigObj = {
                method: "GET",
                url: "api/ZillowSearch/Search" + '/' + address
            };
            return $http(httpConfigObj).then(function (response) {
                return response.data;
            }, function (error) {
                return $q.reject(error);
            });
        }
    }
})(angular.module("zillowApp"));