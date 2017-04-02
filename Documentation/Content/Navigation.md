<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Navigation

## Creating navigation

### Basic Navigation

To use navigation, you have to use HTMLWindow UserControl  instead of HTMLViewControl.

The main difference between the two is that HTMLWindow exposes an **_INavigationBuilder_** interface and implements **_INavigationSolver_**.

**_INavigationBuilder_** is meant to build the application routing by associating a viewmodel type to a specific HTML file. HTMLWindow exposes the public property INavigationBuilder NavigationBuilder. If the same ViewModel type can be displayed using diferent View you can use the Id string to discrimate the Views.

```CSharp
public interface INavigationBuilder
{
   void Register<T>(string iPath,string Id=null); 
   void RegisterAbsolute<T>(string iPath, string Id = null); 
   void Register<T>(Uri iPath, string Id = null); 
}
```

Ex:
```CSharp
   var navigatorBuilder = myHTMLWindow.NavigationBuilder;
   navigatorBuilder.Register<Vm1>("View\\index.html");
   navigatorBuilder.Register<Vm2>("View\\index2.html");
   navigatorBuilder.Register<Vm2>("View\\index3.html", "alternative");
```
 
Once the routing is done, you can navigate from ViewModel to ViewModel using the INavigationSolver interface implements by the HTMLWindow:

```CSharp
public interface INavigationSolver : IDisposable
{
   bool UseINavigable { get; set; }
   Task NavigateAsync(object viewModel, string Id = null, JavascriptBindingMode iMode = JavascriptBindingMode.TwoWay);
   event EventHandler<NavigationEvent> OnNavigate;
}
```

The NavigateAsync method will find the HTLM page associated with the viewModel using the INavigationBuilder resolution and apply a binding beetween the View and the ViewModel using the corresponding iMode. 


Ex:
```CSharp
    //Navigate to View\index.html
    await myHTMLWindow.NavigateAsync(vm);

    //Navigate to View\index2.html
    await myHTMLWindow.NavigateAsync(vm2);

    //Navigate to View\index3.html
    await myHTMLWindow.NavigateAsync(vm2, "alternative");
```

OnNavigate event is called everytime the ViewModel changes.
If UseINavigable is set to true and the ViewModel implements the INavigable interface the Navigation setter is called during navigation allowing that a ViewModel knows the INavigationSolver and use it to navigate to another ViewModel.
```CSharp
public interface INavigable
{
   INavigationSolver Navigation { get; set; }
} 
```

### Convetion Navigation

Since Core version 0.5.0, Neutronium has navigation helper that provides short-cut for navigation based on convention. For example:

```CSharp
public static void Register(INavigationBuilder builder)
{
    // Create a convention for the corresponding builder
    // Every type will be registered using the template
    // "View\{vm}\dist\index.HTML" where VM will be the class
    // name without postfix "ViewModel" if nay
    var convention = builder.GetTemplateConvention(@"View\{vm}\dist\index.HTML");

    // Use fluent helper to register class from same assembly as RoutingConfiguration
    // in "NeutoniumDemo.ViewModel" namespace excliding ApplicationMenuViewModel
    typeof(RoutingConfiguration).GetTypesFromSameAssembly()
                                .InNamespace("NeutoniumDemo.ViewModel")
                                .Except(typeof(ApplicationMenuViewModel))
                                .Register(convention);
}
```

See [BuilderExtension.cs](../../Neutronium.Core/Navigation/Routing/BuilderExtension.cs),  [TypesProviderExtension.cs](../../Neutronium.Core/Navigation/Routing/TypesProviderExtension.cs) and [ITypesProvider.cs](../../Neutronium.Core/Navigation/Routing/ITypesProvider.cs) for detailed API desscription.

## Transition

HTMLWindow UserControl embeds two WebBrowser used to ensure smooth transition between view: one is used to display the current view, the other is used when NavigateAsync is called: the next view is then loaded in the second WebControl, the ViewModel is then bound and finally this WebControl becomes visible.

During this process, it is possible to display javascript animation when one view is closing and when one View is first displayed.

This possible due to custom hook implmented by both Vue and knockout binding

### For knockout
You can use custom bindings **_onopened_** and **_onclose_**. Ex:
```HTML
<div class="box" data-bind="onopened:OnEnter, onclose:OnClose"></div>
```
where: 
* OnEnter is a function receiving one unique argument which is the element that owns the binding. It is called when the HTLM View is displayed.
* OnClose is a function receiving as a first argument a callback to be called when the animation is over and as second argument the element that owns the binding. It is very important to always call the callback as the new View will only be displayed after the callback is called.

Ex:
```javascript
function OnEnter(element){
    $(element).addClass("boxanimated");
}
      
function OnClose(callback, element){
    $(element).removeClass("boxanimated");
    setTimeout(callback, 2000);
}
```

Full example can be found in the projects: 
* [Example.CefGlue.Ko.BasicNavigation](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.CefGlue.Ko.BasicNavigation)
* [Example.CefGlue.Ko.Navigation](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.CefGlue.Ko.Navigation)


### For Vue

Yo can use customand built-in mixins to trigger animations (see [this section](https://github.com/David-Desmaisons/Neutronium/wiki/Vue-Binding#using-built-in-mixin) for more details on Vue mixin in Neutronium)

Once added to Vue instance, **_glueHelper.openMixin_** will triger a call to ViewModel method **_onOpen_** when the view is ready. 

Once added to Vue instance, **_glueHelper.closeMixin_** will triger a call to ViewModel method **_onClose_** when the view is closing. 

Ex:
```javascript
var localMixin = {
        methods: {
            onOpen: function (callback) {
                $("#BB").addClass("boxanimated");
                setTimeout(callback, 2000);
            },
            onClose: function (callback) {
                $("#BB").removeClass("boxanimated");
                setTimeout(callback, 2000);
            }
        },
    };

    Vue._vmMixin = [localMixin, glueHelper.openMixin, glueHelper.closeMixin];
```

Full example can be found in the project: 
* [Example.ChromiumFx.Vue.BasicNavigation](https://github.com/David-Desmaisons/Neutronium/tree/master/Examples/Example.ChromiumFx.Vue.Navigation)


[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)

 