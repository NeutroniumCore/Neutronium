(function () {
    console.log("VueGlue loaded");

    const silenterProperty = '__silenter';

    function silentChange(father, propertyName, value) {
        const silenter = father[silenterProperty];
        if (silenter) {
            silenter.silentChange(propertyName, value);
            return;
        }
        father[propertyName] = value;
    }

    function silentChangeAndInject(father, propertyName, value, observer) {
        silentChange(father, propertyName, value);
        inject(value, observer);
    }

    const silenterProto = {
        init (father) {
            this.father = father;
            this.listeners = {}
            return this;
        },
        create(propertyName, callback) {
            const listener = { callback };
            this.updateListener(listener, propertyName);
            this.listeners[propertyName] = listener;
        },
        updateListener(listener, propertyName) {
            listener.watch = vueVm.$watch(() => this.father[propertyName], listener.callback)
        },
        silentChange: function (propertyName, value) {
            const listener = this.listeners[propertyName];
            listener.watch();
            this.father[propertyName] = value;
            this.updateListener(listener, propertyName);
        }
    };

    var visited = new Map();
    visited.set(undefined, null);

    function visitObject(vm, visit, visitArray) {
        "use strict";
        if (!vm || visited.has(vm._MappedId))
            return;

        visited.set(vm._MappedId, vm);

        if (Array.isArray(vm)) {
            visitArray(vm);
            const arrayCount = vm.length;
            for (var i = 0; i < arrayCount; i++) {
                const value = vm[i];
                visitObject(value, visit, visitArray);
            }
            return;
        }

        const needVisitSelf = !vm.__readonly__;

        for (var property in vm) {
            var value = vm[property];
            if (typeof value === "function")
                continue;

            if (needVisitSelf) {
                visit(vm, property);
            }
            visitObject(value, visit, visitArray);
        }
    }

    var vueVm = null;

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
        var changelistener = collectionListener(array, observer);
        var listener = array.subscribe(changelistener);
        array.silentSplice = function () {
            listener();
            var res = array.splice.apply(array, arguments);
            listener = array.subscribe(changelistener);
            return res;
        };
    }

    function onPropertyChange(observer, prop, father) {
        var blocked = false;

        return function (newVal, oldVal) {
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

    var inject = function (vm, observer) {
        if (!vueVm)
            return vm

        visitObject(vm, (father, prop) => {
            father.__silenter || Object.defineProperty(father, silenterProperty, { value: Object.create(silenterProto).init(father) });
            var silenter = father.__silenter;
            silenter.create(prop, onPropertyChange(observer, prop, father));
        }, array => updateArray(array, observer));
        return vm
    };

    var fufillOnReady;
    var ready = new Promise(function (fullfill) {
        fufillOnReady = fullfill;
    });

    var enumMixin = {
        methods: {
            enumImage: function (value, enumImages) {
                if (!value instanceof Enum)
                    return null;

                var images = enumImages || this.enumImages;
                if (!images)
                    return null;

                var ec = images[value.type];
                return ec ? ec[value.name] : null;
            }
        }
    };

    function listenEventAndDo(options) {
        const status = options.status;
        const command = options.command;
        const inform = options.inform;
        const callBack = options.callBack;

        this.$watch("$data.__window__.State", function (newVal) {
            if (newVal.name == status) {
                const cb = () => this.$data.__window__[command].Execute();
                callBack(cb);
            }
        });

        this.$nextTick(function () {
            this.$data.__window__[inform] = true;
        })
    };

    const VueAdapter = Vue.adapter

    var openMixin = VueAdapter.addOnReady({},
        function () {
            listenEventAndDo.call(this, { status: "Opened", command: "EndOpen", inform: "IsListeningOpen", callBack: (cb) => this.onOpen(cb) });
        });

    var closeMixin = VueAdapter.addOnReady({},
      function () {
          listenEventAndDo.call(this, { status: "Closing", command: "CloseReady", inform: "IsListeningClose", callBack: (cb) => this.onClose(cb) });
      });

    var promiseMixin = {
        methods: {
            asPromise: function asPromise(callback) {
                return function asPromise(argument) {
                    return new Promise(function (fullfill, reject) {
                        var res = { fullfill: function (res) { fullfill(res); }, reject: function (err) { reject(new Error(err)); } };
                        callback.Execute(argument, res);
                    });
                }
            }
        }
    };

    var commandMixin = {
        props: {
            command: {
                type: Object,
                default: null
            },
            arg: {
                type: Object,
                default: null
            }
        },
        computed: {
            canExecute: function () {
                if (this.command === null)
                    return false;
                return this.command.CanExecuteValue;
            }
        },
        watch: {
            'command.CanExecuteCount': function () {
                this.computeCanExecute();
            },
            arg: function () {
                this.computeCanExecute();
            }
        },
        methods: {
            computeCanExecute: function () {
                if (this.command !== null)
                    this.command.CanExecute(this.arg);
            },
            execute: function () {
                if (this.canExecute) {
                    var beforeCb = this.beforeCommand;
                    if (!!beforeCb)
                        beforeCb();
                    this.command.Execute(this.arg);
                }
            }
        }
    };

    commandMixin = Vue.adapter.addOnReady(commandMixin, function () {
        var ctx = this;
        setTimeout(() => {
            if (!!ctx.arg)
                ctx.computeCanExecute();
        });
    });

    var helper = {
        enumMixin,
        openMixin,
        closeMixin,
        promiseMixin,
        commandMixin,
        silentChange,
        inject,
        silentChangeAndInject,
        register: function (vm, observer) {
            console.log("VueGlue register");
            var mixin = Vue._vmMixin;
            if (!!mixin && !Array.isArray(mixin))
                mixin = [mixin];

            var vueOption = VueAdapter.addOnReady({
                el: "#main",
                mixins: mixin,
                data: vm
            },
                function () {
                    fufillOnReady(null);
                });

            vueVm = new Vue(vueOption);

            window.vm = vueVm;

            return inject(vm, observer);
        },
        ready: ready
    };

    window.glueHelper = helper;
}());