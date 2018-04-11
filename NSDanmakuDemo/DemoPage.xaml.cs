using NSDanmaku.Controls;
using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace NSDanmakuDemo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DemoPage : Page
    {
        public DemoPage()
        {
            this.InitializeComponent();
            MTC.DanmuLoaded += MTC_DanmuLoaded;
            MTC.OnMiniWindows += MTC_OnMiniWindows;
            MTC.OnExitMiniWindows += MTC_OnExitMiniWindows;
        }

        private async void MTC_OnExitMiniWindows(object sender, EventArgs e)
        {

            await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            danmuControls.ClearAll();
            danmuControls.SetSpeed(12);
            danmuControls.SetFontSizeZoom(1.0);
        }

        private async void MTC_OnMiniWindows(object sender, EventArgs e)
        {
            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
            {
                await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
                danmuControls.ClearAll();
                danmuControls.SetSpeed(5);
                danmuControls.SetFontSizeZoom(0.5);
            }
          
        }

        private void MTC_DanmuLoaded(object sender, Danmaku e)
        {
            danmuControls = e;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);



            //var d = MTC.FindName("danmuControls");
            //danmuControls = ;
            //danmuControls.LoadDanmaku(DanmakuMode.Video);
        }

        NSDanmaku.Controls.Danmaku danmuControls;
        DispatcherTimer timer;
        List<DanmakuModel> danmakus;
        private async void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            MTC.AddLog("Test");

            NSDanmaku.Helper.DanmakuParse danmakuParse = new NSDanmaku.Helper.DanmakuParse();

            danmakus = await danmakuParse.ParseBiliBili(29892777);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            FileOpenPicker fileOpen = new FileOpenPicker();
            fileOpen.FileTypeFilter.Add(".mp4");
            fileOpen.FileTypeFilter.Add("*");
            MTC.AddLog("加载视频中。。。");
            var file = await fileOpen.PickSingleFileAsync();
            if (file != null)
            {
                MTC.VideoTitle = file.Name;
                MTC.AddLog("开始播放。。。");
                mediaPlayer.SetSource(await file.OpenAsync(Windows.Storage.FileAccessMode.Read), "");
                mediaPlayer.Play();
            }



        }

        private void Timer_Tick(object sender, object e)
        {
            if (mediaPlayer.CurrentState == MediaElementState.Playing)
            {
                var danmu = danmakus.Where(x => Convert.ToInt32(x.time) == Convert.ToInt32(mediaPlayer.Position.TotalSeconds));
                foreach (var item in danmu)
                {
                    switch (item.location)
                    {
                        case NSDanmaku.Model.DanmakuLocation.Top:
                            danmuControls.AddTopDanmu(item, false);
                            break;
                        case NSDanmaku.Model.DanmakuLocation.Bottom:
                            danmuControls.AddBottomDanmu(item, false);
                            break;
                        default:
                            danmuControls.AddRollDanmu(item, false);
                            break;
                    }
                }

            }
        }

        private void mediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (mediaPlayer.CurrentState)
            {
                case MediaElementState.Closed:
                    danmuControls.ClearAll();
                    break;
                case MediaElementState.Opening:
                    break;
                case MediaElementState.Buffering:
                    danmuControls.PauseDanmaku();
                    break;
                case MediaElementState.Playing:
                    MTC.HideLog();
                    danmuControls.ResumeDanmaku();
                    break;
                case MediaElementState.Paused:
                    danmuControls.PauseDanmaku();
                    break;
                case MediaElementState.Stopped:
                    danmuControls.ClearAll();
                    break;
                default:
                    break;
            }
        }

        private void mediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MTC.AddLog(e.ErrorMessage);
        }
    }
}
