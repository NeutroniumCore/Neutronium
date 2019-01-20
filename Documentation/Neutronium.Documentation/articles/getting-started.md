# Getting started

## Install Neutronium

As described in [Installing section](.\installing.html).

## Creating a binding

HTMLViewControl and HTMLWindow uses DataContext as the current javascript ViewModel.

For example, considering a DataContext that is an instance of `Person` with:

```CSharp

public class ViewModelBase : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected void Set<T>(ref T pnv, T value, string pn)
	{
		pnv = value;
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pn));
	}
}

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
	private string _LastName;
	public string LastName
	{
		get { return _LastName; }
		set { Set(ref _LastName, value, "LastName"); }
	}

	private string _Name;
	public string Name
	{
		get { return _Name; }
		set { Set(ref _Name, value, "Name"); }
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

A valid HTML view using vue would be:
```HTML
<!doctype html>
<html>
	<head>
		<title>Vue.js Example</title>
	</head>
	<body>
		<input type="text" v-model="Name" placeholder="First name" >
		<ul>
			<li v-for="skill in Skills">
				<span>{{skill.Type}}:{{skill.Name}}</span>
				<button @click="RemoveSkill.Execute(skill)">Remove skill</button>
			</li>
		</ul>
		<div>
			<h2>{{Name}}</h2>
			<h2>{{LastName}}</h2>
		</div>
	</body>
</html>
```
 

Please see [Knockout](../binding/knockout.html) and [Vue](../binding/vue.html) binding sections for more details.
