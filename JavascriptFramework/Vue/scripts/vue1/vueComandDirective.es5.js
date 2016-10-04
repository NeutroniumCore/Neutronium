/*jshint esversion: 6 */
"use strict";

(function () {
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
                    var arg = ctx.params.commandarg || null;
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
})();

