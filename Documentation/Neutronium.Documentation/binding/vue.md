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

# Vue binding 

## Important:

Vue binding will be automatically applyed inside an HTML element with the main id.<br>
So all HTML depending on vue should be wrapped inside a main div:

```HTML
<div id="main">
   <!-- Your vue logic here -->
</div>
```

See Complete Example: [Example.ChromiumFX.Vue.UI](https://github.com/NeutroniumCore/Neutronium/tree/master/Examples/Example.ChromiumFX.Vue.UI)


Vue binding is provided by the ```Neutronium.JavascriptFramework.Vue``` assembly that exposes the following injectors:<br>
**For Vue.js 1.0**: ```VueSessionInjector```   registered name **VueInjector**<br>
**For Vue.js 2.0**: ```VueSessionInjectorV2``` registered name **VueInjectorV2**<br>

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
 	public ICommand AddSkill { get; private set; }
	
	public Person()
	{
		Skills = new ObservableCollection<Skill>();
		RemoveSkill = new RelayCommand<Skill>(s=> this.Skills.Remove(s));
		AddSkill = new RelayCommand(_ => this.Skills.Add(new Skill("Vue.js", "javascript"));
	}	  
}
```

## Bind to a property:
```HTML
<span>{{Name}}</span>
```

## Bind to a nested property:
```HTML
<span>{{Skill.Name}}</span>
```

## Bind to a Collection:
```HTML
<div v-for:"skill in Skills" :key="skill.Name">{{skill.Name}}</div>
```

## Bind to a Command:

Recommended way to use Neutronium is to use the [vue-cli](https://github.com/NeutroniumCore/neutronium-vue) and [vue-mixin-command](https://github.com/NeutroniumCore/neutronium-vue-command-mixin) to bind with command. But it is possible to use low level API to bind with command:

```HTML
<button @:click:"RemoveSkill.Execute(skill)"></button>
```

## Using command mixin:

When using the vue-cli 3 plugin template [`vue-cli-plugin-neutronium`](https://github.com/NeutroniumCore/vue-cli-plugin-neutronium) which is the recommended tool when building mid or large scale Neutronium application ([reference here](../articles/large-project.html)).<br/>

### Install:

```bash
npm install --save neutronium-vue-command-mixin
```
### Usage
Provide mixin to easily integrate ICommand in vue.js using Neutronium.
Component this mixin exposes:

### Props
* `command`<br>
Type: `Object`<br>
Required: `true`<br>
The property corresponding to the C# command.

* `arg`<br>
Type: `Object`<br>
Required: `false`<br>
The argument that will be passed to comand when execute is called.

### Computed
* `canExecute`<br>
Type: `Boolean`<br>
true if Command CanExecute is true.

### Method
* `execute`<br>
Call the corresponding command with the argument `arg`<br>

### Events
* `beforeExecute`<br>
Called before calling command execute if CanExecute is true. <br>
The event argument provides two properties: <br>
  * `arg`: `Object` the command argument, 
  * `cancel`: `false` set it to true to cancel the execution

* `afterExecute`<br>
Called after calling command execute.<br>
The event argument is the command argument.<br>

### Example
Declaring buttonCommand component in a .vue file (using semantic ui):
 
```HTML
<template>
  <div class="ui button" :class="{'disabled': !canExecute}" @click="execute">   
    <slot></slot>  
  </div>
</template>
<script>
import comandMixin from 'neutronium-vue-command-mixin'

export default {
  mixins:[comandMixin]
}
</script>
```

Using buttonCommand:

```HTML
<button-command :command="compute" :arg="argument">
Submit
</button-command> 
```

### Advanced: `IResultCommand` support

Neutronium provides binding to [IResultCommand](./mvvm-components.html#iresultcommand) making possible to call a C# function returning a Task from javascript and receiving the response as a promise.

Npm module [neutronium-vue-resultcommand-topromise](https://github.com/NeutroniumCore/neutronium-vue-resultcommand-topromise) is an helper to obtain promise from resultCommand on the javascript side.

Example:

 
 To bind to C# ResultCommand property:
 ```CSharp
 public class ViewModel
 {
     public IResultCommand ResultCommand {get;} 
     
     public ViewModel()
     {
         ResultCommand = RelayResultCommand.Create<string, int>(Count);
     }

     private int? Count(string routeName)
     {
        return routeName?.Lenght;
     }
 }
 ```
 
 Do on javascript side:
```javascript
import {toPromise} from 'neutronium-vue-resultcommand-topromise'

const promise = toPromise(viewModel.ResultCommand, 'countLetterNumber');
promise.then((ok)=>{
     //Ok code
 }, (error) =>{
 //Error handling
})
```
