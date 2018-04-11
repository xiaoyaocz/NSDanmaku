using Microsoft.Toolkit.Uwp.UI.Controls;
using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace NSDanmaku.Controls
{
    public sealed partial class Danmaku : UserControl
    {
        public Danmaku()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 字体大小缩放，电脑推荐默认1.0，手机推荐0.5
        /// </summary>
        public double sizeZoom = 1.0;
        /// <summary>
        /// 透明度，0.1-1.0
        /// </summary>
        public double transparent = 1.0;
        /// <summary>
        /// 滚动弹幕速度
        /// </summary>
        public int speed = 10;
        /// <summary>
        /// 弹幕样式
        /// </summary>
        public DanmakuBorderStyle borderStyle = DanmakuBorderStyle.Shadow;
        /// <summary>
        /// 弹幕动画管理
        /// </summary>
        List<Storyboard> storyList = new List<Storyboard>();
        /// <summary>
        /// 弹幕模式
        /// </summary>
        public DanmakuMode danmakuMode = DanmakuMode.Video;
        /// <summary>
        /// 防挡字幕
        /// </summary>
        public bool notHideSubtitle = false;

        protected override Size MeasureOverride(Size availableSize)
        {
            SetRows();
            return base.MeasureOverride(availableSize);
        }
        private void SetRows()
        {
            if (tb_test.ActualHeight == 0)
            {
                return;
            }
            var rowHieght = tb_test.ActualHeight;

            var topHieght = grid_Top.ActualHeight;
            var pageHieght = grid_Roll.ActualHeight;
            //将全部行去除
            grid_Top.RowDefinitions.Clear();
            grid_Bottom.RowDefinitions.Clear();
            grid_Roll.RowDefinitions.Clear();

            int num = Convert.ToInt32(topHieght / rowHieght);
            int pnum = Convert.ToInt32(pageHieght / rowHieght);

            for (int i = 0; i < num; i++)
            {
                grid_Bottom.RowDefinitions.Add(new RowDefinition());
                grid_Top.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < pnum; i++)
            {
                grid_Roll.RowDefinitions.Add(new RowDefinition());
            }
        }

        public void LoadDanmaku(DanmakuMode mode)
        {
            mode = DanmakuMode.Video;
            SetRows();
        }
        public void LoadDanmaku(DanmakuMode mode,double fontZoom, int _speed, double _transparent, DanmakuBorderStyle style)
        {
            sizeZoom = fontZoom;
            speed = _speed;
            transparent = _transparent;
            borderStyle = style;
            SetRows();
        }

        public void SetFontSizeZoom(double value)
        {
            if (value > 3)
            {
                value = 3;
            }
            if (value < 0.1)
            {
                value = 0.1;
            }
            sizeZoom = value;
            tb_test.FontSize = 25 * value;
            SetRows();
            foreach (var item in grid_Roll.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * sizeZoom;
                    }
                }

            }
            foreach (var item in grid_Top.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * sizeZoom;
                    }
                }

            }
            foreach (var item in grid_Bottom.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * sizeZoom;
                    }
                }

            }
        }

        public void SetSpeed(int value)
        {
            if (value <= 0)
            {
                value = 1;
            }
            speed = value;
        }

        public void SetTransparent(double value)
        {
            if (value > 0.1)
            {
                value = 0.1;
            }
            transparent = value;
            gv.Opacity = value;
        }

        /// <summary>
        /// 暂停弹幕
        /// </summary>
        public void PauseDanmaku()
        {
            foreach (var item in storyList)
            {
                item.Pause();
            }
        }
        /// <summary>
        /// 继续弹幕
        /// </summary>
        public void ResumeDanmaku()
        {
            foreach (var item in storyList)
            {
                item.Resume();
            }
        }

        public void Remove(DanmakuModel danmaku)
        {
            switch (danmaku.location)
            {
                case DanmakuLocation.Top:
                    
                    foreach (Grid item in grid_Top.Children)
                    {
                        if (item.Tag as DanmakuModel==danmaku)
                        {
                            grid_Top.Children.Remove(item);
                        }
                    }
                    break;
                case DanmakuLocation.Bottom:
                    foreach (Grid item in grid_Bottom.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
                        {
                            grid_Bottom.Children.Remove(item);
                        }
                    }
                    break;
                case DanmakuLocation.Other:
                    foreach (Grid item in grid_Roll.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
                        {
                            grid_Roll.Children.Remove(item);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearAll()
        {
            storyList.Clear();
            grid_Bottom.Children.Clear();
            grid_Top.Children.Clear();
            grid_Roll.Children.Clear();

        }

        private Grid CreateControlShadow(DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();
            DropShadowPanel dropShadowPanel = new DropShadowPanel()
            {
                BlurRadius = 6,
                ShadowOpacity = 1,
                OffsetX = 0,
                OffsetY = 0,
                Color = SetBorder(model.color)
            };


            Grid grid = new Grid();



            tx.Text = model.text;

            tx.FontWeight = FontWeights.Bold;
            tx.Foreground = new SolidColorBrush( model.color);
            //弹幕大小
            double size = model.size * sizeZoom;
            tx.FontSize = size;


            dropShadowPanel.Content = tx;

            grid.Children.Add(dropShadowPanel);

            return grid;
        }
        private Grid CreateControlBorder(DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();
            TextBlock tx2 = new TextBlock();
            Grid grid = new Grid();


            tx2.Text = model.text;
            tx.Text = model.text;
            tx.FontWeight = FontWeights.Bold;
            tx2.FontWeight = FontWeights.Bold;
            tx2.Foreground = new SolidColorBrush(SetBorder(model.color));
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;

            tx2.FontSize = size;
            tx.FontSize = size;

            tx2.Margin = new Thickness(1);
            //grid包含弹幕文本信息

            grid.Children.Add(tx2);
            grid.Children.Add(tx);
            return grid;
        }
        private Grid CreateControlNoBorder(DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();

            Grid grid = new Grid();

            tx.Text = model.text;
            tx.FontWeight = FontWeights.Bold;

            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;

            tx.FontSize = size;

            grid.Children.Add(tx);
            return grid;
        }
        private Color SetBorder(Color textColor)
        {
            if (textColor.R < 100 && textColor.G < 100 && textColor.B < 100)
            {
                return Colors.White;
            }
            else
            {
                return Colors.Black;
            }
        }


        private int ComputeTopRow()
        {
            var rows = 0;
            var max = grid_Top.RowDefinitions.Count;
            if (notHideSubtitle&& grid_Top.RowDefinitions.Count>=5)
            {

                max = grid_Top.RowDefinitions.Count - 3;
            }
            for (int i = 0; i < max; i++)
            {
                rows = i;
                bool has = false;
                foreach (Grid item in grid_Top.Children)
                {
                    var row = Grid.GetRow(item);
                    if (row == i)
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    return rows;
                }
                if (i == max - 1)
                {
                    return -1;
                }
            }
            return rows;
        }
        private int ComputeBottomRow()
        {

            var rows = grid_Bottom.RowDefinitions.Count;
            if (rows == 0)
            {
                SetRows();
                rows = grid_Bottom.RowDefinitions.Count - 1;
            }
            for (int i = grid_Bottom.RowDefinitions.Count - 1; i >= 0; i--)
            {
                rows = i;
                bool has = false;
                foreach (Grid item in grid_Bottom.Children)
                {
                    var row = Grid.GetRow(item);
                    if (row == i)
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    return rows;
                }
            }
            return rows;
        }
        private int ComputeRollRow()
        {
            var rows = 0;

            var max = grid_Roll.RowDefinitions.Count;
            if (notHideSubtitle && grid_Top.RowDefinitions.Count >= 5)
            {
                max = grid_Roll.RowDefinitions.Count - 3;
            }
            for (int i = 0; i < max; i++)
            {
                rows = i;
                bool has = false;
                foreach (Grid item in grid_Roll.Children)
                {
                    var row = Grid.GetRow(item);
                    if (row == i)
                    {
                        if ((item.RenderTransform as TranslateTransform).X > grid_Roll.ActualWidth / 1.2)
                        {
                            has = true;
                            break;
                        }
                    }
                }

                if (!has)
                {
                    return rows;
                }
                if (i == max - 1)
                {
                    return -1;
                }
            }
            return rows;
        }
        /// <summary>
        /// 添加滚动弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public void AddRollDanmu(DanmakuModel m,bool own)
        {
            Grid grid=null;
            switch (borderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(Colors.Gray);
                grid.BorderThickness = new Thickness(1);
            }
            var r = ComputeRollRow();
            if (r == -1)
            {
                return;
            }
            Grid.SetRow(grid, r);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid_Roll.Children.Add(grid);
            grid_Roll.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = gv.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(speed));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            myDoubleAnimationX.To = -(grid.ActualWidth);//到达
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            storyList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Roll.Children.Remove(grid);
                storyList.Remove(moveStoryboard);

            });
            moveStoryboard.Begin();


        }
        /// <summary>
        /// 添加直播滚动弹幕
        /// </summary>
        /// <param name="text">参数</param>
        /// <param name="own">是否自己发送的</param>
        /// <param name="color">颜色</param>
        public void AddLiveDanmu(string text, bool own,Color? color)
        {
            if (color==null)
            {
                color = Colors.White;
            }
            var m = new DanmakuModel() {
                 text= text,
                 color= color.Value,
                 location= DanmakuLocation.Roll,
                 size=25
            };
            Grid grid = null;
            switch (borderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(Colors.Gray);
                grid.BorderThickness = new Thickness(1);
            }
            var r = ComputeRollRow();
            if (r == -1)
            {
                return;
            }
            Grid.SetRow(grid, r);

            grid_Roll.Children.Add(grid);
            grid_Roll.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = gv.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(speed));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            myDoubleAnimationX.To = -(grid.ActualWidth);//到达
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            storyList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Roll.Children.Remove(grid);
                storyList.Remove(moveStoryboard);

            });
            moveStoryboard.Begin();
        }
        /// <summary>
        ///  添加顶部弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public void AddTopDanmu(DanmakuModel m, bool own)
        {
           
            Grid grid = null;
            switch (borderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(Colors.Gray);
                grid.BorderThickness = new Thickness(1);
            }

            var r = ComputeTopRow();
            if (r == -1)
            {
                return;
            }

            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(grid, r);
            grid_Top.Children.Add(grid);


            //创建空转换动画
            TranslateTransform moveTransform = new TranslateTransform();
            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(5));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            storyList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Top.Children.Remove(grid);
                storyList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }
        /// <summary>
        ///  添加底部弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public void AddBottomDanmu(DanmakuModel m, bool own)
        {
            Grid grid = null;
            switch (borderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(Colors.Gray);
                grid.BorderThickness = new Thickness(1);
            }
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(grid, ComputeBottomRow());
            grid_Bottom.Children.Add(grid);


            //创建空转换动画
            TranslateTransform moveTransform = new TranslateTransform();
            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(5));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            storyList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Bottom.Children.Remove(grid);
                storyList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }

    }
}
