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
                    var arg = ctx.params.comandarg || null;
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
            if (newValue.CanExecute(arg) == undefined && newValue.CanExecuteValue)
                newValue.Execute(arg);
        }
    }, base);

    Vue.directive("command", comand);

    var simpleComand = Object.assign({
        execute: function (newValue, arg) {
            newValue.Execute(arg);
        }
    }, base);

    Vue.directive("simpleCommand", simpleComand);

    var commandMixin = {
        props: {
            command: Object,
            arg: {
                type: Object,
                default: null
            }
        },
        computed: {
            canExecute: function () {
                return this.command.CanExecuteValue;
            }
        },
        watch: {
            'command.CanExecuteCount': function() {
                this.command.CanExecute(this.arg);
            }
        },
        ready: function () {
            Vue.nextTick(function () {
                if (!!this.arg)
                    this.command.CanExecute(this.arg);
            })
        },
        methods:{
            execute: function(){
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