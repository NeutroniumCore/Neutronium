<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Vue cli template

Neutronium provides a vue-cli template [neutronium-vue](https://github.com/NeutroniumCore/neutronium-vue) that provides many advantages to develop large project with Neutronium vue:
* During development use web-pack-dev-server and chrome to create the UI
* Use ES6, sass, less...
* Decompose your project in easy to maintain vue files during development
* Use npm to manage your dependencies
* Use Webpack build to generate all files you need to reference in Neutronium


To install neutronium  vue template use [vue-cli](https://github.com/vuejs/vue-cli)
If not installed, install first vue-cli:

``` bash
$ npm install -g vue-cli
```

Then in the view folder create your project 
``` bash
$ vue init NeutroniumCore/neutronium-vue view1
$ cd view1
$ npm install
$ npm run dev
```

## npm scripts

```bash
npm run dev
```
Debug your view in the browser. MainView model data are provided by json file: data\data.json

```bash
npm run build
```
Generate files ready to be used in Neutronium in the dist folder: you have to reference these files (Content/Copy Always) in visual studio.


## Step by step installation
1. Open folder where you want to install the view from VS<br>
<img src="../images/webpack_01.png" height="550px"><br>
2. Install template
``` bash
$ vue init David-Desmaisons/neutronium-vue about<br>
```
<img src="../images/webpack_02.png" height="550px"><br>
3. npm install
``` bash
$ cd about
$ npm install
```
<img src="../images/webpack_03.png"><br>
4. Begin developing the view using hot-reload (you may use atom or sublime to edit js files)
``` bash
$ npm run dev
```
<img src="../images/webpack_05.png"><br>
5. Once the view is ready build the files
``` bash
$ npm run build
```
<img src="../images/webpack_06.png" height="550px"><br>
6. Include the files in VD<br>
* Click show all files<br>
<img src="../images/webpack_07.png" height="250px"><br>
<img src="../images/webpack_08.png" height="550px"><br>
* Include the files in project: DO NOT INCLUDE files under node_modules<br>
<img src="../images/webpack_09.png" height="550px"><br>
<img src="../images/webpack_10.png" height="550px"><br>
* Set Properties on dist files: Content/Copy Always<br>
<img src="../images/webpack_11.png" height="550px"><br>
7. Run C# application<br>
<img src="../images/webpack_12.png" ><br>


## Folder organization

``` bash
├── data
├── dist
├── src
│   ├── asset
│   ├── components
│   ├── App.vue
│   ├── entry.js
│   ├── install.js
│   └── main.js
├── index.hml
``` 

`Data`: contains the data.json which is the viewmodel data used during development in the browser.
`Dist`: contains generated files to be used in Neutronium
`src`: contains assets (folder assets), vue components (folder components), main component: App.vue.
You should not edit `entry.js` nor `main.js` which are boilerplate files needed for the dev and production build.
Both index.html files (fromm root and dist) should not be edited for the same reason.

## Main file:

App.Vue represent the main entry of the vue application, its prop ``mainViewModel``` represent the C# viewModel

### Install.js file

If you need to register globally plugin in Vue instance use install.js.
Example:

```javascript
import Notifications from 'vue-notification'

function install(vue) {
    vue.use(Notifications)
}

export {
    install
} 
```

For version >=1.0.0, it is also possible to use install.js to set Vue instance options.
This is needed if you want to use mixins, or some popular tools such as [vue-router](https://router.vuejs.org/en/) or [vue-i18n](https://github.com/kazupon/vue-i18n).
To do so you need to export a vueInstanceOption function returning Vue instance option.

Example:

```javascript
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
```


## Tips:

You can generate a Json from viewModel captured in a Neutronium debug session using [Neutronium debug tools](./Debug.md) and use it as data.json in order to create the view with a realistic ViewModel. 

![SaveVM](../images/Tools/Toolbar-save.png)

### [Additional vue.js components](./Using_aditional_dedicated_vue.js_component.md)

[How to set up a project](./SetUp.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)

