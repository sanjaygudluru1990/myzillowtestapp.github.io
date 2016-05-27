(function (zillowApp) {
    'use strict';
    zillowApp.controller("ExceptionController", _exceptionController);
    _exceptionController.$inject = ["$stateParams"];

    function _exceptionController($stateParams) {
        var exceptionvm = this;
        exceptionvm.errObj = $stateParams.errObj;
    }
})(angular.module("zillowApp"));