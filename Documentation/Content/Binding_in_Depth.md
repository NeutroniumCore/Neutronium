<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Binding in Depth

## Basic features

 * All CLR types are supported by Neutronium:
  String, Int, Double, Decimal are transformed in their javascript equivalent.

 * C# DateTime type is mapped to javascript dateTime.

 * Enum is transfomed to custom javascript object containing two properties: intValue and displayName. intValue is the numeric value of the enum, displayName is the value of the Description attribute if any or the object.ToString() value.

 * Complex object are mapped to javascript using reflection on public attributes.


## Binding support

* Tracking of property changes is done via **_INotifyPropertyChanges_** interface. This way changes in the ViewModel are reflected in the HTML View.

* Tracking of collection changes is supported listening **_INotifyCollectionChanges_** interface. This way, any collection implementing this interface will be updated in the view when changing. This provides native integration with **_ObservableCollection<T>_**.

* Changes in HTML View are propagated to ViewModel using knockoutjs subscription or Vuejs watch (both property and collection). This allows you for example to have a collection binding to the selected items in the HTML view that will bind to your ViewModel collection.

* **_ICommand_** are converted to javascript function so you can execute them using knockout or Vue.

## Complex viewmodel supported

-Nested ViewModel fully supported

-One to one object mapping (that is if you have a same object referenced n times in your C# ViewModel, it will be mapped only one time and reference n times in the javascript session).


## Import
Only public property are mapped during binding. So it may be a good idea to use internal for property that have no effect on the view, as it may improve binding performance.


Full working examples are provided in the Neutronium examples folder. See projects:
* [Example.ChromiumFX.Vue.UI](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.ChromiumFX.Vue.UI)
* [Example.ChromiumFX.Ko.UI](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.ChromiumFX.Ko.UI)
* [Example.CefGlue.Ko.SelectedItems](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.CefGlue.Ko.SelectedItems)
* [Example.ChromiumFX.Vue.Collection](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.ChromiumFX.Vue.Collection)

### [How it works](./How_it_works.md)

[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)
