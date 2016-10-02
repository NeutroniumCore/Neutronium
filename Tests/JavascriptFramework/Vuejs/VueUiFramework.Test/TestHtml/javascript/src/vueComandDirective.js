/*jshint esversion: 6 */
(function () {
    const base = {
        params: ["commandarg"],

        update: function (newValue, oldValue) {
            const evt = this.arg || "click";
            if (!!oldValue) {
                this.el.removeEventListener(evt, this.callBack);
            }
            if (!!newValue) {
                var ctx = this;
                this.callBack = function callBack() {
                    var arg = ctx.params.commandarg || null;
                    ctx.execute(newValue, arg);
                };
                this.el.addEventListener(evt, this.callBack);
            }
        },
        unbind: function () {
            const evt = this.arg || "click";
            if (!!this.callBack) {
                this.el.removeEventListener(evt, this.callBack);
            }
        }
    };

    var comand = Object.assign({
        execute: function (newValue, arg) {
            newValue.CanExecute(arg);
            if (newValue.CanExecuteValue)
                newValue.Execute(arg);
        }
    }, base);

    var simpleComand = Object.assign({
        execute: function (newValue, arg) {
            newValue.Execute(arg);
        }
    }, base);

    Vue.directive("command", comand);
    Vue.directive("simpleCommand", simpleComand);

    var commandMixin = {
        props: {
            command:{
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
        ready: function () {
            setTimeout(() => {
                if (!!this.arg)
                    this.computeCanExecute();
            });
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

    Vue.__commandMixin = commandMixin;

}());