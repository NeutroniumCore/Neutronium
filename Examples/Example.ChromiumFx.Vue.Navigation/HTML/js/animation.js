(function () {
    console.log("animation");
    
    var localMixin = {
        methods: {
            onOpen: function (callback) {
                $("#BB").addClass("boxanimated");
                setTimeout(callback, 2000);
            },
            onClose: function (callback) {
                $("#BB").removeClass("boxanimated");
                setTimeout(callback, 2000);
            }
        },
    };

    window.glueHelper.setOption({
        mixins: [localMixin, glueHelper.openMixin, glueHelper.closeMixin]
    });
}());
