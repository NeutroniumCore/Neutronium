<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Binding in Depth

## Basic features

 * All CLR types are supported by Neutronium:
  String, char, int, uint, short, ushort, long, ulong, double, decimal, float, byte and sbyte (both in upcoming v1.0.0) are transformed in their javascript equivalent.

 * C# DateTime type is mapped to javascript dateTime.

 * Enum are transformed to custom javascript objects containing two properties: intValue and displayName. intValue is the numeric value of the enum, displayName is the value of the Description attribute if any or the object.ToString() value.

 * C# collections such as IEnumerable, IList are converted to javascript arrays.

  * C# dictionaries with string key are converted to javascript objects (from version >= 1.0.0).

  * Dynamic Objects (from version >= 1.0.0):

    * [`ExpandoObject`](https://msdn.microsoft.com/en-us/library/system.dynamic.expandoobject(v=vs.110).aspx) objects are converted to javascript objects allowing two-way binding including updating and adding keys.

    * Objects inheriting from [`DynamicObject`](https://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject(v=vs.110).aspx) are converted to javascript objects using properties returned by [GetDynamicMemberNames](https://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.getdynamicmembernames(v=vs.110).aspx)

 * Complex objects are mapped to javascript using reflection on public attributes.


## Binding support

* Tracking of property changes is done via **_INotifyPropertyChanges_** interface. This way changes in the ViewModel are reflected in the HTML View.

* Tracking of collection changes is supported listening **_INotifyCollectionChanges_** interface. This way, any collection implementing this interface will be updated in the view when changing. This provides native integration with **_ObservableCollection<T>_**.

* Changes in HTML View are propagated to ViewModel using knockoutjs subscription or Vuejs watch (both property and collection). This allows you for example to have a collection binding to the selected items in the HTML view that will bind to your ViewModel collection.

* **_ICommand_** are converted to javascript function so you can execute them using knockout or Vue.

* **BindableAttribute** support (from version >= 1.0.0)

    Neutronium uses [BindableAttribute](https://msdn.microsoft.com/en-us/library/system.componentmodel.bindableattribute(v=vs.110).aspx) information when creating bindings.
    Property marked as bindable false will not be accessible from javascript:
```CSharp
    [Bindable(false)]
    public string InvisibleFromNeutroniumBinding {get; set;}
```
Property marked as readonly will not be updatable from javascript:
```CSharp
    [Bindable(true, BindingDirection.OneWay)]
    public string NotSettableFromNeutroniumBinding {get; set;}
```
It is possible to use BindableAttribute attribute at class level, this way all properties of the class will default with the corresponding attribute value. That value can be overloaded by attribute at property level:
```CSharp
[Bindable(false)]
public class ViewModel {
    // Invisible as inherited from class attribute
    public string InvisibleFromNeutroniumBinding {get; set;}

    [Bindable(true, BindingDirection.TwoWay)]
    public string SettableFromNeutroniumBinding {get; set;}
}
```

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
