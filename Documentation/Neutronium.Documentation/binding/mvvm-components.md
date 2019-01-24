# Neutronium.MVVMComponents

This assembly provides base interface and implementation that extends C# MVVM.

Neutronium.Core provides binding to all interfaces so that you can use them in javascript binding.


## `ISimpleCommand` 

`ISimpleCommand` is comparable to a `ICommand` that can always be executed. 


```CSharp
public interface ISimpleCommand
{
    /// <summary>
    /// Execute the command with the corresponding argument
    /// </summary>
    /// <param name="argument"></param>
    void Execute(object argument);
}
```

**In version 1.0.0:**

`ISimpleCommand` present the following changes.

```CSharp
public interface ISimpleCommand
{
    /// <summary>
    /// Execute the command with no argument
    /// </summary>
    void Execute();
}
```

If you need to pass argument to the `execute` method use the new `ISimpleCommand<T>`.


## `ISimpleCommand<T>`

`ISimpleCommand<T>` is the typed version of `ISimpleCommand`. 


```CSharp
public interface ISimpleCommand<T>
{
    /// <summary>
    /// Execute the command with the corresponding argument
    /// </summary>
    /// <param name="argument"></param>
    void Execute(T argument);
}
```

## `ICommand<T>` 

`ICommand<T>` is comparable to a typed `ICommand`. 


```CSharp
public interface ICommand<in T> : IUpdatableCommand
{
    /// <summary>
    /// Execute the command with the corresponding argument
    /// </summary>
    /// <param name="argument"></param>
    void Execute(T argument);

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current
    /// state.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. If the command does not require data to be passed,
    /// this object can be set to null.
    /// </param>
    /// <returns>
    /// true if this command can be executed; otherwise, false.
    /// </returns>
    bool CanExecute(T parameter);
}
```

## `ICommandWithoutParameter` 

`ICommandWithoutParameter` is comparable to a `ICommand` that does not need any argument. 


```CSharp
public interface ICommandWithoutParameter : IUpdatableCommand
{
    /// <summary>
    /// Execute the command
    /// </summary>
    void Execute();

    /// <summary>
    ///  Determines whether the command can execute in its current
    ///  state.
    /// </summary>
    /// <returns>
    /// true if this command can be executed; otherwise, false.
    /// </returns>
    bool CanExecute { get; }
}
```

# Implementations

Neutronium.MVVMComponents provides build-in implementation for ISimpleCommand that should cover all use cases.
Starting with version 1.0.0. `RelaySimpleCommand` and `RelaySimpleCommand<T>` also implements ICommand so that they can be used in Neutronium as well as traditional WPF application.


### `RelaySimpleCommand`

```CSharp
public class RelaySimpleCommand : ISimpleCommand
{
    private readonly Action _Do;

    public RelaySimpleCommand(Action doAction)
    {
        _Do = doAction;
    }

    public void Execute(object argument)
    {
        _Do();
    }
}
```

### `RelaySimpleCommand<T>`

```CSharp
public class RelaySimpleCommand<T> : ISimpleCommand where T:class
{
    private readonly Action<T> _Do;

    public RelaySimpleCommand(Action<T> doAction)
    {
        _Do = doAction;
    }

    public void Execute(object argument)
    {
        _Do(argument as T);
    }
}
```

**Starting from version 1.0.**

### `RelayTaskCommand` and `RelayToogleCommand`

Provides implementation for both `ICommand` and `ICommandWithoutParameter`

### `RelayTaskCommand<T>` and `RelayToogleCommand<T>`

Provides implementation for both `ICommand<T>` and `ICommand`

## `IResultCommand`

`IResultCommand` is comparable to a command that would allow to asynchronously return a result.

```CSharp
public interface IResultCommand
{
    /// <summary>
    /// return asynchronously a result given a specific argument
    /// </summary>
    /// <param name="argument"></param>
    /// <returns></returns>
    Task<object> Execute(object argument);
}
```

Neutronium.Core provides build-in implementation and factory to create IResultCommand that should cover all most cases.

## Factory

### `RelayResultCommand` 
`RelayResultCommand`  is a static factory that creates `IResultCommand` from synchronous function.

```CSharp
public static class RelayResultCommand
{
    /// <summary>
    /// Create a IResultCommand from given function
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TResult">
    /// </typeparam>
    /// <param name="function">
    /// </param>
    /// <returns></returns>
    public static IResultCommand Create<TIn, TResult>(Func<TIn, TResult> function)

    /// <summary>
    /// Create a IResultCommand from given function
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="function"></param>
    /// <returns></returns>
    public static IResultCommand Create<TResult>(Func<TResult> function)
}
```

## Implementations

### `RelayResultCommand<TIn, TResult>`

Create a IResultCommand from a Func<TIn, Task<TResult>> where TIn is the argument type

```CSharp
public RelayResultCommand(Func<TIn, Task<TResult>> resultBuilder)
```