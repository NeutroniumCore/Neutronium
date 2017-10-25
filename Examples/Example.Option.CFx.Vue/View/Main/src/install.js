import VueI18n from 'vue-i18n'

const messages = {
  en: {
    message: {
      hello: 'hello world'
    }
  },
  ja: {
    message: {
      hello: 'こんにちは、世界'
    }
  },
  ru: {
    message: {
        hello: "привет, мир!"
    }
  }
}

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