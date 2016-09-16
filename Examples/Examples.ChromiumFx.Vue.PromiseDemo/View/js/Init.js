(function () {
    function asPromise(callback) {
        return function asPromise(argument) {
            return new Promise(function (fullfill, reject) {
            var res = { fullfill: function (res) { fullfill(res); }, reject: function (err) { reject(new Error(err)); } };
            callback.Execute(argument, res);
        });  
        }
    }

    const localMixin = {
        data: {
            factoryresult: null,
            lastError: null
        },
        methods: {
            asPromise : asPromise,
            click: function () {
                var self = this;
                asPromise(this.CreateObject)(this.Name)
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

    Vue._vmMixin = [localMixin];
})()