using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace NSDanmaku.Controls
{
    public sealed partial class Danmaku : UserControl
    {
        public static float LogicalDpi { get; set; } = 0;
        /// <summary>
        /// 初始化弹幕DPI
        /// </summary>
        public static void InitDanmakuDpi()
        {
            try
            {
                Windows.Graphics.Display.DisplayInformation displayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
                LogicalDpi = displayInformation.LogicalDpi;
            }
            catch (Exception)
            {
                LogicalDpi = 96f;
            }
           
        }


        public Danmaku()
        {
            this.InitializeComponent();
            DanmakuArea = 1.0;
            DanmakuBold = false;
            DanmakuFontFamily = "";
        }
        #region 弹幕属性
        /// <summary>
        /// 字体大小缩放，电脑推荐默认1.0，手机推荐0.5
        /// </summary>
        public double DanmakuSizeZoom
        {
            get { return (double)GetValue(DanmakuSizeZoomProperty); }
            set { SetValue(DanmakuSizeZoomProperty, value); }
        }
       
        public static readonly DependencyProperty DanmakuSizeZoomProperty =
            DependencyProperty.Register("DanmakuSizeZoom", typeof(double), typeof(Danmaku), new PropertyMetadata(1.0, OnDanmakuSizeZoomChanged));

        private static void OnDanmakuSizeZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = Convert.ToDouble(e.NewValue);
            if (value > 3)
            {
                value = 3;
            }
            if (value < 0.1)
            {
                value = 0.1;
            }
            //DanmakuSizeZoom = value;

            ((Danmaku)d).SetFontDanmakuSizeZoom(value);
        }
        private void SetFontDanmakuSizeZoom(double value)
        {
           
            SetRows(this.ActualHeight);
            foreach (var item in grid_Scroll.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * DanmakuSizeZoom;
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
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * DanmakuSizeZoom;
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
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * DanmakuSizeZoom;
                    }
                }

            }
        }

        /// <summary>
        /// 滚动弹幕动画持续时间,单位:秒,越小弹幕移动速度越快
        /// </summary>
        public int DanmakuDuration
        {
            get { return (int)GetValue(DanmakuDurationProperty); }
            set { SetValue(DanmakuDurationProperty, value); }
        }

        public static readonly DependencyProperty DanmakuDurationProperty =
            DependencyProperty.Register("DanmakuDuration", typeof(int), typeof(Danmaku), new PropertyMetadata(10, OnDanmakuDurationChanged));
        private static void OnDanmakuDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = Convert.ToInt32(e.NewValue);
            if (value <= 0)
            {
                value = 1;
            }
           ((Danmaku)d).SetDanmakuDuration(value);
        }
        public void SetDanmakuDuration(int value)
        {
            DanmakuDuration = value;
        }


        /// <summary>
        /// 弹幕是否加粗
        /// </summary>
        public bool DanmakuBold
        {
            get { return (bool)GetValue(DanmakuBoldProperty); }
            set { SetValue(DanmakuBoldProperty, value); }
        }

        public static readonly DependencyProperty DanmakuBoldProperty =
            DependencyProperty.Register("DanmakuBold", typeof(bool), typeof(Danmaku), new PropertyMetadata(0));


        /// <summary>
        /// 弹幕字体名称
        /// </summary>
        public string DanmakuFontFamily
        {
            get { return (string)GetValue(DanmakuFontFamilyProperty); }
            set { SetValue(DanmakuFontFamilyProperty, value); }
        }

     
        public static readonly DependencyProperty DanmakuFontFamilyProperty =
            DependencyProperty.Register("DanmakuFontFamily", typeof(string), typeof(Danmaku), new PropertyMetadata(0));


        /// <summary>
        /// 弹幕样式
        /// </summary>
        public DanmakuBorderStyle DanmakuStyle
        {
            get { return (DanmakuBorderStyle)GetValue(DanmakuStyleProperty); }
            set { SetValue(DanmakuStyleProperty, value); }
        }
        public static readonly DependencyProperty DanmakuStyleProperty =
          DependencyProperty.Register("DanmakuStyle", typeof(DanmakuBorderStyle), typeof(Danmaku), new PropertyMetadata(DanmakuBorderStyle.Stroke));


        /// <summary>
        /// 弹幕显示区域，取值0.1-1.0
        /// </summary>
        public double DanmakuArea
        {
            get { return (double)GetValue(DanmakuAreaProperty); }
            set { SetValue(DanmakuAreaProperty, value); }
        }
       
        public static readonly DependencyProperty DanmakuAreaProperty =
            DependencyProperty.Register("DanmakuArea", typeof(double), typeof(Danmaku), new PropertyMetadata(1, OnDanmakuAreaChanged));

        private static void OnDanmakuAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = Convert.ToDouble(e.NewValue);
            if (value <=0)
            {
                value = 0.1;
            }
            if (value > 1)
            {
                value = 1;
            }

           ((Danmaku)d).SetDanmakuArea(value);
        }
        public void SetDanmakuArea(double value)
        {
            DanmakuArea = value;
        }
        #endregion

        //动画管理
        List<Storyboard> topBottomStoryList = new List<Storyboard>();
        List<Storyboard> rollStoryList = new List<Storyboard>();
        List<Storyboard> positionStoryList = new List<Storyboard>();

        protected override Size MeasureOverride(Size availableSize)
        {
            SetRows(availableSize.Height);
            return base.MeasureOverride(availableSize);
        }

        private void SetRows(double height)
        {
            
            var txt = new TextBlock() { 
                Text="测试test",
                FontSize=25*DanmakuSizeZoom,
            };
            txt.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var rowHieght = txt.DesiredSize.Height;
          
           
          
            //将全部行去除
            grid_Top.RowDefinitions.Clear();
            grid_Bottom.RowDefinitions.Clear();
            grid_Scroll.RowDefinitions.Clear();

            int num = Convert.ToInt32(height / rowHieght);
           // int pnum = Convert.ToInt32(height / rowHieght);

            //for (int i = 0; i < num; i++)
            //{
            //    grid_Bottom.RowDefinitions.Add(new RowDefinition());
            //    grid_Top.RowDefinitions.Add(new RowDefinition());
            //}
            for (int i = 0; i < num; i++)
            {
                grid_Bottom.RowDefinitions.Add(new RowDefinition());
                grid_Top.RowDefinitions.Add(new RowDefinition());
                grid_Scroll.RowDefinitions.Add(new RowDefinition());
            }
        }
        private int GetTopAvailableRow()
        {
          
            var max = grid_Top.RowDefinitions.Count/2;
           
            for (int i = 0; i < max; i++)
            {
                
                var row = grid_Top.Children.FirstOrDefault(x=>Grid.GetRow((x as Grid)) == i);
                if (row!=null)
                {
                    continue;
                }
                else
                {
                    return i;
                }
                
            }
            return -1;
        }
        private int GetBottomAvailableRow()
        {

            var max = grid_Bottom.RowDefinitions.Count/2;
            for (int i = 1; i <= max; i++)
            {
                var rowNum = grid_Bottom.RowDefinitions.Count - i;
                var row = grid_Bottom.Children.FirstOrDefault(x => Grid.GetRow((x as Grid)) == rowNum);
                if (row != null)
                {
                    continue;
                }
                else
                {
                    return rowNum;
                }
            }
            //for (int i = grid_Bottom.RowDefinitions.Count; i >= 0; i--)
            //{
            //    var row = grid_Bottom.Children.FirstOrDefault(x => Grid.GetRow((x as Grid)) == i);
            //    if (row != null)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        if (i>=max)
            //        {
            //            return i;
            //        }
                    
            //    }

            //}
            return -1;
        }
        private int GetScrollAvailableRow(Grid item)
        {
            var width = grid_Scroll.ActualWidth;
            //计算弹幕尺寸
            item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var newWidth = item.DesiredSize.Width;
            if (newWidth <= 0) return -1;
           
            var max = grid_Scroll.RowDefinitions.Count*DanmakuArea;
           
            for (int i = 0; i < max; i++)
            {
                //1、检查当前行是否存在弹幕
                var lastItem=grid_Scroll.Children.LastOrDefault(x => Grid.GetRow((x as Grid)) == i);
                if (lastItem == null)
                {
                    return i;
                }
              
                var lastWidth = (lastItem as Grid).ActualWidth;
                var lastX = (lastItem.RenderTransform as TranslateTransform).X;
                
                //2、前弹幕必须已经完全从右侧移动完毕
                if (lastX > width- lastWidth)
                {
                    continue;
                }
                //3、后弹幕速度小于等于前弹幕速度
                var lastSpeed = (lastWidth + width) / DanmakuDuration;
                var newSpeed = (newWidth + width) / DanmakuDuration;
                if (newSpeed<= lastSpeed)
                {
                    return i;
                }
                //4、弹幕移动期间不会重叠
                var runDistance =width- lastX;
                var t1 = (runDistance - newWidth) / (newSpeed - lastSpeed);
                var t2 = lastX / lastSpeed;
                if (t1 > t2)
                {
                    return i;
                }    
            }
            return -1;
        }


        private async Task<Grid> CreateNewDanmuControl(DanmakuModel m)
        {
            switch (DanmakuStyle)
            {
                case DanmakuBorderStyle.WithoutStroke:
                    return DanmakuItemControl.CreateControlNoBorder((float)DanmakuSizeZoom, DanmakuBold, DanmakuFontFamily, m);
                case DanmakuBorderStyle.Stroke:
                    return await DanmakuItemControl.CreateControlBorder((float)DanmakuSizeZoom, DanmakuBold, DanmakuFontFamily, m);
                default:
                    return DanmakuItemControl.CreateControlOverlap((float)DanmakuSizeZoom, DanmakuBold, DanmakuFontFamily, m);
            }

        }
        /// <summary>
        /// 添加直播滚动弹幕
        /// </summary>
        /// <param name="text">参数</param>
        /// <param name="own">是否自己发送的</param>
        /// <param name="color">颜色</param>
        public async void AddLiveDanmu(string text, bool own, Color? color)
        {
            if (color == null)
            {
                color = Colors.White;
            }
            var m = new DanmakuModel()
            {
                text = text,
                color = color.Value,
                location = DanmakuLocation.Scroll,
                size = 25
            };
            Grid grid = await CreateNewDanmuControl(m);
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(color.Value);
                grid.BorderThickness = new Thickness(1);
            }
            var r = GetScrollAvailableRow(grid);
            if (r == -1)
            {
                return;
            }
            Grid.SetRow(grid, r);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid_Scroll.Children.Add(grid);
            grid_Scroll.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = gv.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(DanmakuDuration));
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
            rollStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Scroll.Children.Remove(grid);
                grid.Children.Clear();
                grid = null;
                rollStoryList.Remove(moveStoryboard);
                moveStoryboard.Stop();
                moveStoryboard = null;

            });
            moveStoryboard.Begin();
        }
        /// <summary>
        /// 添加一条弹幕
        /// </summary>
        /// <param name="m"></param>
        /// <param name="own"></param>
        /// <returns></returns>
        public async Task AddDanmu(DanmakuModel m, bool own)
        {
            switch (m.location)
            {
                case DanmakuLocation.Scroll:
                    await AddScrollDanmu(m, own);
                    break;
                case DanmakuLocation.Top:
                    await AddTopDanmu(m, own);
                    break;
                case DanmakuLocation.Bottom:
                    await AddBottomDanmu(m, own);
                    break;
                case DanmakuLocation.Position:
                    await AddPositionDanmu(m);
                    break;
                default:
                    //await AddScrollDanmu(m, own);
                    break;
            }
        }

       
        /// <summary>
        /// 添加滚动弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public async Task AddScrollDanmu(DanmakuModel m, bool own)
        {
            Grid grid = await CreateNewDanmuControl(m);

            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(m.color);
                grid.BorderThickness = new Thickness(1);
            }
            var r = GetScrollAvailableRow(grid);
            if (r == -1)
            {
                grid = null;
                return;
            }

            Grid.SetRow(grid, r);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid_Scroll.Children.Add(grid);
            grid_Scroll.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = gv.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(DanmakuDuration));
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
            rollStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Scroll.Children.Remove(grid);
                grid.Children.Clear();
                grid = null;
                rollStoryList.Remove(moveStoryboard);
                moveStoryboard.Stop();
                moveStoryboard = null;
            });
            moveStoryboard.Begin();


        }

        /// <summary>
        /// 添加图片滚动弹幕
        /// </summary>
        /// <param name="m">参数</param>
        public void AddScrollImageDanmu(BitmapImage m)
        {
            Grid grid = null;
            grid = DanmakuItemControl.CreateImageControl(m);
            var r = GetScrollAvailableRow(grid);
            if (r == -1)
            {
                return;
            }
            Grid.SetRow(grid, r);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;
            grid_Scroll.Children.Add(grid);
            grid_Scroll.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = gv.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(DanmakuDuration));
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
            rollStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {

                grid_Scroll.Children.Remove(grid);
                grid = null;
                rollStoryList.Remove(moveStoryboard);

            });
            moveStoryboard.Begin();


        }
        /// <summary>
        ///  添加顶部弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public async Task AddTopDanmu(DanmakuModel m, bool own)
        {

            Grid grid = await CreateNewDanmuControl(m);
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(m.color);
                grid.BorderThickness = new Thickness(1);
            }

            var r = GetTopAvailableRow();
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
            topBottomStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Top.Children.Remove(grid);
                grid.Children.Clear();
                grid = null;
                topBottomStoryList.Remove(moveStoryboard);
                moveStoryboard.Stop();
                moveStoryboard = null;

            });
            moveStoryboard.Begin();
        }
        /// <summary>
        ///  添加底部弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public async Task AddBottomDanmu(DanmakuModel m, bool own)
        {
            Grid grid = await CreateNewDanmuControl(m);
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(m.color);
                grid.BorderThickness = new Thickness(1);
            }
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            var row = GetBottomAvailableRow();
            if (row == -1)
            {
                return;
            }
            Grid.SetRow(grid, row);
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
            topBottomStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Bottom.Children.Remove(grid);
                grid.Children.Clear();
                grid = null;
                topBottomStoryList.Remove(moveStoryboard);
                moveStoryboard.Stop();
                moveStoryboard = null;
            });
            moveStoryboard.Begin();
        }
        /// <summary>
        /// 添加定位弹幕
        /// </summary>
        /// <param name="m"></param>
        public async Task AddPositionDanmu(DanmakuModel m)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(m.text);
            m.text = data[4].ToString().Replace("/n", "\r\n");
             Grid grid = await CreateNewDanmuControl(m); ;
            var DanmakuFontFamilyFamily = data[data.Length - 2].ToString();
            
            grid.Tag = m;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;

            double toX = 0;
            double toY = 0;

            double X = 0, Y = 0;
            double dur = 0;

            if (data.Length > 7)
            {
                X =data[0].ToDouble();
                Y = data[1].ToDouble();

                toX =data[7].ToDouble();
                toY = data[8].ToDouble();

                dur = data[10].ToDouble();

            }
            else
            {
                toX = data[0].ToDouble();
                toY =data[1].ToDouble();
            }
            if (toX < 1 && toY < 1)
            {
                toX = this.ActualWidth * toX;
                toY = this.ActualHeight * toY;
            }
            if (X < 1 && Y < 1)
            {
                X = this.ActualWidth * X;
                Y = this.ActualHeight * Y;
            }

            if (data.Length >= 7)
            {
                var rotateZ = data[5].ToDouble();
                var rotateY = data[6].ToDouble();
                PlaneProjection projection = new PlaneProjection();
                projection.RotationZ = -rotateZ;
                projection.RotationY = rotateY;
                grid.Projection = projection;
            }

            //Canvas.SetLeft(grid, toX);
            //Canvas.SetTop(grid, toY);

            canvas.Children.Add(grid);


            double dmDuration = data[3].ToDouble();
            var opacitys = data[2].ToString().Split('-');
            double opacityFrom = opacitys[0].ToDouble();
            double opacityTo = opacitys[1].ToDouble();

            //创建故事版
            Storyboard moveStoryboard = new Storyboard();


            //if (X != toX || Y != toY)
            //{
            Duration duration = new Duration(TimeSpan.FromMilliseconds(dur));
            {
                DoubleAnimation myDoubleAnimationY = new DoubleAnimation();
                myDoubleAnimationY.Duration = duration;
                myDoubleAnimationY.From = Y;
                myDoubleAnimationY.To = toY;


                Storyboard.SetTarget(myDoubleAnimationY, grid);
                Storyboard.SetTargetProperty(myDoubleAnimationY, "(Canvas.Top)");
                moveStoryboard.Children.Add(myDoubleAnimationY);
            }
            {
                DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
                myDoubleAnimationX.Duration = duration;
                myDoubleAnimationX.From = X;
                myDoubleAnimationX.To = toX;
                Storyboard.SetTarget(myDoubleAnimationX, grid);
                Storyboard.SetTargetProperty(myDoubleAnimationX, "(Canvas.Left)");
                moveStoryboard.Children.Add(myDoubleAnimationX);
            }
            //}
            //else
            //{
            //    Canvas.SetTop(grid,toY);
            //    Canvas.SetLeft(grid,toX);
            //}

            //透明度动画 
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(dmDuration)),
                From = opacityFrom,
                To = opacityTo
            };
            Storyboard.SetTarget(opacityAnimation, grid);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            moveStoryboard.Children.Add(opacityAnimation);



            positionStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                canvas.Children.Remove(grid);
                positionStoryList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }
        #region 弹幕控制方法
        /// <summary>
        /// 暂停弹幕
        /// </summary>
        public void PauseDanmaku()
        {
            foreach (var item in topBottomStoryList)
            {
                item.Pause();
            }
            foreach (var item in rollStoryList)
            {
                item.Pause();
            }
            foreach (var item in positionStoryList)
            {
                item.Pause();
            }
        }
        /// <summary>
        /// 继续弹幕
        /// </summary>
        public void ResumeDanmaku()
        {
            foreach (var item in topBottomStoryList)
            {
                item.Resume();
            }
            foreach (var item in rollStoryList)
            {
                item.Resume();
            }
            foreach (var item in positionStoryList)
            {
                item.Resume();
            }
        }
        /// <summary>
        /// 移除指定弹幕
        /// </summary>
        /// <param name="danmaku"></param>
        public void Remove(DanmakuModel danmaku)
        {
            switch (danmaku.location)
            {
                case DanmakuLocation.Top:

                    foreach (Grid item in grid_Top.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
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
                    foreach (Grid item in grid_Scroll.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
                        {
                            grid_Scroll.Children.Remove(item);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 清空弹幕
        /// </summary>
        public void ClearAll()
        {
            topBottomStoryList.Clear();
            rollStoryList.Clear();
            grid_Bottom.Children.Clear();
            grid_Top.Children.Clear();
            grid_Scroll.Children.Clear();

        }

        /// <summary>
        /// 读取屏幕上的全部弹幕
        /// </summary>
        /// <param name="danmakuLocation">类型</param>
        /// <returns></returns>
        public List<DanmakuModel> GetDanmakus(DanmakuLocation? danmakuLocation = null)
        {
            List<DanmakuModel> danmakus = new List<DanmakuModel>();
            if (danmakuLocation == null || danmakuLocation == DanmakuLocation.Top)
            {
                foreach (Grid item in grid_Top.Children)
                {
                    danmakus.Add(item.Tag as DanmakuModel);
                }
            }
            if (danmakuLocation == null || danmakuLocation == DanmakuLocation.Bottom)
            {
                foreach (Grid item in grid_Bottom.Children)
                {
                    danmakus.Add(item.Tag as DanmakuModel);
                }
            }
            if (danmakuLocation == null || danmakuLocation == DanmakuLocation.Scroll)
            {
                foreach (Grid item in grid_Scroll.Children)
                {
                    danmakus.Add(item.Tag as DanmakuModel);
                }
            }
            return danmakus;
        }

        /// <summary>
        /// 隐藏弹幕
        /// </summary>
        /// <param name="location">需要隐藏的位置</param>
        public void HideDanmaku(DanmakuLocation location)
        {
            switch (location)
            {
                case DanmakuLocation.Scroll:
                    grid_Scroll.Visibility = Visibility.Collapsed;
                    break;
                case DanmakuLocation.Top:
                    grid_Top.Visibility = Visibility.Collapsed;
                    break;
                case DanmakuLocation.Bottom:
                    grid_Bottom.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 显示弹幕
        /// </summary>
        /// <param name="location">需要显示的位置</param>
        public void ShowDanmaku(DanmakuLocation location)
        {
            switch (location)
            {
                case DanmakuLocation.Scroll:
                    grid_Scroll.Visibility = Visibility.Visible;
                    break;
                case DanmakuLocation.Top:
                    grid_Top.Visibility = Visibility.Visible;
                    break;
                case DanmakuLocation.Bottom:
                    grid_Bottom.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
