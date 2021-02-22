# NSDanmaku
一个简单的UWP弹幕控件

## 截图
![pic0.png](https://i.loli.net/2021/02/22/taBKgwvI3RymJ8V.png)

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
//添加弹幕
danmaku.AddDanmu(...);
//添加滚动弹幕
danmaku.AddRollDanmu(...);
//添加顶部弹幕
danmaku.AddTopDanmu(...);
//添加底部弹幕
danmaku.AddBottomDanmu(...);
```
## 常用方法

```
//暂停弹幕
danmaku.PauseDanmaku();
//继续弹幕
danmaku.ResumeDanmaku();
//移除指定的弹幕
danmaku.Remove(...);
//移除屏幕上的弹幕
danmaku.ClearAll(...);
//读取屏幕上的弹幕
danmaku.GetDanmakus(...);
//隐藏指定位置的弹幕
danmaku.HideDanmaku(...);
//显示指定位置的弹幕
danmaku.ShowDanmaku(...);
```


## 属性
- `DanmakuBold` 弹幕是否加粗

- `DanmakuSizeZoom` 字体大小缩放倍数

- `DanmakuDuration` 滚动弹幕动画持续时间,单位:秒,越小弹幕移动速度越快

- `DanmakuFontFamily` 弹幕字体名称

- `DanmakuStyle` 弹幕样式，可选以下四种样式：
	- Default：两个TextBlock重叠
	- NoBorder：无边框，单TextBlock
	- Shadow：阴影
	- Border：边框

## 参考资料
[https://www.zhihu.com/question/370464345](https://www.zhihu.com/question/370464345)

[https://zhuanlan.zhihu.com/p/159027974](https://zhuanlan.zhihu.com/p/159027974)
