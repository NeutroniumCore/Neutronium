(function() {

    var visited = new Map();

    function visitObject(vm, visit, visitArray) {
        "use strict";
        if (!vm || !!visited.has(vm._MappedId))
            return;

        if (typeof vm !== "object")
            return;

        visited.set(vm._MappedId, vm);

        if (Array.isArray(vm)) {
            visitArray(vm);
            vm.forEach((value) => visitObject(value, visit, visitArray));
            return;
        }

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

    function Listener(listener, change) {
        this.listen = function() {
            this.subscriber = listener();
        }

        this.silence = function(value) {
            this.subscriber();
            change(value);
            this.listen();
        }
    }

    function collectionListener(object, observer) {
        return function(changes) {
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
        var changelistener = collectionListener(array, observer);
        var listener = array.subscribe(changelistener);
        array.silentSplice = function() {
            listener();
            var res = array.splice.apply(array, arguments);
            listener = array.subscribe(changelistener);
            return res;
        };
    }

    function onPropertyChange(observer, prop, father) {
        var blocked = false;

        return function(newVal, oldVal) {
            if (blocked) {
                blocked = false;
                return;
            }

            if (newVal === oldVal)
                return;

            if (Array.isArray(newVal)) {
                var args = [0, oldVal.length].concat(newVal);
                oldVal.splice.apply(oldVal, args);
                blocked = true;
                father[prop] = oldVal;
                return;
            }

            observer.TrackChanges(father, prop, newVal);
        };
    }

    var inject = function(vm, observer) {
        if (!vueVm)
            return vm;

        visitObject(vm, (father, prop) => {
            if (!father.__silenter)
                Object.defineProperty(father, '__silenter', {value: {}});
            var silenter = father.__silenter;
            var listenerfunction = onPropertyChange(observer, prop, father);
            var newListener = new Listener(() => vueVm.$watch(() => father[prop], listenerfunction), (value) => father[prop] = value);
            newListener.listen();
            silenter[prop] = newListener;
        }, array => updateArray(array, observer));
        return vm;
    };

    var fufillOnReady;
    var ready = new Promise(function(fullfill) {
        fufillOnReady = fullfill;
    });

    var helper = {
        enumMixin: {
            methods: {
                enumImage: function(value, enumImages) {
                    if (!value instanceof Enum)
                        return null;

                    var images = enumImages || this.enumImages;
                    if (!images)
                        return null;

                    var ec = images[value.type];
                    return ec ? ec[value.name] : null;
                }
            }
        },
        inject: inject,
        register: function(vm, observer) {
            var mixin = Vue._vmMixin;
            if (!!mixin && !Array.isArray(mixin))
                mixin = [mixin];

            vueVm = new Vue({
                el: "#main",
                mixins: mixin,
                data: vm,
                ready: function() {
                    fufillOnReady(null);
                }
            });

            window.vm = vueVm;

            return inject(vm, observer);
        },
        ready: ready
    };

    window.glueHelper = helper;
}());