(function () {

    ko.register = function (vm) {
        vm.factoryresult = ko.observable(null);
        vm.error = ko.observable(null);
        vm.status = ko.observable(0);

        vm.result = function (res) {
            alert(res.LastName());
            vm.factoryresult(res);
        };

        vm.error = function (err) {
            vm.error(err);
            console.log(err);
        };
    }
})();