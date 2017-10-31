(function () {
    var localMixin = {
        computed: {
            completeName: function () {
                var valor = this.Name + " " + this.LastName;
                console.log(valor);
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