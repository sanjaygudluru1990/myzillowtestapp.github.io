(function (zillowApp) {
    'use strict';
    zillowApp.directive("mapDirective", _mapDirective);
    _mapDirective.$inject = ["$timeout"];

    function _mapDirective($timeout) {
        return {
            restrict: 'AE',
            scope: {
                lat: '=',
                lng: '='
            },
            link: function (scope, telement, attrs) {
                $timeout(function () {
                    var map = new google.maps.Map(document.getElementById(attrs.id), {
                        center: { lat: scope.lat, lng: scope.lng },
                        zoom: 8
                    });
                }, 100)
            }
        };
    }
})(angular.module("zillowApp"));