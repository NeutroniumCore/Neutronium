(function () {
    function visitOnlyMethod(vm, visit){
        for (var property in vm) {
            var value = vm[property]
            if (vm.hasOwnProperty(property) && typeof value !== "function") {
                visit(property, value)
            }
        }
    }

    var helper = {
        register: function (vm, observer) {
            var vueVm = new Vue({
                el: '#main',
                data: vm
            });

            visitOnlyMethod(vm, (prop, _) => {
                console.log(prop)
                vueVm.$watch(prop, function (newVal, oldVal) {
                    console.log(prop, newVal, oldVal)
                    observer.TrackChanges(vm, prop, newVal);
                })
            });

            return vm;
        }
    };

    window.glueHelper = helper;
}())