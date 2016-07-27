using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Andon
{
    class Draw
    {
        public void MapInitialize(PictureBox PbBG)
        {
            PbBG.Top = 0;
            PbBG.Left = 0;
            PbBG.Width = Info.LEDWidth;
            PbBG.Height = Info.LEDHeight;
            Bitmap btMap = new Bitmap(PbBG.Width, PbBG.Height);
            Graphics gMap = Graphics.FromImage((Image)btMap);
            PbBG.BackColor = Color.Black;
            PbBG.Image = (Image)btMap;

            //PbScale.Image = btLine;
        }

        public void DrawText(PictureBox PbText, PictureBox Parent, String Content, int TextSize, int TextNo, int Left, int Top, Color ColorText)
        {
            PbText.Name = Content;
            string textC = "R";
            switch (ColorText.Name)
            {
                case "Red":
                    textC = "R";
                    break;
                case "Green":
                    textC = "G";
                    break;
                case "Yellow":
                    textC = "Y";
                    break;
            }

            PbText.Tag = "TE" + TextNo.ToString() + "#" + textC;
            PbText.BackColor = Color.Transparent;
            PbText.Parent = Parent;
            StringBuilder sb = new StringBuilder(Content);
            double width = sb.Length * TextSize;
            double height = TextSize;
            PbText.Width = (int)width;
            PbText.Height = (int)height;
            Bitmap btText = new Bitmap((int)width, (int)height);
            PbText.Image = btText;
            Graphics gText = Graphics.FromImage(btText);
            double fontSize = TextSize * 0.75;
            Font font = new Font("宋体", (float)fontSize);
            SolidBrush sbrush = new SolidBrush(ColorText);
            gText.DrawString(PbText.Name, font, sbrush, 0, 0);
            PbText.Top = Top;
            PbText.Left = Left;
            PbText.SizeMode = PictureBoxSizeMode.StretchImage;

        }

        public void reText(PictureBox PbText, String Text, Color ColorText, float LeftMeter, float TopMeter, int TextSize)
        {
            string textC = "R";
            switch (ColorText.Name)
            {
                case "Red":
                    textC = "R";
                    break;
                case "Green":
                    textC = "G";
                    break;
                case "Yellow":
                    textC = "Y";
                    break;
            }

            StringBuilder sbTag = new StringBuilder(PbText.Tag.ToString());
            sbTag.Remove(sbTag.Length - 1, 1);
            sbTag.Append(textC);
            PbText.Tag = sbTag.ToString();

            StringBuilder sb = new StringBuilder(Text);
            double width = sb.Length * 0.8 * TextSize;
            double height = 0.8 * TextSize;
            Bitmap btText = new Bitmap((int)width, (int)height);
            Graphics g = Graphics.FromImage(btText);
            g.Clear(Color.Transparent);
            SolidBrush sbrush = new SolidBrush(ColorText);
            double fontSize = TextSize * 0.533;
            Font fontText = new Font("宋体", (float)fontSize);
            g.DrawString(Text, fontText, sbrush, 0, 0);
            PbText.Image = btText;
            PbText.Name = Text;
            PbText.Left = (int)(LeftMeter);
            PbText.Top = (int)(TopMeter);
            PbText.Width = (int)width;
            PbText.Height = (int)height;

        }

        public void DrawLine(PictureBox PbMap, Color ColorLine, int SX, int SY, int EX, int EY)
        {
            Graphics gMap = Graphics.FromImage(PbMap.Image);
            Pen penLine = new Pen(ColorLine, 1);
            gMap.DrawLine(penLine, SX, SY, EX, EY);
        }
    }
}
