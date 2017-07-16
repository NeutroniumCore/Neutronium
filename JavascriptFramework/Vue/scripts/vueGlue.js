(function () {
    console.log("VueGlue loaded");

    const silenterProto = {
        silentChange: function (propertyName, value) {
            this[propertyName].silence(value)
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

        for (var property in vm) {
            var value = vm[property];
            if (typeof value === "function")
                continue;

            visit(vm, property);
            visitObject(value, visit, visitArray);
        }
    }

    var vueVm = null;

    const ListenerProp = {
        init: function (listener, change) {
            this.listener = listener;
            this.change = change;
            return this;
        },
        listen: function () {
            this.subscriber = this.listener();
            return this;
        },
        silence: function (value) {
            this.subscriber();
            this.change(value);
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
            father.__silenter || Object.defineProperty(father, '__silenter', { value: Object.create(silenterProto) });
            var silenter = father.__silenter;
            var listenerfunction = onPropertyChange(observer, prop, father);
            var newListener = Object.create(ListenerProp)
                                    .init(() => vueVm.$watch(() => father[prop], listenerfunction), (value) => father[prop] = value)
                                    .listen();
            silenter[prop] = newListener;
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
        enumMixin: enumMixin,
        openMixin: openMixin,
        closeMixin: closeMixin,
        promiseMixin: promiseMixin,
        commandMixin: commandMixin,
        inject: inject,
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