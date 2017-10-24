(function () {
    var localMixin = {
        data: {
            result: null,
            error: null
        },
        methods: {
            onResult: function (res) {
                this.result = res;
            },
            onError: function (error) {
                this.error = error;
            }
        }
    };

    window.glueHelper.setOption({
        mixins: [localMixin]
    });
}());