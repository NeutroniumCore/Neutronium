(function () {
    const localMixin = {
        data: {
            factoryresult: null,
            lastError: null,
            inputName: null
        },
        methods: {
            click: function () {
                var self = this;
                this.asPromise(this.CreateObject)(this.Name)
                    .then(function (res) {
                        alert(res.LastName);
                        self.factoryresult = res;
                    })
                    .catch(function (reason) {
                        console.log(reason);
                        alert(reason);
                    });
            },
            error: function (error) {
                alert(error);
                this.lastError = error;
            },
            result: function (res) {
                alert(res.LastName);
                this.factoryresult = res;
            }
        }
    };

    Vue._vmMixin = [window.glueHelper.promiseMixin, localMixin];
})()