using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace NSDanmaku.Controls
{
    public  class DanmakuItemControl
    {
        /// <summary>
        /// 创建重叠弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Grid CreateControlOverlap(float sizeZoom, bool bold, string fontFamily, DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();
            TextBlock tx2 = new TextBlock();
            Grid grid = new Grid();


            tx2.Text = model.text;
            tx.Text = model.text;
            if (bold)
            {
                tx.FontWeight = FontWeights.Bold;
                tx2.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                tx.FontFamily = new FontFamily(fontFamily);
                tx2.FontFamily = new FontFamily(fontFamily);
            }
            tx2.Foreground = new SolidColorBrush(GetBorderColor(model.color));
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;

            tx2.FontSize = size;
            tx.FontSize = size;

            tx2.Margin = new Thickness(1);
            //grid包含弹幕文本信息

            grid.Children.Add(tx2);
            grid.Children.Add(tx);
            grid.Tag = model;
            return grid;
        }
        /// <summary>
        /// 创建无边框弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Grid CreateControlNoBorder(float sizeZoom, bool bold, string fontFamily, DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = new TextBlock();

            Grid grid = new Grid();

            tx.Text = model.text;
            if (bold)
            {
                tx.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                tx.FontFamily = new FontFamily(fontFamily);
            }
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * sizeZoom;

            tx.FontSize = size;

            grid.Children.Add(tx);
            grid.Tag = model;
            return grid;
        }
        /// <summary>
        /// 创建图片弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Grid CreateImageControl(BitmapImage image)
        {
            //创建基础控件
            Image img = new Image();
            img.Source = image;
            Grid grid = new Grid();
            DanmakuModel model = new DanmakuModel() { text = "666666" };
            grid.Tag = model;
            grid.Children.Add(img);
            return grid;
        }
        /// <summary>
        /// 创建边框弹幕
        /// </summary>
        /// <param name="sizeZoom"></param>
        /// <param name="bold"></param>
        /// <param name="fontFamily"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<Grid> CreateControlBorder(float sizeZoom,bool bold, string fontFamily, DanmakuModel model)
        {
            if (Danmaku.LogicalDpi <= 0)
            {
                Danmaku.InitDanmakuDpi();
            }
            float size = (float)model.size * (float)sizeZoom;

            CanvasDevice device = CanvasDevice.GetSharedDevice();
           
            CanvasTextFormat fmt = new CanvasTextFormat() { FontSize = size };
            var tb = new TextBlock { Text = model.text, FontSize = size, };

            if (bold)
            {
                fmt.FontWeight = FontWeights.Bold;
                tb.FontWeight = FontWeights.Bold;
            }
            if (fontFamily != "")
            {
                fmt.FontFamily = fontFamily;
                tb.FontFamily = new FontFamily(fontFamily);
            }
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            
            var myBitmap = new CanvasRenderTarget(device, (float)tb.DesiredSize.Width, (float)tb.DesiredSize.Height, Danmaku.LogicalDpi);
           
            CanvasTextLayout canvasTextLayout = new CanvasTextLayout(device, model.text, fmt, (float)tb.DesiredSize.Width, (float)tb.DesiredSize.Height);
            
            CanvasGeometry combinedGeometry = CanvasGeometry.CreateText(canvasTextLayout);

            using (var ds = myBitmap.CreateDrawingSession())
            {
                ds.Clear(Colors.Transparent);
                ds.DrawGeometry(combinedGeometry, GetBorderColor(model.color), 2f,new CanvasStrokeStyle() {
                    DashStyle = CanvasDashStyle.Solid
                });
                ds.FillGeometry(combinedGeometry, model.color);
            }
            Image image = new Image();
            BitmapImage im = new BitmapImage();
            using (InMemoryRandomAccessStream oStream = new InMemoryRandomAccessStream())
            {
                await myBitmap.SaveAsync(oStream, CanvasBitmapFileFormat.Png, 1.0f);
                await im.SetSourceAsync(oStream);
            }
            image.Source = im;
            image.Stretch = Stretch.Uniform;
            Grid grid = new Grid();

            grid.Tag = model;
            grid.Children.Add(image);

            return grid;

        }

        public static Color GetBorderColor(Color textColor)
        {
            var brightness = ((textColor.R * 299) + (textColor.G * 587) + (textColor.B * 114)) / 1000;
            return brightness > 70? Colors.Black: Colors.White;
        }
    }
}
