/*jshint esversion: 6 */
(function () {

    function createDirective(execute) {

        function createListener(el, evt, value, arg) {
            if (!value)
                return null;

            var res = function callBack() {
                execute(value, arg);
            };
            el.addEventListener(evt, res);
            return res;
        }

        function deleteListener(el, evt, callback) {
            if (!!callBack) {
                el.removeEventListener(evt, callBack);
            }
        }

        return {
            bind: function (el, binding) {
                const evt = binding.arg || "click";
                var arg = null;
                el.__callBack = createListener(el, evt, binding.value, arg);
            },
            update: function (el, binding) {
                if (binding.value == binding.oldValue)
                    return;
                const evt = binding.arg || "click";
                deleteListener(el, evt, el.__callBack);
                //var arg = ctx.params.commandarg || null;
                var arg = null;
                el.__callBack = createListener(el, evt, binding.value, arg);
            },
            unbind: function (el, binding) {
                const evt = binding.arg || "click";
                deleteListener(el, evt, el.__callBack);
            }
        };
    }

    var comand = createDirective(function (newValue, arg) {
        newValue.CanExecute(arg);
        if (newValue.CanExecuteValue)
            newValue.Execute(arg);
    });

    var simpleComand = createDirective(function (newValue, arg) {
        newValue.Execute(arg);
    });

    Vue.directive("command", comand);
    Vue.directive("simplecommand", simpleComand);

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

    Vue.__commandMixin = commandMixin;

}());