"use strict";

(function () {
    console.log("VueGlue loaded");

    var silenterProperty = '__silenter';
    var vueVm = null;

    function silentChange(father, propertyName, value) {
        setTimeout(function () {
            return silentChangeSync(father, propertyName, value);
        }, 0);
    }

    function silentChangeSync(father, propertyName, value) {
        var silenter = father[silenterProperty];
        if (silenter) {
            silentChangeElement(silenter, propertyName, value);
            return;
        }
        father[propertyName] = value;
    }

    function silentChangeAndInject(father, propertyName, value, observer) {
        setTimeout(function () {
            return silentChangeAndInjectSync(father, propertyName, value, observer);
        }, 0);
    }

    function silentChangeAndInjectSync(father, propertyName, value, observer) {
        silentChangeSync(father, propertyName, value);
        inject(value, observer);
    }

    function disposeSilenters() {
        setTimeout(disposeSilentersSync.bind(null, Array.from(arguments)), 0);
    }

    function disposeSilentersSync(args) {
        var arrayCount = args.length;
        for (var i = 0; i < arrayCount; i++) {
            var father = args[i];
            var silenter = father[silenterProperty];
            if (!silenter) continue;

            disposeElement(silenter);
            delete father[silenterProperty];
        }
    }

    function silentChangeElement(element, propertyName, value) {
        var listener = element.listeners[propertyName];
        listener.watch();
        element.father[propertyName] = value;
        updateListenerElement(element, listener, propertyName);
    }

    function disposeElement(element) {
        var listeners = element.listeners;
        for (var property in listeners) {
            var listener = listeners[property];
            listener.watch();
        }
        element.listeners = {};
    }

    function updateListenerElement(element, listener, propertyName) {
        listener.watch = vueVm.$watch(function () {
            return element.father[propertyName];
        }, listener.callback);
    }

    function createElement(element, propertyName, callback) {
        var listener = { callback: callback };
        updateListenerElement(element, listener, propertyName);
        element.listeners[propertyName] = listener;
    }

    function Silenter(father) {
        this.father = father;
        this.listeners = {};
    }

    var visited = new Map();
    visited.set(undefined, null);

    function visitObject(vm, visit, visitArray) {
        "use strict";
        if (!vm || visited.has(vm._MappedId)) return;

        visited.set(vm._MappedId, vm);

        if (Array.isArray(vm)) {
            visitArray(vm);
            var arrayCount = vm.length;
            for (var i = 0; i < arrayCount; i++) {
                var _value = vm[i];
                visitObject(_value, visit, visitArray);
            }
            return;
        }

        var needVisitSelf = !vm.__readonly__;

        for (var property in vm) {
            var value = vm[property];
            if (typeof value === "function") continue;

            if (needVisitSelf) {
                visit(vm, property);
            }
            visitObject(value, visit, visitArray);
        }
    }

    function collectionListener(object, observer) {
        return function (changes) {
            var arg_value = [],
                arg_status = [],
                arg_index = [];
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

            if (newVal === oldVal) return;

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

    var inject = function inject(vm, observer) {
        if (!vueVm) return vm;

        visitObject(vm, function (father, prop) {
            father[silenterProperty] || Object.defineProperty(father, silenterProperty, { value: new Silenter(father), configurable: true });
            var silenter = father[silenterProperty];
            createElement(silenter, prop, onPropertyChange(observer, prop, father));
        }, function (array) {
            return updateArray(array, observer);
        });
        return vm;
    };

    var fufillOnReady;
    var ready = new Promise(function (fullfill) {
        fufillOnReady = fullfill;
    });

    var enumMixin = {
        methods: {
            enumImage: function enumImage(value, enumImages) {
                if (!value instanceof Enum) return null;

                var images = enumImages || this.enumImages;
                if (!images) return null;

                var ec = images[value.type];
                return ec ? ec[value.name] : null;
            }
        }
    };

    function listenEventAndDo(options) {
        var status = options.status;
        var command = options.command;
        var inform = options.inform;
        var callBack = options.callBack;

        this.$watch("$data.__window__.State", function (newVal) {
            var _this = this;

            if (newVal.name == status) {
                var cb = function cb() {
                    return _this.$data.__window__[command].Execute();
                };
                callBack(cb);
            }
        });

        this.$nextTick(function () {
            this.$data.__window__[inform] = true;
        });
    };

    var VueAdapter = Vue.adapter;

    var openMixin = VueAdapter.addOnReady({}, function () {
        var _this2 = this;

        listenEventAndDo.call(this, { status: "Opened", command: "EndOpen", inform: "IsListeningOpen", callBack: function callBack(cb) {
                return _this2.onOpen(cb);
            } });
    });

    var closeMixin = VueAdapter.addOnReady({}, function () {
        var _this3 = this;

        listenEventAndDo.call(this, { status: "Closing", command: "CloseReady", inform: "IsListeningClose", callBack: function callBack(cb) {
                return _this3.onClose(cb);
            } });
    });

    var promiseMixin = {
        methods: {
            asPromise: function asPromise(callback) {
                return function asPromise(argument) {
                    return new Promise(function (_fullfill, _reject) {
                        var res = { fullfill: function fullfill(res) {
                                _fullfill(res);
                            }, reject: function reject(err) {
                                _reject(new Error(err));
                            } };
                        callback.Execute(argument, res);
                    });
                };
            }
        }
    };

    var commandMixin = {
        props: {
            command: {
                type: Object,
                "default": null
            },
            arg: {
                type: Object,
                "default": null
            }
        },
        computed: {
            canExecute: function canExecute() {
                if (this.command === null) return false;
                return this.command.CanExecuteValue;
            }
        },
        watch: {
            'command.CanExecuteCount': function commandCanExecuteCount() {
                this.computeCanExecute();
            },
            arg: function arg() {
                this.computeCanExecute();
            }
        },
        methods: {
            computeCanExecute: function computeCanExecute() {
                if (this.command !== null) this.command.CanExecute(this.arg);
            },
            execute: function execute() {
                if (this.canExecute) {
                    var beforeCb = this.beforeCommand;
                    if (!!beforeCb) beforeCb();
                    this.command.Execute(this.arg);
                }
            }
        }
    };

    commandMixin = Vue.adapter.addOnReady(commandMixin, function () {
        var ctx = this;
        setTimeout(function () {
            if (!!ctx.arg) ctx.computeCanExecute();
        });
    });

    var helper = {
        enumMixin: enumMixin,
        openMixin: openMixin,
        closeMixin: closeMixin,
        promiseMixin: promiseMixin,
        commandMixin: commandMixin,
        silentChange: silentChange,
        inject: inject,
        silentChangeAndInject: silentChangeAndInject,
        disposeSilenters: disposeSilenters,
        register: function register(vm, observer) {
            console.log("VueGlue register");
            var mixin = Vue._vmMixin;
            if (!!mixin && !Array.isArray(mixin)) mixin = [mixin];

            var vueOption = VueAdapter.addOnReady({
                el: "#main",
                mixins: mixin,
                data: vm
            }, function () {
                fufillOnReady(null);
            });

            vueVm = new Vue(vueOption);

            window.vm = vueVm;

            return inject(vm, observer);
        },
        ready: ready
    };

    window.glueHelper = helper;
})();

