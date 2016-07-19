(function () {
    Vue._vmMixin = {
        created: function () {
            console.log("created", this);
        }
    };
}());



