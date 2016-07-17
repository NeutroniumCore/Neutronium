(function () {

    var visited = {};

    function visitObject(vm, visit, visitArray) {
        "use strict";
        if (!vm || !!visited[vm._MappedId])
            return;

        if (typeof vm !== "object")
            return;

        visited[vm._MappedId] = vm;

        if (Array.isArray(vm)) {
            visitArray(vm);
            vm.forEach(value =>  visitObject(value, visit, visitArray));
            return;
        }

        visited[vm._MappedId] = vm;

        for (var property in vm) {
            if (!vm.hasOwnProperty(property))
                continue;

            var value = vm[property];
            if (typeof value === "function")
                continue;

            visit(vm, property);
            visitObject(value, visit, visitArray);
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

    function collectionListener(object, observer) {
        return function (changes) {
            var arg_value = [], arg_status = [], arg_index = [];
            var length = changes.length;
            for (var i = 0; i < length; i++) {
                arg_value.push(changes[i].value);
                arg_status.push(changes[i].status);
                arg_index.push(changes[i].index);
            }
            observer.TrackCollectionChanges(object, arg_value, arg_status, arg_index);
        };
    }

    function updateArray(array, observer) {
        //var listener = array.subscribe(change => console.log(change));
        var listener = array.subscribe(change => collectionListener(array, observer));
        array.silentSplice = function () {
            listener();
            var res = array.splice.apply(array, arguments);
            listener = array.subscribe(change => collectionListener(array, observer));
            return res;
        };
    }

    var inject = function (vm, observer) {
        if (!vueVm)
            return vm;

        visitObject(vm, (father, prop) => {
            father.__silenter = father.__silenter || {};
            var silenter = father.__silenter;
            newListener = new Listener(() => vueVm.$watch(() => father[prop], function (newVal) {
                            observer.TrackChanges(father, prop, newVal);
                        }), (value)=> father[prop] =value);               
            newListener.listen();
            silenter[prop] = newListener;
        }, array => updateArray(array, observer));
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