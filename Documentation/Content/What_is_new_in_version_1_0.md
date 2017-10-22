<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# What is new in version 1.0.0?

## Binding
* Massive performance improvement on updates from C# on both large data and frequent updates scenario
* Support of `byte` and `sbyte` types
* Conversion of dictionary with string keys to javascript object
* Support dynamic objects conversion: both `ExpandoObject` and inheritors of `DynamicObject`
* Support to [Bindable attribute](./Binding_in_Depth.md#binding-support)
* Introduction of `ICommand<T>`, `ISimpleCommand<T>` and `ICommandWithoutParameter` as well as corresponding `RelayCommand` to better control command type control.

## WPF Component
* Support of packuri allowing usage of `resource` type file to be used as HTML, CSS and javascript files. [See here](./Using_packuri.md)

## Vue.js integration
* Upgrade to **Vue.js v2.5.2**
* Upgrade to **Vue devtools v3.1.3**
* Performance improvement on update from C#

## Bug Fix:
* Correction of reentrance bug on update from javascript causing none update in some adage cases.