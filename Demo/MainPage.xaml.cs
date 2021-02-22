using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            danmaku.DanmakuStyle = NSDanmaku.Model.DanmakuBorderStyle.Border;
        }

        private async void btn_AddRoll_Click(object sender, RoutedEventArgs e)
        {
            await danmaku.AddScrollDanmu(new NSDanmaku.Model.DanmakuModel()
            {
                color = Colors.White,
                location = NSDanmaku.Model.DanmakuLocation.Scroll,
                size = 25,
                text = text.Text 
            }, ck_own.IsChecked.Value);
        }

        private async void btn_AddTop_Click(object sender, RoutedEventArgs e)
        {
            await danmaku.AddTopDanmu(new NSDanmaku.Model.DanmakuModel()
            {
                color = Colors.Blue,
                location = NSDanmaku.Model.DanmakuLocation.Scroll,
                size = 25,
                text = text.Text
            }, ck_own.IsChecked.Value);
        }

        private async void btn_AddBottom_Click(object sender, RoutedEventArgs e)
        {
           await danmaku.AddBottomDanmu(new NSDanmaku.Model.DanmakuModel()
            {
                color = Colors.Red,
                location = NSDanmaku.Model.DanmakuLocation.Scroll,
                size = 25,
                text = text.Text
            }, ck_own.IsChecked.Value);
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            danmaku.ClearAll();
        }
        private async void Timer_Tick(object sender, object e)
        {
            var danmu = danmakus.Where(x => Convert.ToInt32(x.time) == slider.Value);
            foreach (var item in danmu)
            {
                try
                {
                    await danmaku.AddDanmu(item, false);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Can't add danmaku:" + item.source);
                }
            }
            slider.Value++;
        }
        List<NSDanmaku.Model.DanmakuModel> danmakus;
        private async void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if (danmakus == null)
            {
                try
                {
                    NSDanmaku.Helper.DanmakuParse danmakuParse = new NSDanmaku.Helper.DanmakuParse();
                    danmakus = await danmakuParse.ParseBiliBili(11311248);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Can't load danmaku");
                    return;
                }

            }
            danmaku.ResumeDanmaku();
            timer.Start();
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            danmaku.PauseDanmaku();

        }

        private void btn_GetAll_Click(object sender, RoutedEventArgs e)
        {
            var ls = danmaku.GetDanmakus();
            Debug.WriteLine("Count:" + ls.Count);
        }

        private void ck_HideRoll_Checked(object sender, RoutedEventArgs e)
        {
            danmaku.HideDanmaku(NSDanmaku.Model.DanmakuLocation.Scroll);
        }

        private void ck_HideRoll_Unchecked(object sender, RoutedEventArgs e)
        {
            danmaku.ShowDanmaku(NSDanmaku.Model.DanmakuLocation.Scroll);
        }
    }
}
