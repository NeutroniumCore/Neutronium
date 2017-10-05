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
            if (!newValue.CanExecute) {
                newValue.Execute(arg);
                return;
            }
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
}());