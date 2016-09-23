<p align="center"><img width="100"src="https://raw.githubusercontent.com/David-Desmaisons/Neutronium/master/Deploy/logo.png"></p>
<h1 align="center">Neutronium</h1>

[![MIT License](https://img.shields.io/appveyor/ci/David-Desmaisons/Neutronium.svg?maxAge=2592000)](https://ci.appveyor.com/project/David-Desmaisons/neutronium)
[![NuGet Badge](https://buildstats.info/nuget/Neutronium.Core)](https://www.nuget.org/packages/Neutronium.Core/)
[![MIT License](https://img.shields.io/github/license/David-Desmaisons/Neutronium.svg)](https://github.com/David-Desmaisons/Neutronium/blob/master/LICENSE)

What is Neutronium ?
--------------------

* Neutronium is a framework to create **.NET desktop applications** using **HTML, CSS** and **javascript**.

* Neutronium uses **[MVVM pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel)** the same way as classic WPF application.

* Neutronium provides bindings with **Vue.js** and **Knockout.js** to build powerfull HTML5 application.

Why Neutronium ?
----------------

* Use all the **power of the javascript stack** to build .NET desktop applications.

* **Easy to use**: 

 * **Focus on ViewModel logic** and abstract away from complex context, process and binding management you have to deal with other lower level embeded WebBrowser solutions available for .NET such as [Awesomium](http://www.awesomium.com/), [CefGlue](http://xilium.bitbucket.org/cefglue/), [CefSharp](https://github.com/cefsharp/CefSharp) and [ChromiumFx](https://bitbucket.org/chromiumfx/chromiumfx)
 * Architecture Neutronium application **just like standard WPF** application.
 * **[Solution template](https://visualstudiogallery.msdn.microsoft.com/c7679997-e25b-4a79-a65f-30758fb756d8)** available for a quick start

* **Reuse** ViewModel designed for WPF with a different View Engine.

Get started
----------

Best way to start with Neutronium is to download template C# solution [from visual studio gallery](https://visualstudiogallery.msdn.microsoft.com/c7679997-e25b-4a79-a65f-30758fb756d8).

Nuget packages
--------------

[ChromiumFx broswer and Vue.js](https://www.nuget.org/packages/Neutronium.ChromiumFx.Vue/)

[ChromiumFx broswer and knockout.js](https://www.nuget.org/packages/Neutronium.ChromiumFx.Knockout/)

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

This project is a continuation and improvement of [MVVM-for-awesomium.](https://github.com/David-Desmaisons/MVVM-for-awesomium/)
