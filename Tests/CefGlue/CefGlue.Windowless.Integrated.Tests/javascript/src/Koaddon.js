(function () {
    ko.register = function (vm) {
        vm.completeName = ko.computed(function () {
            return vm.Name() + " " + vm.LastName();
        });
    };
})();