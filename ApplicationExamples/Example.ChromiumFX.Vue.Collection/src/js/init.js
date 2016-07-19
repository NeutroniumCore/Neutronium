(function () {
    Vue._vmMixin = {
        created: function() {
            console.log("created", this);
        },
        computed : {
            count : function() {
                return this.Skills.length;
            }
        }
    }
}());



