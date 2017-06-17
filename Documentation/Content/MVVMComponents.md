# Neutronium.MVVMComponents

This assembly provides base interface and implementation that extends C# MVVM.

Neutronium.Core provides binding to all interfaces so that you can use them in javascript binding.


# `ISimpleCommand` 

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

## Implementations

Neutronium.Core provides build-in implementation for ISimpleCommand that should cover all use cases.

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

# `IResultCommand`

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


[How to set up a project](./SetUp.md) - [Overview](./Overview.md) - [Debug Tools](./Tools.md) - [Architecture](./Architecture.md) - [F.A.Q](./FAQ.md)