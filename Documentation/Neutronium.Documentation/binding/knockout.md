# Knockout binding 

See complete example at: [Example.ChromiumFX.Ko.UI](https://github.com/NeutroniumCore/Neutronium/tree/master/Examples/Example.ChromiumFX.Ko.UI)

Knockout binding is provided by the ```Neutronium.JavascriptFramework.Knockout``` assembly that exposes the injector:<br>
```KnockoutFrameworkManager```   registered name **KnockoutInjector**<br>

Given the following ViewModel:

```CSharp
public class Skill
{
	public string Type { get;}
	public string Name { get;}

	public Skill (string name, string skillType)
	{
		Name = name;
		Type = skillType;
	}
}

public class Person: ViewModelBase
{
	private string _Name;
	public string Name
	{
		get { return _Name; }
		set { Set(ref _Name, value, "Name"); }
	}

	private Skill _MainSkill;
	public Skill MainSkill
	{
		get { return _MainSkill; }
		set { Set(ref _MainSkill, value, "MainSkill"); }
	}
	   
	public IList<Skill> Skills { get; private set; }

	public ICommand RemoveSkill { get; private set; }
	
	public Person()
	{
		Skills = new ObservableCollection<Skill>();
		RemoveSkill = new RelayCommand<Skill>(s=> this.Skills.Remove(s));
	}	  
}
```
## Bind to a property:
```HTML
<span data-bind="text:Name"></span>
```

## Bind to a nested property:
```HTML
<span data-bind="Skill().Name"></span>
```

## Bind to a Collection:
```HTML
<div data-bind="foreach: Skills()">
     <div ><span data-bind="text:Type"></span></div >
</div>
```
Note that you need the parenthesis as arrays are mapped to observable value which value is an observable array.

## Bind to a Command with custom binding :
```HTML
<button data-bind="command: $data.RemoveSkill"></button>
```
By default the command will be called if CanExecute is true on click with element context as command argument.


## Bind to a Command with custom binding :
```HTML
<button data-bind="command: $data.RemoveSkill"></button>
```
The command will be called if CanExecute is true on click with element context as command argument.


## Bind to a Command with custom binding on different event:
```HTML
<button data-bind="command: $data.RemoveSkill, commandoption: { event:'dblclick'}">
</button>
```
This will call Execute if CanExecute is true with element context as parameter on button double click.

## Special Hook

It is possible to create a register method on the global ko object.
This method should take one argument which is the ViewModel.
It will called before calling ko.applyBindings, so it can be used to register computed properties.

Example:

```javascript
ko.register = function (vm) {
   vm.count = ko.computed( function() {
       return this.Skills().length;
   }, vm);
}
```