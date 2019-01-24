# Navigation

## Routing or javascript-side navigation

This is the recommended navigation when creating a large Neutronium application:
* A C# implementation is provided as part of [Neutronium Building Blodk](../tools/building-block.html#Application)
* A Vue implementation using [vue-router](https://router.vuejs.org/) is provided by [vue-cli plugin](../tools/vue-cli-plugin#Application)

In this architecture:
* Navigation convention is created on the c# side
* Javascript as the responsibility of  

A full example integrating these two components is provided in the [Neutronium.SPA template](https://github.com/NeutroniumCore/Neutronium.SPA.Template).

### Creating navigation

**_INavigationBuilder_** is meant to build the application routing by associating a viewModel type to a specific HTML file. HTMLWindow exposes the public property INavigationBuilder NavigationBuilder. If the same ViewModel type can be displayed using different View you can use the Id string to discriminate the Views.

```CSharp
public interface INavigationBuilder
{
   void Register<T>(string path,string Id=null); 
   void RegisterAbsolute<T>(string path, string Id = null); 
   void Register<T>(Uri path, string Id = null); 
}
```

Example for javascript routing navigation:
```CSharp
   var navigatorBuilder = myHTMLWindow.NavigationBuilder;
   navigatorBuilder.Register<Vm1>("vm1");
   navigatorBuilder.Register<Vm2>("vm2");
   navigatorBuilder.Register<Vm2>("vm3", "alternative");
```

Example for C# navigation:
```CSharp
   var navigatorBuilder = myHTMLWindow.NavigationBuilder;
   navigatorBuilder.Register<Vm1>("View\\index.html");
   navigatorBuilder.Register<Vm2>("View\\index2.html");
   navigatorBuilder.Register<Vm2>("View\\index3.html", "alternative");
```
 
Once the routing is done, you can navigate from ViewModel to ViewModel using the INavigationSolver interface implements by the HTMLWindow:





## C#-side navigation

### Creating navigation

To use navigation, you have to use HTMLWindow UserControl instead of HTMLViewControl.

The main difference between the two is that HTMLWindow exposes an **_INavigationBuilder_** interface and implements **_INavigationSolver_**.


```CSharp
public interface INavigationSolver : IDisposable
{
   bool UseINavigable { get; set; }
   Task NavigateAsync(object viewModel, string Id = null, JavascriptBindingMode mode = JavascriptBindingMode.TwoWay);
   event EventHandler<NavigationEvent> OnNavigate;
}
```

The NavigateAsync method will find the HTLM page associated with the viewModel using the INavigationBuilder resolution and apply a binding between the View and the ViewModel using the corresponding iMode. 


Ex:
```CSharp
    //Navigate to View\index.html
    await myHTMLWindow.NavigateAsync(vm);

    //Navigate to View\index2.html
    await myHTMLWindow.NavigateAsync(vm2);

    //Navigate to View\index3.html
    await myHTMLWindow.NavigateAsync(vm2, "alternative");
```

OnNavigate event is called every time the ViewModel changes.
If UseINavigable is set to true and the ViewModel implements the INavigable interface the Navigation setter is called during navigation allowing that a ViewModel knows the INavigationSolver and use it to navigate to another ViewModel.
```CSharp
public interface INavigable
{
   INavigationSolver Navigation { get; set; }
} 
```

### Convention Navigation

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
    // in "NeutoniumDemo.ViewModel" namespace excluding ApplicationMenuViewModel
    typeof(RoutingConfiguration).GetTypesFromSameAssembly()
                                .InNamespace("NeutoniumDemo.ViewModel")
                                .Except(typeof(ApplicationMenuViewModel))
                                .Register(convention);
}
```

See [BuilderExtension.cs](../../Neutronium.Core/Navigation/Routing/BuilderExtension.cs),  [TypesProviderExtension.cs](../../Neutronium.Core/Navigation/Routing/TypesProviderExtension.cs) and [ITypesProvider.cs](../../Neutronium.Core/Navigation/Routing/ITypesProvider.cs) for detailed API description.



### Transition

HTMLWindow UserControl embeds two WebBrowser used to ensure smooth transition between view: one is used to display the current view, the other is used when NavigateAsync is called: the next view is then loaded in the second WebControl, the ViewModel is then bound and finally this WebControl becomes visible.

During this process, it is possible to display javascript animation when one view is closing and when one View is first displayed.

This possible due to custom hook implemented by both Vue and knockout binding

#### For knockout
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
