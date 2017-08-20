<p align="center"><img <p align="center"><img width="100"src="../../Deploy/logo.png"></p>

# Using pack uri 

## Step by step

1. Change html assets `Build Action` from `Content` to `Resource`.

<p align="center"><img <p align="center"><img src="../packuri_1.png"></p>

2. Change HtmlViewControl's `RelativeSource` property to `Uri` with pack uri.

 ```xml
<Grid>
    <wpf:HTMLViewControl RelativeSource="View/Main/dist/index.html" IsDebug="true" JavascriptUIEngine="VueInjectorV2" x:Name="wcBrowser"  HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
</Grid>
 ```
To
```xml
<Grid>
    <wpf:HTMLViewControl Uri="pack://application:,,,/View/Main/dist/index.html" IsDebug="true" JavascriptUIEngine="VueInjectorV2" x:Name="wcBrowser"  HorizontalAlignment="Stretch" VerticalAlignment="Stretch" />
</Grid>
 ```