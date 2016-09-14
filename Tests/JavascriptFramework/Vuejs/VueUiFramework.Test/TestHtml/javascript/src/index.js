(function () {
    console.log("index");
    var localMixin = {
        computed: {
            completeName: function () {
                return this.Name + " " + this.LastName;
            }
        }
    };

    Vue.component('commandbutton', {
        mixins: [Vue.__commandMixin],
        props: {
            msg: String
        },
        template: "<button  @click='execute'>{{msg}}</button>"
    })

    Vue._vmMixin = [localMixin];
}());