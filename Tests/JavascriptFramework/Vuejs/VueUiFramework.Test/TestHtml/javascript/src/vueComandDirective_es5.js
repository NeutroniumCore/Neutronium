/*jshint esversion: 6 */
"use strict";

(function () {
    if (typeof Object.assign != 'function') {
        Object.assign = function (target) {
            'use strict';
            if (target == null) {
                throw new TypeError('Cannot convert undefined or null to object');
            }

            target = Object(target);
            for (var index = 1; index < arguments.length; index++) {
                var source = arguments[index];
                if (source != null) {
                    for (var key in source) {
                        if (Object.prototype.hasOwnProperty.call(source, key)) {
                            target[key] = source[key];
                        }
                    }
                }
            }
            return target;
        };
    }

    var base = {
        params: ["commandarg"],

        update: function update(newValue, oldValue) {
            var evt = this.arg || "click";
            if (!!oldValue) {
                this.el.removeEventListener(evt, this.callBack);
            }
            if (!!newValue) {
                var ctx = this;
                this.callBack = function callBack() {
                    var arg = ctx.params.comandarg || null;
                    ctx.execute(newValue, arg);
                };
                this.el.addEventListener(evt, this.callBack);
            }
        },
        unbind: function unbind() {
            var evt = this.arg || "click";
            if (!!this.callBack) {
                this.el.removeEventListener(evt, this.callBack);
            }
        }
    };

    var comand = Object.assign({
        execute: function execute(newValue, arg) {
            newValue.CanExecute(arg);
            if (newValue.CanExecuteValue) newValue.Execute(arg);
        }
    }, base);

    var simpleComand = Object.assign({
        execute: function execute(newValue, arg) {
            newValue.Execute(arg);
        }
    }, base);

    Vue.directive("command", comand);
    Vue.directive("simpleCommand", simpleComand);

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
        ready: function ready() {
            var _this = this;

            setTimeout(function () {
                if (!!_this.arg) _this.computeCanExecute();
            });
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

    Vue.__commandMixin = commandMixin;
})();

