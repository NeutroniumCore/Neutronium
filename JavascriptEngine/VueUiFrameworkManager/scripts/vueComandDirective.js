(function () {
    Vue.directive("comand", {
        params: ["comandarg"],
        bind: function() {
        },
        update: function (newValue, oldValue) {
            if (!!oldValue) {
                this.el.removeEventListener(this.arg, this.callBack);
            }
            if (!!newValue) {
                var ctx = this;
                this.callBack = function callBack() {
                    var arg = ctx.params.comandarg || null;
                    console.log(arg);
                    if (newValue.CanExecute(arg)==undefined && newValue.CanExecuteValue)
                        newValue.Execute(arg);
                };
                this.el.addEventListener(this.arg, this.callBack);
            }
        },
        unbind: function () {
            if (!!this.callBack) {
                this.el.removeEventListener(this.arg, this.callBack);
            }
        }
    });
}());