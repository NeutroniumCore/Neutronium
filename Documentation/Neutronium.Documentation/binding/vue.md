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
<div v-for:"skill in Skills">{{skill.Name}}</div>
```

## Bind to a Command:

Recommended way to use Neutronium is to use the [vue-cli](https://github.com/NeutroniumCore/neutronium-vue) and [vue-mixin-command](https://github.com/NeutroniumCore/neutronium-vue-command-mixin) to bind with command. But it is possible to use low level API to bind with command:

```HTML
<button @:click:"RemoveSkill.Execute(skill)"></button>
```

## Bind to a Command using built-in v-command:
```HTML
<button v-command:"AddSkill"><\button>
```
This will call Execute with null as parameter if CanExecute is true on button click.


## Bind to a Command using built-in v-command with argument:
```HTML
<button v-command:"RemoveSkill" :commandarg="skill"></button>
```
This will call Execute with skill as parameter if CanExecute is true on button click.


## Bind to a Command using built-in v-command on specific event:
```HTML
<button v-command.dblclick:"RemoveSkill" :commandarg="skill"></button>
```
This will call Execute with skill as parameter if CanExecute is true on button double click.

## Vue-cli: neutronium-vue

Neutronium provides a vue-cli template `neutronium-vue`. This is recommended tool when building mid or large scale Neutronium application. If you use it, you may pull to [next chapter](./Build_large_project_with_Vue.js_and_Webpack.md).


## Mixins:

### Register Mixins:

* For version< 1.0.0

    To add mixins to main Vue instance you can set Vue._vmMixim to an array of mixins. These mixins will be applyed by Neutronium before creating the Vue instance:

```javascript
Vue._vmMixin = [myBinding, myBinding2];
```

For example you can use this hook to add computed properties to your Vm:

```javascript
var localMixin = {
        computed: {
            count: function() {
                return this.Skills.length;
            }
        }
    };

Vue._vmMixin = [localMixin];
```

* For version>= 1.0.0
    * With Vue-cli `neutronium-vue` use `install.js` hooks: [see here for more details](../articles/large-project.html).

    * If not using vue-cli, you can use:
```javascript
window.glueHelper.setOption({mixins : [localMixin]});
``` 
This will add the corresponding mixins to vue root instance.

### Built-in mixin
Neutronium comes with some built-in plugins to simplify binding.
There are available as properties of object window.glueHelper.
If using `vue-cli`, you will rather use npm to install these mixins.

* **glueHelper.enumMixin**

This mixin allows to easily create image binding for enum.

Given the enum:
```C#
public enum Genre
{
   Male,
   Female
}
```

You need to add to the VM a property named enumImages that properties will match the enum values.
Ex:

```javascript
var localMixin = {
    data: {
        enumImages: {
           Genre: {
               Male: "images/male.png",
               Female: "images/female.png"
           }
        }
   }
};

//Register enumMixin and localMixin
Vue._vmMixin = [localMixin, glueHelper.enumMixin];

//or with neutronium version>= 1.0.0
window.glueHelper.setOption({mixins : [localMixin, glueHelper.enumMixin]});
```

Then you can use the **enumImage** method that **glueHelper.enumMixin** will add to the vue instance:
HTML
```HTML
<img height="50" width="50" :src="enumImage(genre)">
```

* **Command mixin**

When using `vue-cli`, you should import the npm module: [neutronium-vue-command-mixin](https://github.com/NeutroniumCore/neutronium-vue-command-mixin). 

Otherwise command-mixin is available through `glueHelper.commandMixin`.

This mixin is used to create component based on command.
It defines two props command and arg:

```javascript
command:{
    type: Object,
    default: null
},
arg: {
    type: Object,
    default: null
}
```

Define self-explanatory method : **execute**

and self-explanatory computed: **canExecute**


If you implement **beforeCommand** method, it will be called before the command Execute 

Here is an example of usage (vue-cli context):

Template
```HTML
<template>
  <div class="ui button" :class="{'disabled': !canExecute}" @click="execute">   
    <slot></slot>  
  </div>
</template>
```

javascript:
```javascript
import commandMixin from 'neutronium-vue-command-mixin'
export default {
  mixins:[commandMixin]
}
```

### Advanced: `IResultCommand` support

Neutronium provides binding to [IResultCommand](./MVVMComponents.md#iresultcommand) making possible to call a C# function returning a Task from javascript and receiving the response as a promise.

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
