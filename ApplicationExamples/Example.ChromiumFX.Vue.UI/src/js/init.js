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

    Vue.component('commandbutton', {
        mixins: [Vue.__commandMixin],
        methods:{
            beforeCommand: function(){
                alert('add skill');
            }
        },
        props: {
            msg: String
        },
        template: "#commandbuttontemplate"
        //template: '<div class="button" :class="{ ''on'': canExecute, "off": !canExecute }" @dblclick="execute">{{msg}}</div>'
    })

    Vue._vmMixin = [localMixin, glueHelper.enumMixin];
}());




