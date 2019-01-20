<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Binding Mechanism

When creating a binding the following steps are followed:

1. On UI thread, reflection is Used to create a network of objects that map the viewmodel.
2. On javascript thread, javascript object corresponding to the viewmodel are created.
3. Listeners are attached to `INotify` interfaces
4. Javascript model is used as ViewModel in the browser session and listeners are attached to track object changes. In case of knockout the original object is converted in order to ba able to used ko.observable properties.


You need to dispose the binding to remove all the events listening.

![](../images/MVVMCG.png)

[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Debug.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)