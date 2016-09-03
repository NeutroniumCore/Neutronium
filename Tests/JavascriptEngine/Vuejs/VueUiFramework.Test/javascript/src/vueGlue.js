(function () {

    var visited = {};

    function visitOnlyMethod(vm, visit) {
        "use strict";
        if (!vm || !!visited[vm._MappedId])
            return;

        visited[vm._MappedId] = vm;

        for (var property in vm) {
            if (!vm.hasOwnProperty(property))
                continue;

            var value = vm[property], type = typeof value;
            if (type === "function")
                continue;

            visit(vm, property);
            if (Array.isArray(value)) {
                for (let i = 0; i < value.length; i++) {
                    visitOnlyMethod(value[i], visit);
                }
            }

            if (type === "object") {
                visitOnlyMethod(value, visit);
            }
        }
    }

    var vueVm = null;

    function Listener(listener, change){
        this.listen = function(){
            this.subscriber = listener();
        }

        this.silence = function(value){
            this.subscriber();
            change(value);
            this.listen();
        }
    }

    var inject = function (vm, observer) {
        if (!vueVm)
            return vm;

        visitOnlyMethod(vm, (father, prop) => {
            father.__silenter = father.__silenter || {};
            var silenter = father.__silenter;
            newListener = new Listener(() => vueVm.$watch(() => father[prop], function (newVal) {
                            observer.TrackChanges(father, prop, newVal);
                        }), (value)=> father[prop] =value);               
            newListener.listen();
            silenter[prop] = newListener;
        });
        return vm;
    };

    var helper = {
        inject: inject,
        register: function (vm, observer) {
            vueVm = new Vue({
                el: "#main",
                data: vm
            });

            window.vm = vueVm;

            return inject(vm, observer);
        }
    };

    window.glueHelper = helper;
}())