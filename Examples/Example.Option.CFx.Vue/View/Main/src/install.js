import VueI18n from 'vue-i18n'
import {messages} from './messages'

function install(vue) {
    //Call vue use here if needed
    vue.use(VueI18n);
}

function vueInstanceOption() {
    const i18n = new VueI18n({
        locale: 'ru', // set locale
        messages, // set locale messages
    });

    //Return vue global option here, such as vue-router, vue-i18n, mix-ins, .... 
    return {i18n}
}

export {
    install,
    vueInstanceOption
} 