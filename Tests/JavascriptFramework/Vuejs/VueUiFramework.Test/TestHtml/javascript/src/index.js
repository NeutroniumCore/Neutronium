(function () {
    var localMixin = {
        computed: {
            completeName: function () {
                var valor = this.Name + " " + this.LastName;
                return valor;
            }
        }
    };

    Vue.component('commandbutton', {
        mixins: [glueHelper.commandMixin],
        props: {
            msg: String
        },
        template: "<button  @click='execute'>{{msg}}</button>"
    });

    window.glueHelper.setOption({
        mixins: [localMixin]
    });

}());