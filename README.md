<p align="center"><img width="100"src="https://raw.githubusercontent.com/David-Desmaisons/Neutronium/master/Deploy/logo.png"></p>
<h1 align="center">Neutronium</h1>

[![Neutronium.ChromiumFx.Vue](http://img.shields.io/nuget/v/Neutronium.ChromiumFx.Vue.svg?style=flat)](https://www.nuget.org/packages/Neutronium.ChromiumFx.Vue//)
[![Neutronium.ChromiumFx.Vue](https://img.shields.io/github/license/mashape/apistatus.svg)](https://www.nuget.org/packages/Neutronium.ChromiumFx.Vue//)

Description
--------------
Neutronium is an MVVM HTLM View engine for C# application.
HTML view infra relies on CEFGlue framework which is a C# glue for Chromium Embedded Framework(CEF).
Neutronium abstracts all the heavy work of managing C# and javascript integration (including Thread management, type conversion) by providing a standard MVMM binding.
iThis allows to reuse 100% of View Model written for WPF in a HTML UI(provided you implement correctly the INotifyPropertyChanged, INotifyCollectionChanged and ICommand patterns).  
On the javascript side, C# objects are converted to javascript objects and can be bound using knockout.js  library.  
All changes between View and ViewModel are propagate back and forth by using knockout.js subscribers and C# events.
C# commands are mapped to javascript method and can be bind to click event using knockout.  
MVVM for CEF(Glue) is shipped with some custom knockout binding to help with standard MVMM binding such as commands.
This library can be used without having to write any javascript on your own as it will take care of all the mapping and plumbing for you!

More Advanced functionalities include navigation engine (based on association between HTML and ViewModel type) and possibility to call C# from javascript with a result value asynchronously using promises.

This project is a port of [MVVM-for-awesomium.](https://github.com/David-Desmaisons/MVVM-for-awesomium/)

Nuget package
-------------

[ChromiumFx broswer and Vue.js](https://www.nuget.org/packages/Neutronium.ChromiumFx.Vue/)

Usage - Example
--------------

**ViewModel (C#)**

		public class ViewModelBase : INotifyPropertyChanged
		{
			protected void Set<T>(ref T ipnv, T value, string ipn)
			{
				if (object.Equals(ipnv, value))
					return;
				ipnv = value;
				OnPropertyChanged(ipn);
			}

			private void OnPropertyChanged(string pn)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(pn));
			}

			public event PropertyChangedEventHandler PropertyChanged;
		}

		public class Skill : ViewModelBase
		{
			private string _Type;
			public string Type
			{
				get { return _Type; }
				set { Set(ref _Type, value, "Type"); }
			}

			private string _Name;
			public string Name
			{
				get { return _Name; }
				set { Set(ref _Name, value, "Name"); }
			}
		}

		public class Person: ViewModelBase
		{
			public Person()
			{
				Skills = new ObservableCollection<Skill>();
				RemoveSkill = new RelayCommand<Skill>(s=> this.Skills.Remove(s));
			}
		  
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
		}
		
		
**View (HTML) using knockout mark-up**

	<!doctype html>
	<html>
		<head>
			<title></title>
			<script src="js/knockout.js" type="text/javascript"></script>
			<script src="js/Ko_Extension.js" type="text/javascript"></script>
		</head>
		<body>
			<input type="text" data-bind="value: Name, valueUpdate:'afterkeydown'" placeholder="First name" >
			<ul data-bind="foreach: Skills">
				<li><span data-bind="text:Type"></span>:<span data-bind="text:Name"></span>
				<button data-bind="command: $root.RemoveSkill">Remove skill</button></li>
			</ul>
			<div>
				<h2><span data-bind="text: Name"></span></h2>
				<h2><span data-bind="text: LastName"></span></h2>
			</div>

			<button data-bind="command: ChangeSkill">Click me</button>
		</body>
	</html>

	
**Create the component(C# Xaml)**

	<MVVM.CEFGlue:HTMLViewControl RelativeSource="src\index.html" />

The binding is done on the DataContext property just as standard WPF,
That's it!

[Documentation (wiki) here.](https://github.com/David-Desmaisons/MVVM.CEF.Glue/wiki/)
