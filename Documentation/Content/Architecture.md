<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Architecture

## Overview

Neutronium presents a layered, puggable arquitecture:

![](../images/Architecture.png)


* `Neutronium.Core` provides core binding, synchronization and javascript manipulation being abstracted from real web browser implementation by an abstraction layer. This allows Neutronium to support various Web browser.

* `Neutronium.WPF` provides WPF user controls as well as any WPF dependent features.

* `WebBrowserEngines`:

    * `Neutronium.WebBrowserEngine.ChromiumFx` provides binding to ChromiumFx which the recommended web browser to run Neutronium.

    * `Neutronium.WebBrowserEngine.CefGlue` provides binding to CefGlue which the  web browser mainly used for automated tests.

* `JavascriptFrameworks`:

    * `Neutronium.JavascriptFrameworks.Vue` provides Vue.js binding and any vue specific feature.

    * `Neutronium.JavascriptFrameworks.knockout` provides knockout.js binding and any vue specific feature.

* `Neutronium.MVVMComponents` provides additional abstraction to MVVM interfaces such as:

```CSharp
public interface IResultCommand
{
    Task<object> Execute(object iargument);
}
```

```CSharp
public interface ISimpleCommand
{
    void Execute(object argument);
}
```
`Neutonium.Core` provides binding to `Neutronium.MVVMComponents` interfaces 


## Nuget packages

To simplify deployment Neutronium provide two main alternative packages:

* [Neutronium.ChromiumFx.Vue](https://www.nuget.org/packages/Neutronium.ChromiumFx.Vue/)
* [Neutronium.ChromiumFx.Knockout](https://www.nuget.org/packages/Neutronium.ChromiumFx.Knockout/)

Both includes all dependencies nedded to run a Neutronium project including ChromiumFx web browser binding and vue or knockut framework.

[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Tools.md) - [F.A.Q](./FAQ.md)



