(function () {
    var localMixin = {
        computed: {
            completeName: function () {
                return this.Name + " " + this.LastName;
            }
        }
    };

    Vue._vmMixin = [localMixin];
}());