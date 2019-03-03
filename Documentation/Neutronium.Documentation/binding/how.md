<a href="https://github.com/NeutroniumCore/Neutronium" target="_blank">
  <img
    style="position: fixed; top: 0; right: 0; border: 0; z-index:99999"
    width="149"
    height="149"
    src="https://github.blog/wp-content/uploads/2008/12/forkme_right_gray_6d6d6d.png?resize=149%2C149"
    class="attachment-full size-full"
    alt="Fork me on GitHub"
    data-recalc-dims="1"
  />
</a>

# Binding internals

## Main mechanism

When creating a binding the following steps are followed:

1. On UI thread, reflection is Used to create a network of objects that map the viewModel.
2. On javascript thread, javascript object corresponding to the viewModel are created.
3. Listeners are attached to `INotify` interfaces
4. Javascript model is passed to javascript framework adapter. They are in charge for using them as viewModel in the browser session and creating listeners to track object changes. In case of knockout the original object is converted in order to ba able to used ko.observable properties.

5. Events on the javascript side are sent back on the C# side to update the viewModel. Events on the C# side are sent back on the javascript side to update the viewModel.

6. It is needed to dispose the binding to remove all the events listening.

## Optimizations

### 1. Javascript object creation
  Since version 1.2.0, different strategies are used to convert C# object to javascript depending on the size of the object to convert:

* For large objects:<br/>
Javascript objects are created in batches to avoid too many IO due to inter-process communication needed by Chromium architecture.<br/>
For example, value objects are created in on time using `eval` API.

* For small objects:<br/>
Embedded browser API are used for object creation.

### 2. Time slice collecting
Implemented in version 1.4.0
#### Context
In a MVVM application, it is possible that the back-end dispatches updates at very high frequency.
If this update rate crosses a certain limit, it might not be the best solution to rebuilt the UI for each changes in a synchronous manner. Because:

It is useless displaying or trying to display changes at a rate that user can not perceive or that hardware can not support (60fps being the upper limit) (See google RAIL for perceptual consideration).
Even if your UI solution is fast, updating the UI comes always with a CPU/time a cost . In a very high frequency context, UI refreshing can cause delay/freeze in the UI thread.
In these conditions, alternatives strategies may be a good idea to update UI.
See this blog post for similar consideration: Pull vs. Push models for UI updates proposing times based solution.

#### Solution: Time slice collecting

On UI thread, update request coming from INotifyPropertyChanged, INotifyCollectionChanged, or ICommand.CanExecuteChanged is added on the
changes queue.
If this is not already the case, an update is scheduled to run when on UI thread next idle time (using Dispatcher Dispatch with a priority of DataBind)
Potentially repeat 1)
Replay the changes added to the queue in the same order.
This approach has the following advantages:

If the same property has been changed over and over during changes collection, only one change will be performed during replay.
Updating UI in a V8 context means performing some changes on the UI thread and some changes on the V8 javascript context. Replaying the changes allow to switch context only twice for all changes instead of twice for each changes.
-Simplicity, using idle time is simpler than using timers to throttle or debounce changes.


