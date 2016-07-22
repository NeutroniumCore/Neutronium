//(function () {
//    console.log('init');
//    ko.Enumimages = { Sex: { Male: 'images/male.png', Female: 'images/sem%20t%c3%adtulo.png' } };
//}());

(function () {
    var images = {
         Sex: { Male: 'images/male.png', Female: 'images/sem%20t%c3%adtulo.png' } 
    };

    Vue._vmMixin = {
        methods:{
            enumImage: function(value){
                if (!value instanceof Enum)
                    return null;

                var ec = images[value.type];
                return ec ? ec[value.name] : null;
            }
        },
        computed: {
            count: function () {
                return this.Skills.length;
            }
        }
    }
}());




