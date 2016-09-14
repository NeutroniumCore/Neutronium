(function () {

    ko.register = function (vm) {
        vm.factotyresult = ko.observable(null);
        vm.lastError = ko.observable(null);

        vm.result = function (res) {
            alert(res.LastName());
            vm.factotyresult(res);
        };

        vm.error = function (err) {
            vm.lastError(err);
            console.log(err);
            alert(err);
        };

        vm.click = function () {
            executeAsPromise(vm, 'CreateObject',vm.Name()).then(
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