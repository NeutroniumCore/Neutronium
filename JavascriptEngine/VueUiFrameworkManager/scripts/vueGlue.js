(function () {
    function onlyMethod(vm){
        var method = {};

        for (var property in vm) {
            if (vm.hasOwnProperty(property) && typeof vm[property] === "function") {
                method[property] = vm[property];
            }
        }
        return method;
    }

    var helper = {
        register: function (vm) {
            new Vue({
                el: '#main',
                data: vm,
                methods: onlyMethod(vm)
            });

            return vm;
        }
    };

    window.glueHelper = helper;
}())