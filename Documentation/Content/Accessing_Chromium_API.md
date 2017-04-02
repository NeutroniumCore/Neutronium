<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Acessing Chromium API

Neutronium provides some hooks when you need to access some low-level Chromium API


## Chromium command line

Neutronium ChromiumFx application: `ChromiumFxWebBrowserApp` provides a `UpdateLineCommandArg` method that when overrided allow to provide custom command line switch.

This method receives a [CfxOnBeforeCommandLineProcessingEventArgs](https://chromiumfx.bitbucket.org/api/html/T_Chromium_Event_CfxOnBeforeCommandLineProcessingEventArgs.htm) instance as argument.


Exemple to disable same-origin-policy:
```CSharp
protected override void UpdateLineCommandArg(CfxOnBeforeCommandLineProcessingEventArgs beforeLineCommand)
{
    beforeLineCommand.CommandLine.AppendSwitch("disable-web-security");
}
```

See example solution [Example.ChromiumFX.Vue.SOP](https://github.com/NeutroniumCore/Neutronium/tree/master/Examples/Example.ChromiumFX.Vue.SOP) for complete example.


## Chromium settings

`ChromiumFxWebBrowserApp` provides a `UpdateChromiumSettings` method that when overrided allow to update [ChromiumFxSetting](https://chromiumfx.bitbucket.io/api/html/T_Chromium_CfxSettings.htm).


```CSharp
protected override void UpdateChromiumSettings(CfxSettings settings) 
{
    // edit settings here
}
```

[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)
