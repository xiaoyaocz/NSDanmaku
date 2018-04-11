using NSDanmaku.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace NSDanmakuDemo.Controls
{
    public sealed class DanmakuMTC : MediaTransportControls
    {
        public DanmakuMTC()
        {
            this.DefaultStyleKey = typeof(DanmakuMTC);
            VideoTitle = "";
        }
        public event EventHandler<Danmaku> DanmuLoaded;
        public event EventHandler OnMiniWindows;
        public event EventHandler OnExitMiniWindows;

        public bool IsMiniWindow = false;
        //public Danmaku danmuControls;
        protected override void OnApplyTemplate()
        {
         
            if (DanmuLoaded!=null)
            {
                DanmuLoaded(this, GetTemplateChild("danmuControls") as Danmaku);
            }


            (GetTemplateChild("MiniWindowsButton") as AppBarButton).Click += MiniWindowsButton_Click;


           


            base.OnApplyTemplate();
        }

        private void MiniWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsMiniWindow)
            {
                IsMiniWindow = true;
                (sender as AppBarButton).Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///Assets/PlayerAssets/down.png") };

                if (OnMiniWindows != null)
                {
                    OnMiniWindows(this, new EventArgs());
                }
            }
            else
            {
                IsMiniWindow = false;
                (sender as AppBarButton).Icon = new BitmapIcon() { UriSource = new Uri("ms-appx:///Assets/PlayerAssets/top.png") };
                if (OnExitMiniWindows != null)
                {
                    OnExitMiniWindows(this, new EventArgs());
                }
            }
            
            
        }



        public void AddLog(string text)
        {
            var tb= GetTemplateChild("Txt_Log") as TextBlock;
            tb.Visibility = Visibility.Visible;
            tb.Text += string.Format("[{0}]{1}\r\n",DateTime.Now.ToString("HH:mm:ss"),text);
        }
        public void ShowLog()
        {
            var tb = GetTemplateChild("Txt_Log") as TextBlock;
            tb.Visibility = Visibility.Visible;
        }
        public void HideLog()
        {
            var tb = GetTemplateChild("Txt_Log") as TextBlock;
            tb.Visibility = Visibility.Collapsed;
        }


        public string VideoTitle
        {
            get { return (string)GetValue(VideoTitleProperty); }
            set { {
                    SetValue(VideoTitleProperty, value);
                } }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoTitleProperty =
            DependencyProperty.Register("VideoTitle", typeof(string), typeof(MediaTransportControls), new PropertyMetadata("",null));
    }
}
