<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# What is new in version 1.0.0?

## Binding
* Massive performance improvement on updates from C# on both large data and frequent updates scenario.

    V.0.6.0<br>
    <img src="../perf0.6.gif"><br>

    V.1.0.0<br>
    <img src="../perf1.0.gif"><br>




* Support of `byte` and `sbyte` types.
* Conversion of dictionary with key string to javascript object.
* Support dynamic objects conversion: both `ExpandoObject` and inheritors of `DynamicObject`.
* Support to [Bindable attribute](./Binding_in_Depth.md#binding-support).
* Introduction of `ICommand<T>`, `ISimpleCommand<T>` and `ICommandWithoutParameter` as well as corresponding `RelayCommand` to better control command argument type. See [here](./MVVMComponents.md) for full details

## WPF Component
* Support of pack-uri allowing usage of `resource` type file to be used as HTML, CSS and javascript files thanks to [lhyqy5](https://github.com/lhyqy5). Documentation [here](./Using_packuri.md).

## Vue.js integration
* Upgrade to **Vue.js v2.5.2**
* Upgrade to **Vue devtools v3.1.3**
* Possibility to set Vue instance options (useful for using `vue-router` or `vue-i18n` for example) [see here](./Build_large_project_with_Vue.js_and_Webpack.md#installjs-file).
* Performance improvement on update from C#

## Tooling
* Compatible with [neutronium-vue vue-cli version 4.0.0](https://github.com/NeutroniumCore/neutronium-vue)

## Migrating to version 1.0.0
* If you need to upgrade from v.0.6.0, see here the [guidelines](./Migrate_from_0_6_to_version_1_0.md)

## Bug Fix:
* Correction of reentrance bug on update from javascript causing none update in some edge cases.