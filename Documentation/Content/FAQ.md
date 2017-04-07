<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Frequently Asked Questions

* **Why Neutronium?**

The idea behind Neutronium is to provide a HTML-CSS-Javascript UI engine enpowered with two way bindings supporting all features of WPF MVVM bindings.
This way Neutronium can be as "remplacement" of WPF.


* **How Neutronium compares with other libraries?**

    * [Electron](http://electron.atom.io/)

        Neutronium is electron for .NET? Well, kind of. Neutronium however is a higher abstraction so that you don't need to care about Chromium implementation such as renderer or browser processes.

    * [Awesomium](http://www.awesomium.com/), [CefGlue](http://xilium.bitbucket.org/cefglue/), [ChromiumFx](https://bitbucket.org/chromiumfx/chromiumfx), [CefSharp](https://github.com/cefsharp/CefSharp)

        All are libraries offering binding to Chromium using proprietary binding (Awesomium), or [Chromium Embedded Framewok](https://en.wikipedia.org/wiki/Chromium_Embedded_Framework) bindings. All of them present much lower level of integration than Neutronium. Note that internally Neutronium uses ChromiumFx.


    * [react-native-windows](https://github.com/Microsoft/react-native-windows)

        Provide react to WPF/UWP bindings. Almost the contrary aproach as Neutronium as react-to-native converts react to native UI component whereas Neutronium uses embeded web browser.


* **Why vue.js?**

    [Vue.js](http://vuejs.org/) is providing C# to javascript binding in Neutronium application. Neutronium Core provide a generic architecture (see more [here](.\Architecture.md)) where javascript frameworj can be plugged in. This framework should provide reactivity with two way bindings and listeners patterns and should not be to opinionated in order to fit Neutronium MVVM workflow.

    First version of neutronium was only using [knockout.js](http://knockoutjs.com/) which is complient with these features.

    Then appeared [Vue.js](http://vuejs.org/) which is also has same prerequesites but quickly with a much larger community. Vue.js beyond reactivity provides great tooling, powerfull template engine, router, etc... As such Vue.js is the preferred option to develop with Neutronium.


* **How to display Window only when HTML control is rendering?**

    `HTMLViewControl` and `HTMLWindow` both have a IsHTMLLoaded property that is set to true when HTML is rendered, so it is possible to bind the window visibility to this property.

    Furthermore Neutronium.WPF provide a `BoolToHiddenConverter` that performs adequate conversion from `IsHTMLLoaded` to visibility transforming `false` into `Hidden` (standard `BooleanToVisibility` converts false into `Collapsed`).

    Ex:
```HTML

<Window x:Class="MainWindow"
        xmlns:neutronium="clr-namespace:Neutronium.WPF;assembly=Neutronium.WPF" 
...
</Window>

<Window.Resources>
    <neutronium:BoolToHiddenConverter x:Key="BoolToHidden"/>
</Window.Resources>
    
<Grid>
    <neutronium:HTMLViewControl x:Name="HtmlView" RelativeSource="View\index.html" />
</Grid>

<Window.Visibility>
    <Binding ElementName="HtmlView" Path="IsHTMLLoaded" Converter="{StaticResource BoolToHidden}"/>
</Window.Visibility>
```




*  **Is it possible to configure `same origin policy` or other browser features in a Neutronium application?**

    [See here](./Accessing_Chromium_API.md)


[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md)




