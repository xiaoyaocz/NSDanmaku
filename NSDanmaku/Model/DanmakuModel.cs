using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace NSDanmaku.Model
{
    public enum DanmakuLocation
    {
        Roll,//滚动
        Top,//顶部
        Bottom,//底部
        Other//其他类型，如高级弹幕自定义位置
    }
    public enum DanmakuSite
    {
        Bilibili,
        Acfun
    }
    public enum DanmakuBorderStyle
    {
        Default,
        NoBorder,
        Shadow
    }
    public enum DanmakuMode
    {
        Video,
        Live
    }
    public class DanmakuModel
    {
        public string text { get; set; }
        /// <summary>
        /// 弹幕大小
        /// </summary>
        public double size { get; set; }
        /// <summary>
        /// 弹幕颜色
        /// </summary>
        public Color color { get; set; }
        /// <summary>
        /// 弹幕出现时间
        /// </summary>
        public double time { get; set; }
        /// <summary>
        /// 弹幕发送时间
        /// </summary>
        public string sendTime { get; set; }
        /// <summary>
        /// 弹幕池
        /// </summary>
        public string pool { get; set; }
        /// <summary>
        /// 弹幕发送人ID
        /// </summary>
        public string sendID { get; set; }
        /// <summary>
        /// 弹幕ID
        /// </summary>
        public string rowID { get; set; }
        /// <summary>
        /// 弹幕出现位置
        /// </summary>
        public DanmakuLocation location
        {
            get;set;
        }

        public DanmakuSite fromSite { get; set; }

        public string source { get; set; }
    }
}
