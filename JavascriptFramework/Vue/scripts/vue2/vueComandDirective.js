/*jshint esversion: 6 */
(function () {

    function createDirective(execute) {

        function createListener(el, evt, binding) {
            if (!binding.value)
                return null;

            var command, arg = null;
            if (!!binding.value.arg) {
                command = binding.value.command;
                arg = binding.value.arg;
            } else {
                command = binding.value;
            }

            var res = function callBack() {
                execute(command, arg);
            };
            el.addEventListener(evt, res);
            return res;
        }

        function deleteListener(el, evt, callback) {
            if (!!callback) {
                el.removeEventListener(evt, callback);
            }
        }

        function getEvent(binding) {
            return binding.arg || "click";
        }

        return {
            bind: function (el, binding) {
                const evt = getEvent(binding);
                el.__callBack = createListener(el, evt, binding);
            },
            update: function (el, binding) {
                const evt = getEvent(binding);
                deleteListener(el, evt, el.__callBack);             
                el.__callBack = createListener(el, evt, binding);
            },
            unbind: function (el, binding) {
                const evt = getEvent(binding);
                deleteListener(el, evt, el.__callBack);
            }
        };
    }

    var comand = createDirective(function (command, arg) {
        command.CanExecute(arg);
        if (command.CanExecuteValue)
            command.Execute(arg);
    });

    var simpleComand = createDirective(function (command, arg) {
        command.Execute(arg);
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