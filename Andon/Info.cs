using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Andon
{
    class Info
    {
        public static List<PictureBox> PbText = new List<PictureBox>();
        public static int TextNo;
        public static List<String[]> Line = new List<String[]>();
        public static int LEDWidth;
        public static int LEDHeight;
        public static String Content;
        public static String Color;
        public static String[] colorText = { "R", "G", "Y" };
    }
}
