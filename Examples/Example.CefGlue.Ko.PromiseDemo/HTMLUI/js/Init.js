(function () {

    ko.register = function (vm) {
        vm.factotyresult = ko.observable(null);
        vm.lastError = ko.observable(null);

        vm.ViewModel().result = function (res) {
            alert(res.LastName());
            vm.factotyresult(res);
        };

        vm.ViewModel().error = function (err) {
            vm.lastError(err);
            console.log(err);
            alert(err);
        };

        vm.ViewModel().click = function () {
            executeAsPromise(vm.ViewModel(), 'CreateObject', vm.ViewModel().Name()).then(
                function (res) {
                    alert(res.LastName());
                    vm.factotyresult(res);
                }
            ).catch(function(reason) {
                console.log(reason);
                alert(reason);
            });
        };
    };
})()