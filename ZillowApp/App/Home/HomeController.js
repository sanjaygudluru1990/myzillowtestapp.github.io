(function (zillowApp) {
    'use strict';
    zillowApp.controller("HomeController", _homeController);
    _homeController.$inject = ["ZillowDataFactory","$timeout","$state"];

    function _homeController(ZillowDataFactory,$timeout,$state) {
        var homevm = this;
        homevm.addressLine = "";
        homevm.properties = [];
        homevm.getPropertyResults = function _processAddress() {
            ZillowDataFactory.GetZillowSearchResults(homevm.addressLine).then(
                function (data) {
                    homevm.properties = data;
                    $timeout(function () {
                        _processPropertyDom(); 
                    }, 10);
                    console.log(data);
                },
                function (error) {
                    console.log(error);
                    $state.go("error", {errObj: error.data});
                });
        }

        //AIzaSyBtD1sVA-YSVva9yW0_GZce2cf9GUxADj8
        //2081 Seminole, Tustin, CA ,92782
        function _processPropertyDom() {
            var element = document.getElementById('popularList');
            var position = 0;
            for (let i = 0; i < element.children.length; i++) {
                let child = element.children[i];

                child.addEventListener("click", function (e) {
                    if (position != 0) { document.getElementById("pos-" + position).style.display = 'none'; }
                    let domNode = this,
                        searchTooltip = document.getElementById("searchTooltip");
                    position = domNode.getAttribute("data-tooltip-pos");

                    for (let k = 0; k < element.children.length; k++) {
                        element.children[k].classList.remove("active");
                    }

                    child.classList.add("active");

                    searchTooltip.style.top = position + 'px';
                    document.getElementById("pos-" + position).style.display = 'block';
                });
            }
        }
    }
})(angular.module("zillowApp"));