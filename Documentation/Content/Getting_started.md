# Getting started

The best way to start with Neutronium is to download template C# solution [from visual studio gallery](https://visualstudiogallery.msdn.microsoft.com/c7679997-e25b-4a79-a65f-30758fb756d8).

Template application will install nuget dependencies and scaffold a very simple solution including simple View HTML and View Model.

![solution](../images/Solution.png)

Neutronium applications are WPF application:
* App sets-up Chromium initialization and deals with clean-up on closing application
* MainWindow contains an unique UserControl: Neutronium HTMLViewControl
```HTML
<Grid>
    <WPF:HTMLViewControl IsDebug="True" RelativeSource="View\MainView.html" HorizontalAlignment="Stretch" VerticalAlignment="Stretch"/>
</Grid>
```
RelativeSource reference path to HTML.

**Important**: All files used by Neutronium such as HTML, CSS and javascript should be of type _Content_ with _Copy Always_ as copy to output property.


## Creating a binding

The HTMLViewControl will load local "View\MainView.html" HTML file and use the corresponding DataContext as a ViewModel just as standard WPF.

## To bind to the ViewModel in HTML

HTML view files treats DataContext ViewModel as the current javascript ViewModel. For example, a valid HTML view using vue would be:

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
 

Please see [Knockout](./Knockout_Binding.md) and [Vue](Vue_Binding.md) binding sections for more details.


## Example

Folder example of the Neutronium solution contains application examples.

### [Overview](./Overview.md)
