using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScrollingTextControl;

namespace Andon
{
    class Info
    {
        public static List<Label> LabelText = new List<Label>();//单元格标签
        public static List<Label> BlinkFont = new List<Label>();//闪烁字体标签
        public static List<Label> BlinkBack = new List<Label>();//闪烁背景标签
        public static List<Label> BlinkAll = new List<Label>();//闪烁所有字体和毕竟
        public static List<Label> LabelDT = new List<Label>();//时间标签
        public static List<ScrollingText> RollText = new List<ScrollingText>();//滚动字标签
        public static List<PictureBox> PicBox = new List<PictureBox>();//图片标签
        public static List<String> BlinkPicName = new List<String>();//闪图图片
        public static List<String> DT = new List<String>();

        public static List<Color> ColorFont = new List<Color>();
        public static List<Color> ColorBack = new List<Color>();
        public static List<Color> ColorAllF = new List<Color>();
        public static List<Color> ColorAllB = new List<Color>();
        public static List<Bitmap> BtBitmap = new List<Bitmap>();
        public static List<Bitmap> BtBlack = new List<Bitmap>();
        public static List<Bitmap> BtBitmapF = new List<Bitmap>();
        public static List<Bitmap> BtBlackF = new List<Bitmap>();

        public static int TextNo;
        public static String LabelStyle;
        public static String LEDStyle;

        public static String Name;
        public static int LEDWidth;
        public static int LEDHeight;
        public static int UTCTime;

        public static String[] TwoColor = { "红色", "黄色", "绿色", "黑色" };
        //public static String[] DTModel = { "年-月-日 时:分:秒", "年-月-日 时:分", "月-日 时:分:秒", "月-日 时:分","年-月-日","月-日", "时:分:秒", "时:分"};
        public static String[] DTModel = {  "时:分:秒" };
        public static String[] FullColor = { "红色", "黄色", "绿色", "蓝色","青色","紫色","白色","黑色" };
        public static String[] BlinkPicList = { "无", "闪烁" };
        public static String[] BlinkList = { "无", "字体闪烁", "背景闪烁", "同时闪烁"};
    }
}
