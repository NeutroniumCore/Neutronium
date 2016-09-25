<p align="center"><img width="100"src="https://raw.githubusercontent.com/David-Desmaisons/Neutronium/master/Deploy/logo.png"></p>
<h1 align="center">Neutronium</h1>

[![MIT License](https://img.shields.io/appveyor/ci/David-Desmaisons/Neutronium.svg?maxAge=2592000)](https://ci.appveyor.com/project/David-Desmaisons/neutronium)
[![NuGet Badge](https://buildstats.info/nuget/Neutronium.Core)](https://www.nuget.org/packages/Neutronium.Core/)
[![MIT License](https://img.shields.io/github/license/David-Desmaisons/Neutronium.svg)](https://github.com/David-Desmaisons/Neutronium/blob/master/LICENSE)

What is Neutronium?
-------------------

* Neutronium is a library to create **.NET desktop applications** using **HTML, CSS** and **javascript**.

* Neutronium uses **[MVVM pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel)** exactly the same way as WPF application.

* Neutronium provides bindings with **Vue.js** and **Knockout.js** to build powerfull HTML5 UI.

Why Neutronium?
---------------

* Use all the **power of the javascript stack** to build .NET desktop applications.

* **Easy to use**: 

 * **Focus on ViewModel logic** and abstract away from complex context, process and binding management you have to deal with other lower level embeded WebBrowser solutions available for .NET such as [Awesomium](http://www.awesomium.com/), [CefGlue](http://xilium.bitbucket.org/cefglue/), [CefSharp](https://github.com/cefsharp/CefSharp) and [ChromiumFx](https://bitbucket.org/chromiumfx/chromiumfx)
 * Architecture Neutronium application **just like standard WPF** application.
 * Use **standard javascript frameworks** to build UI

* **Easy to set-up**:
 * **[Solution template](https://visualstudiogallery.msdn.microsoft.com/c7679997-e25b-4a79-a65f-30758fb756d8)** available for a quick start

* **Reuse** ViewModel designed for WPF with a different View Engine.

* Build UI on a 100% **Open Source Stack**
 
Main features
-------------
 * Reactive
  * **Two way-binding** beetween view and viewmodel, including **command** binding
 
 * **Pluggable architecture**:  
  * Easily plug-in new javascript frameworks or even embedded browser.
 
How?
----

* Neutronium combines [Chromium](https://www.chromium.org) via [ChromiumFx C# lib](https://bitbucket.org/chromiumfx/chromiumfx) and a binding engine that converts back and forth C# POCO to javascript POCO.
* Javascript objects are then used as ViewModel for javascript MVVM library such as [knockout.js]("http://knockoutjs.com/) or [Vue.js](http://vuejs.org/images/logo.png).
* Listeners are set-up on C# and javscript side for two-way binding.
   
On the shoulders of giants
--------------------------
 
 <p align="center">
 <a href="https://www.chromium.org" >
 <img height="60" src="https://www.chromium.org/_/rsrc/1438879449147/config/customLogo.gif?revision=3">
 </a>
 <a href="https://bitbucket.org/chromiumembedded/cef">
 <img height="70"src="https://bitbucket-assetroot.s3.amazonaws.com/c/photos/2015/Mar/14/3042045877-1-cef-logo_avatar.png">
 </a>
 <a href="http://vuejs.org/">
 <img height="70" style="margin-top: 10px;" src="http://vuejs.org/images/logo.png">
 </a>
 <a href="http://knockoutjs.com/">
 <img height="70" style="margin-top: 10px;" src="http://knockoutjs.com/img/ko-logo.png">
 </a>
 </p>

Usage - Example
--------------

**ViewModel (C#)**

```C#
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
		
**View (HTML)**
* **First option: use Vue.js**
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

* **Alternativelly use knockout.js**

```HTML
<!doctype html>
<html>
	<head>
		<title>knockout.js Example</title>
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
	</body>
</html>
```
	
**Create the component(C# Xaml)**

```XAML
<Neutronium:HTMLViewControl RelativeSource="src\index.html" />
```

The binding is done on the DataContext property just as standard WPF,
That's it!

Get started
----------

Best way to start with Neutronium is to download template C# solution [from visual studio gallery](https://visualstudiogallery.msdn.microsoft.com/c7679997-e25b-4a79-a65f-30758fb756d8).

[Documentation (wiki) here.](https://github.com/David-Desmaisons/Neutronium/wiki/)


Comparaison with other libraries:
---------------------------------
* [Electron](http://electron.atom.io/)

	Neutronium is electron for .NET? Well, kind of. Neutronium however is a higher abstraction so that you don't need to care about Chromium implementation such as renderer or browser processes.

* [Awesomium](http://www.awesomium.com/)

	Different from other libraries Awesomium is not open source. Last update was embedding Chrome 19 so it is pretty out of date. One neutronium distribution offer Awesomium as WebBrowser.

* [CefGlue](http://xilium.bitbucket.org/cefglue/), [ChromiumFx](https://bitbucket.org/chromiumfx/chromiumfx), [CefSharp](https://github.com/cefsharp/CefSharp)

	All are open source libraries presenting up-to-date C# binding for [CEF](https://bitbucket.org/chromiumembedded/cef)
	
 * [CefGlue](http://xilium.bitbucket.org/cefglue/)
 
	Offers all API of CEF. Used by Neutronium as a test WebBrowser using the monoprocess option.

 * [ChromiumFx](https://bitbucket.org/chromiumfx/chromiumfx)
 
	Same as CefGlue + remote API that handles comunication between Chromium processeses. Neutronium recomended set-up uses ChromiumFx as a WebBrowser.

 * [CefSharp](https://github.com/cefsharp/CefSharp)
 
	Well documented and package solution (including nuget). Does not offer all CEF binding to javascript however.
		
Nuget packages
--------------

[ChromiumFx browser and Vue.js](https://www.nuget.org/packages/Neutronium.ChromiumFx.Vue/)

[ChromiumFx browser and knockout.js](https://www.nuget.org/packages/Neutronium.ChromiumFx.Knockout/)

This project is a continuation and improvement of [MVVM-for-awesomium.](https://github.com/David-Desmaisons/MVVM-for-awesomium/)
