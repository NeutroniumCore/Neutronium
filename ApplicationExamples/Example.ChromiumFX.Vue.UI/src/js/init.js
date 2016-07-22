(function () {
    var localMixin = {
        data: {
            enumImages: {
                Sex: {
                    Male: "images/male.png",
                    Female: "images/sem%20t%c3%adtulo.png"
                }
            }
        },
        computed: {
            count: function() {
                return this.Skills.length;
            }
        }
    };

    Vue._vmMixin = [localMixin, glueHelper.enumMixin];
}());




