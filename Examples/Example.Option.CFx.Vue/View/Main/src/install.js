import VueI18n from 'vue-i18n'
import messages from './Dictionary'

function install(vue) {
    //Call vue use here if needed
    vue.use(VueI18n);
}

function vueInstanceOption(vm) {
    const i18n = new VueI18n({
        locale: 'fr-FR', // set locale
        messages, // set locale messages
    });
    console.log(vm);

    //Return vue global option here, such as vue-router, vue-i18n, mix-ins, .... 
    return {i18n}
}

export {
    install,
    vueInstanceOption
} 