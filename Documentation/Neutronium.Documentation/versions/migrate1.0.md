<a href="https://github.com/NeutroniumCore/Neutronium" target="_blank">
  <img
    style="position: fixed; top: 0; right: 0; border: 0; z-index:99999"
    width="149"
    height="149"
    src="https://github.blog/wp-content/uploads/2008/12/forkme_right_gray_6d6d6d.png?resize=149%2C149"
    class="attachment-full size-full"
    alt="Fork me on GitHub"
    data-recalc-dims="1"
  />
</a>

# Migration from version 0.6.0 to 1.0.0

## Breaking changes
### C# changes
* `ISimpleCommand` signature changed. You should migrate to wether `ISimpleCommand<T>` or `ISimpleCommand` is no argument needs to be passed. Complete details [here](../binding/mvvm-components.html)

* `IHTMLBinding` has been renamed `IHtmlBinding`

* Updates on the ViewModel should now happen on the UI thread, since Neutronium is not redirecting them anymore.<br>
Practically, this means that you should ensure that any `INotifyPropertyChanged` events and ObservableCollection updates occurs on the UI threads:
you can use [WPF Dispatcher](https://msdn.microsoft.com/en-us/library/system.windows.threading.dispatcher(v=vs.110).aspx) `Invoke` or `BeginInvoke` methods.<br>
Neutronium ALWAYS will call ViewModel on the UI thread either when updating properties value or executing command. 

### Vue scripts

* Using vue mixins
If you were using manual, registration, change:
```
Vue._vmMixin = [localMixin];
```

For:
```
window.glueHelper.setOption({mixins : [localMixin]});
```

Even better you can use `install.js` file to register mixin if using [neutronium-vue template](.../tools/vue-cli-plugin.html#installjs-file).

### ViewModel binding

Starting from version 1.0.0, Neutronium is converting main ViewModel and window viewModel to an object:
```javascript
{
    ViewModel,
    __window__
}
```
This has no impact if you are using `neutronium-vue` from scratch as new template take this into account.

If you are using `knockout`, you may using `with` binding around application mark-up to set-up the context to `ViewModel`:
```HTML
<!-- ko with:$data.ViewModel()-->
    <!--application-->

<!--/ko-->
``` 

Next section describes how to update project created in version v0.6.0 using `neutronium-vue`.

### Template created from `neutronium-vue`

Update dependencies version:

    "vue": "^2.5.2",
    "vue-loader": "^13.3.0",
    "vue-style-loader": "^3.0.3",
    "vue-template-compiler": "^2.5.0"

Adjust the following files:
* `index.html` (two files, both under root and in the `.\dist` folder)

Change
```HTML
<App :view-model="$data">
</App>
```
To:
```HTML
<App :view-model="$data.ViewModel" :__window__="$data.Window">
</App>
```

* `App.vue`

Change
```javascript
const props  ={
    viewModel: Object,
};
```
To:
```javascript
const props={
    viewModel: Object,
    __window__: Object
};
```
* `main.js`

Change
```javascript
const vm = CircularJson.parse(rawVm);
```
To:
```javascript
function updateVm(vm) {
    var window = vm.__window__
    if (window) {
         delete vm.__window__
        return { ViewModel: vm, Window: window }
    }
    return vm;
}
const vm = updateVm(CircularJson.parse(rawVm));
```


