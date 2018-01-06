<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Migration from version 1.1.0 to 1.2.0

## Attention Point
### Builds:

Version 1.2.0 supports x64 builds. There some gotchas thought:

- Make sure to disable project flag "Prefer 32-bit" (Properties>Build). 
- Make sure that all the projects of the solution have the same platform value. Supported values: `x86`, `x64` or `Any CPU`.


## Template created from `neutronium-vue`

1) Update dependencies version in `package.json`:

```
    "vue": "^2.5.13",
    "vue-template-compiler": "^2.5.13"
```

2) Update `main.js` file.
- First install `neutronium-vm-loader`:

```bash
npm install --save-dev neutronium-vm-loader
```

- Then update `main.js`<br>
From:

```js
import Vue from 'vue'
import App from './App.vue'
import rawVm from '../data/vm'
import CircularJson from 'circular-json'
import {install} from './install'

function updateVm(vm) {
    var window = vm.__window__
    if (window) {
        delete vm.__window__
        return { ViewModel: vm, Window: window }
    }
    return vm;
}
const vm = updateVm(CircularJson.parse(rawVm));

install(Vue)
new Vue({
    components:{
        App
    },
    el: '#main',
    data: vm
})
```

To:
```js
import Vue from 'vue'
import App from './App.vue'
import rawVm from '../data/vm'
import {install} from './install'
import { createVM } from 'neutronium-vm-loader'

const vm = createVM(rawVm);

install(Vue)
new Vue({
    components:{
        App
    },
    el: '#main',
    data: vm
})
```

3) Optional step: update `dist\index.html`

If you want to take advantage of the possibility of loading Vue runtime only to improve performance, perform the following changes:

- Update `dist\index.html`

From:
```HTML
<body>
    <div id="main">
      <App :view-model="$data.ViewModel" :__window__="$data.Window">
      </App>
    </div>
    <script src="./build.js"></script>
  </body>
```

To:
```HTML
<body>
    <div id="main"">
    </div>
    <script src="./build.js"></script>
  </body>
```

- Update `entry.js`

From:
```js
import Vue from 'vue'
import App from './App.vue'
import {install, vueInstanceOption} from './install'
import vueHelper from 'vueHelper'

install(Vue)
vueHelper.setOption(vueInstanceOption)
Vue.component('app', App)
```

To:
```js
import Vue from 'vue'
import App from './App.vue'
import {install, vueInstanceOption} from './install'
import vueHelper from 'vueHelper'


function buildVueOption(vm) {
    var option = vueInstanceOption(vm);
    option.render = function (h) {
        const prop = {
            props: {
                viewModel: this.$data.ViewModel,
                __window__: this.$data.Window
            }
        };
        return h(App, prop);
    }
    return option;
}

install(Vue)
vueHelper.setOption(buildVueOption)
```

-Finally to use runtime only Vue in Neutronium change in `App.xaml.cs`:

```CSharp
factory.RegisterJavaScriptFramework(new VueSessionInjectorV2());
```

To:
```CSharp
factory.RegisterJavaScriptFramework(new VueSessionInjectorV2 {RunTimeOnly = true});
```