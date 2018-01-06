<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Debug

## 1. Interactive Debug

### To activate Debug mode for `HTLMLWindow` or `HTMLViewControl` set IsDebug as true

In debug mode, the following context menu will be displayed:

![debug buttons](../images/Tools/ContextMenu.png)


### 1.1 _Inspect_ 

Opens a fully featured chromium javascript debug windows

![inspect](../images/Tools/ContextMenu-inspect.png)

![inspect](../images/DebugInspect.png)


### 1.2  _Vm Debug_ 

Opens a javascript framework specific window to display information about the binding:

![InspectVM](../images/Tools/ContextMenu-Vm-debug.png)

- For Vue.js

The [vue chrome debug tool](https://github.com/vuejs/vue-devtools) is opened in new window

![vue-devtools](../images/Tools/VueTools2.png)

![vue-devtools](../images/Tools/VueTools.png)


- For Knockout

An adaption of [knockout-view] a debug tool [displaying ViewModel is used](https://github.com/jmeas/knockout-view)

### 1.3 _Save Vm_

![SaveVM](../images/Tools/ContextMenu-Save-vm.png)

Allows to save the value of the bound DataContext to a circular JSON (`.cjson`). This is a very interesting feature when coupled with [neutronium vue webpack template](./Build_large_project_with_Vue.js_and_Webpack.md). Indeed neutronium Webpack configuration can use this files as "fake" ViewModel. 

This is very powerful when coupled with live reload feature as saving a ViewModel will update the corresponding browser when using `npm run dev`.

### 1.4 _About_ 
Opens a windows displaying information about Neutronium configuration:

![About](../images/Tools/ContextMenu-About.png)

![About](../images/about-64-bits.png)


## 2. Trace 

By default, Neutronium will use the trace listener to log events. 
Neutronium will log binding errors as well as console.log message from the HTML session. 

If you need to use a different logger to output the Neutronium events you can implement your own IWebSessionLogger:


```CSharp
    public interface IWebSessionLogger
    {
        /// <summary>
        /// called for debug logging
        /// </summary>
        void Debug(Func<string> information);

        /// <summary>
        /// called for debug logging
        /// </summary>
        void Debug(string information);

        /// <summary>
        /// called for information logging
        /// </summary>
        void Info(string information);

        /// <summary>
        /// called for information logging 
        /// </summary>
        void Info(Func<string> information);

        /// <summary>
        /// called on critical event 
        /// </summary>
        void Error(string information);

        /// <summary>
        /// called on critical event 
        /// </summary>
        void Error(Func<string> information);

        /// <summary>
        /// called on each console log called by browser 
        /// </summary>
        void LogBrowser(ConsoleMessageArgs iInformation, Uri url);

        /// <summary>
        /// called in case of browser critical error
        /// </summary>
        void WebBrowserError(Exception exception, Action cancel);
    }
```

And then setting Neutronium Engine session logger:

```CSharp
var myLogger = new MyLogger();
HTMLEngineFactory.Engine.WebSessionLogger = myLogger;

```

### [Binding in Depth](./Binding-in-Depth)

[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)
