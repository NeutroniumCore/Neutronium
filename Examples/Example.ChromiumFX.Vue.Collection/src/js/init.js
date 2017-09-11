(function () {
    Vue._vmMixin = {
        created: function() {
        },
        computed : {
            count : function() {
                return this.ViewModel.Skills.length;
            }
        }
    }
}());



