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
	- Overlap：两个TextBlock重叠，模拟描边
	- WithoutStroke：无描边，单TextBlock
	- Stroke：描边

## 2.1.0修改
- 移除Microsoft.Toolkit.Uwp.UI.Controls

- 移除DanmakuStyle命名，移除Shadow样式

- 优化描边弹幕DPI

- 防止弹幕超出边界

- 升级引用的包版本

## 参考资料
[https://www.zhihu.com/question/370464345](https://www.zhihu.com/question/370464345)

[https://zhuanlan.zhihu.com/p/159027974](https://zhuanlan.zhihu.com/p/159027974)
