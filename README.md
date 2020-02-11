# NSDanmaku
一个简单的UWP 弹幕引擎

## 截图
![pic0.png](https://i.loli.net/2018/04/11/5ace0c62dba1d.png)

## 快速入门

1、添加nuget引用
`Install-Package NSDanmaku`

2、XAML
```
 ...
 xmlns:controls="using:NSDanmaku.Controls"
 ...
 <Grid Background="Gray">
    <controls:Danmaku x:Name="danmaku"></controls:Danmaku>

 </Grid>
```

3、添加弹幕
```
//添加滚动弹幕
danmaku.AddRollDanmu(...);
//添加顶部弹幕
danmaku.AddTopDanmu(...);
//添加底部弹幕
danmaku.AddBottomDanmu(...);

```