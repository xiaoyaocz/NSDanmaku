using NSDanmaku.Helper;
using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace NSDanmakuDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.SizeChanged += MainPage_SizeChanged;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetRows();
        }

        List<Storyboard> storyList = new List<Storyboard>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SetRows();
        }



        int i = 0;
        private TextBlock CreateDanmu(DanmakuModel m)
        {
            TextBlock tb = new TextBlock();
            tb.Text = m.text;
            tb.Foreground = new SolidColorBrush(m.color);
            tb.FontSize = Convert.ToInt32(m.size) * size;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.HorizontalAlignment = HorizontalAlignment.Left;

            tb.Tag = m;
            return tb;




        }



        private int ComputeTopRow()
        {
            var rows = 0;
            var max = grid_Top.RowDefinitions.Count;
            if (ziMu)
            {
                max = grid_Top.RowDefinitions.Count - 3;
            }
            for (int i = 0; i < max; i++)
            {
                rows = i;
                bool has = false;
                foreach (TextBlock item in grid_Top.Children)
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
                foreach (TextBlock item in grid_Bottom.Children)
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
            if (ziMu)
            {
                max = grid_Roll.RowDefinitions.Count - 3;
            }
            for (int i = 0; i < max; i++)
            {
                rows = i;
                bool has = false;
                foreach (TextBlock item in grid_Roll.Children)
                {
                    var row = Grid.GetRow(item);
                    if (row == i)
                    {
                        if ((item.RenderTransform as TranslateTransform).X > grid_Roll.ActualWidth / 1.5)
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
                if (i== max-1)
                {
                    return -1;
                }
            }
            return rows;
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


        private void btn_AddDanmu_Click(object sender, RoutedEventArgs e)
        {
            //AddRollDanmu("测试弹幕...");
        }

        private void btn_AddTopDanmu_Click(object sender, RoutedEventArgs e)
        {
            //AddTopDanmu();

        }
        private void btn_AddBottomDanmu_Click(object sender, RoutedEventArgs e)
        {
            //AddBottomDanmu();
        }

        private void AddRollDanmu(DanmakuModel m)
        {
            var tb = CreateDanmu(m);
            var r = ComputeRollRow();
            if (r==-1)
            {
                return;
            }
            Grid.SetRow(tb, r);
           
            grid_Roll.Children.Add(tb);
            grid_Roll.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = gv.ActualWidth;
            tb.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(speed));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            myDoubleAnimationX.To = -(tb.ActualWidth);//到达
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            storyList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                grid_Roll.Children.Remove(tb);
                storyList.Remove(moveStoryboard);

            });
            moveStoryboard.Begin();


        }

        private void AddTopDanmu(DanmakuModel m)
        {
            var tb = CreateDanmu(m);

            var r = ComputeTopRow();
            if (r == -1)
            {
                return;
            }

            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(tb, r);
            grid_Top.Children.Add(tb);


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
                grid_Top.Children.Remove(tb);
                storyList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }

        private void AddBottomDanmu(DanmakuModel m)
        {
            var tb = CreateDanmu(m);

            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(tb, ComputeBottomRow());
            grid_Bottom.Children.Add(tb);


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
                grid_Bottom.Children.Remove(tb);
                storyList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {

            foreach (var item in storyList)
            {
                item.Pause();
            }

        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in storyList)
            {
                item.Resume();
            }
        }


        private void btn_PlayTest_Click(object sender, RoutedEventArgs e)
        {

            timer.Start();
        }

        private void btn_PauseTest_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        List<NSDanmaku.Model.DanmakuModel> DanMuPool = null;
        int time = 0;
        DispatcherTimer timer;
        private async void btn_LoadDanmu_Click(object sender, RoutedEventArgs e)
        {
            time = 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            var danmakuParse = new DanmakuParse();
            DanMuPool = await danmakuParse.ParseBiliBili(32005515);
            await new MessageDialog("OK").ShowAsync();
        }
        private void Timer_Tick(object sender, object e)
        {

            var danmu = DanMuPool.Where(x => Convert.ToInt32(x.time) == time);
            foreach (var item in danmu)
            {
                switch (item.location)
                {
                    case NSDanmaku.Model.DanmakuLocation.Top:
                        AddTopDanmu(item);
                        break;
                    case NSDanmaku.Model.DanmakuLocation.Bottom:
                        AddBottomDanmu(item);
                        break;
                    default:
                        AddRollDanmu(item);
                        break;
                }


            }
            time++;
        }





    


        double size = 1;
        private void slider_size_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (slider_size.Value != 0)
            {
                tb_test.FontSize = 25 * slider_size.Value;
                size = slider_size.Value;
                ChangeDanmuSize();
                SetRows();

            }

        }

        private void ChangeDanmuSize()
        {
            foreach (var item in grid_Roll.Children)
            {
                var tb = item as TextBlock;
                var m = tb.Tag as DanmakuModel;

                tb.FontSize = Convert.ToInt32(m.size) * size;
            }
            foreach (var item in grid_Top.Children)
            {
                var tb = item as TextBlock;
                var m = tb.Tag as DanmakuModel;

                tb.FontSize = Convert.ToInt32(m.size) * size;
            }
            foreach (var item in grid_Bottom.Children)
            {
                var tb = item as TextBlock;
                var m = tb.Tag as DanmakuModel;

                tb.FontSize = Convert.ToInt32(m.size) * size;
            }
        }


        double speed = 10;
        private void slider_speed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            speed = slider_speed.Value;
        }

        bool ziMu = false;
        private void check_Bottom_Checked(object sender, RoutedEventArgs e)
        {
            ziMu = true;
        }

        private void check_Bottom_Unchecked(object sender, RoutedEventArgs e)
        {
            ziMu = false;
        }

        private void slider_tran_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (slider_tran.Value!=0)
            {
                gv.Opacity = slider_tran.Value;
            }
            
        }
    }
   

 
}
