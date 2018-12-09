<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Component Overview

## UserControl

Neutronium exposes two WPF UserControls: **HTMLViewControl** and **HTMLWindow** both are embedding a WebBrowser and share main implementation.

### Common API:
* **IsDebug** property allows use of [debug tools](./Debug.md), use false in production mode.

* **HTMLEngine**:
The name of the WebBrowser to be used in this view. If only one WebBrowser is registered (which should be the case normally), you don´t need to set-up this value. See [HTMLEngineFactory section](#htmlenginefactory) for more details.
Current Options: Awesomium, Cef.Glue, ChromiumFx


* **JavascriptUIEngine**:
The name of the javascript framework to be used in this view. If only one JavascriptUIEngine is registered, you don´t need to set-up this value. See [HTMLEngineFactory section](#htmlenginefactory) for more details.
Current Options: VueInjector, KnockoutInjector

* **IDisposable**
You should call IDisposable Dispose method on both component when closing the window to prevent leaks due to event listening.

### HTMLViewControl

Use **HTMLViewControl** if you have one HTML view and a DataContext.

```HTML
<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:wpf="clr-namespace:Neutronium.WPF;assembly=Neutronium.WPF"
        x:Class="Example.Awesomium.Vue.UI.MainWindow"
        Height="350" Width="525">
    <Grid>
       <wpf:HTMLViewControl  HTMLEngine="Awesomium" IsDebug="True" Uri="pack://application:,,,/View/dist/index.html"/>
    </Grid>
</Window>
```

* **Uri**:

Reference the location of the HTML file. When local files are reference pack uri should be used (file properties should be _Resource_, _None_ ). [Click here for complete description on how to reference file](./Reference_Files.md)

* **Deprecated: RelativeSource**:

Reference the location of the HTML file (properties should be _Content_, _Copy Always_ ). Please use uri with pack url instead. [Click here for complete description on how to reference file](./Reference_Files.md)

* **DataContext**:

As any WPF component, DataContext is used to create the binding with the ViewModel. Note that DataContext is not set, the HTML will not be displayed.


### HTMLWindow

Use **HTMLWindow** if you have various HTML files and DataContext. This control implements navigation APIs;

```HTML
<Window
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:wpf="clr-namespace:Neutronium.WPF;assembly=Neutronium.WPF"
        x:Class="Example.ChromiumFx.Vue.Navigation.MainWindow"
        Title="HTML5 vs WPF" Height="350" Width="525">
    <Grid>
        <wpf:HTMLWindow  UseINavigable="True" IsDebug="True" x:Name="HTMLWindow"/>

    </Grid>
</Window>
```

* **UseINavigable**
If true, Neutronium calls set _INavigable_ property _Navigation_ with corresponding _INavigationSolver_

See [Navigation API](./Navigation.md) to complete description of how to use this component.


## Important Note
When **using [templates](https://visualstudiogallery.msdn.microsoft.com/c7679997-e25b-4a79-a65f-30758fb756d8)** HTMLEngineFactory API are done by boilerplate file. So in this case, **you can pull next section.**


## HTMLEngineFactory

At its core Neutronium is abstracted from javascript library and WebBrowser implementation: this is why you can plug Knockout or Vue engine or use different WebBrowsers.


You need to register these abstractions in the static **HTMLEngineFactory.Engine** in order to be able to use them.

Ex:
```CSharp
 var engine = HTMLEngineFactory.Engine;
 engine.RegisterHTMLEngine(new ChromiumFXWPFWebWindowFactory());
 engine.RegisterJavaScriptFramework(new VueSessionInjector());      
```

**Available HTMLEngines:**

    ChromiumFXWPFWebWindowFactory, CefGlueWPFWebWindowFactory, AwesomiumWPFWebWindowFactory

**Note that Recommended engine is ChromiumFXWPFWebWindowFactory.** AwesomiumWPFWebWindowFactory is present for legacy reason and CefGlueWPFWebWindowFactory is used for test purposes.

**Available JavaScriptFrameworks:**

    VueSessionInjector, KnockoutFrameworkManager


You need to dispose the engine when closing the session:
```CSharp
HTMLEngineFactory.Engine.Dispose();
```

## Built-in WPF Application

Each WebBrowser implementation makes available an abstract implementation of an WPF application that register the corresponding WebBrowser and dispose the HTMLEngineFactory  when application ended. The only thinks you need is to register the javascript engine.

Ex: using ChromiumFx and corresponding ChromiumFxWebBrowserApp :

```CSharp
public class WebBrowserApp : ChromiumFxWebBrowserApp 
{
    protected override IJavascriptFrameworkManager GetJavascriptUIFrameworkManager() 
    {
        return new VueSessionInjector();
    }
}
```

## See Next:
 
[Vue Binding](./Vue_Binding.md)

[knockout Binding](./Knockout_Binding.md)    

[How to set up a project](./SetUp.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)





