(function () {
    window.glueHelper.setOption({
        mixins: [{
                created: function() {
                },
                computed: {
                    count: function() {
                        return this.ViewModel.Skills.length;
                    }
                }
            }
        ]
    });
}());



