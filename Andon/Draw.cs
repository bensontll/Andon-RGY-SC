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

        public void DrawBorder(Label LabelText, String[] Border)
        {
            Color PenColor = EditLED.HEXtoC(Border[4]);
            Pen p = new Pen(PenColor, 1);

            Point plt = new Point(0, 0);
            Point prt = new Point(LabelText.Width - 1, 0);
            Point plb = new Point(0, LabelText.Height - 1);
            Point prb = new Point(LabelText.Width - 1, LabelText.Height - 1);
            Bitmap bt = new Bitmap(LabelText.Width, LabelText.Height);
            Graphics g = Graphics.FromImage(bt);
            if (Border[0] == "1")
            {
                g.DrawLine(p, plt, plb);
            }
            if (Border[1] == "1")
            {
                g.DrawLine(p, plt, prt);
            }
            if (Border[2] == "1")
            {
                g.DrawLine(p, prt, prb);
            }
            if (Border[3] == "1")
            {
                g.DrawLine(p, plb, prb);
            }
            LabelText.Image = bt;

        }

        public void DrawPicBorder(PictureBox Pic, String[] Border)
        {
            Color PenColor = EditLED.HEXtoC(Border[4]);
            Pen p = new Pen(PenColor, 1);

            Point plt = new Point(0, 0);
            Point prt = new Point(Pic.Width - 1, 0);
            Point plb = new Point(0, Pic.Height - 1);
            Point prb = new Point(Pic.Width - 1, Pic.Height - 1);

            Bitmap bt = new Bitmap(Application.StartupPath + Pic.Text);
            Pic.Image = bt;
            bt = new Bitmap(Pic.Image);
            Graphics g = Graphics.FromImage(bt);
            if (Border[0] == "1")
            {
                g.DrawLine(p, plt, plb);
            }
            if (Border[1] == "1")
            {
                g.DrawLine(p, plt, prt);
            }
            if (Border[2] == "1")
            {
                g.DrawLine(p, prt, prb);
            }
            if (Border[3] == "1")
            {
                g.DrawLine(p, plb, prb);
            }
            Pic.Image = bt;
        }

        public void Frame(Label LabelText, int CBool)
        {
            Color frameColor = Color.Gray;
            String strBack = LabelText.Tag.ToString().Split('#')[8];
            Color backColor = EditLED.HEXtoC(strBack);
            Pen pf = new Pen(frameColor, 1);
            pf.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Pen pc = new Pen(backColor, 1);

            Rectangle rect = new Rectangle(0, 0, LabelText.Width - 1, LabelText.Height - 1);

            //Bitmap bt = new Bitmap(LabelText.Image);
            Graphics g = Graphics.FromImage(LabelText.Image);
            if(CBool == 0)
            {
                String[] border = LabelText.Tag.ToString().Split('#');
                g.DrawRectangle(pc, rect);
                //LabelText.Image = bt;                
                DrawBorder(LabelText, border);
                LabelText.Visible = false;
                LabelText.Visible = true;
            }
            else if(CBool == 1)
            {
                g.DrawRectangle(pf, rect);
                //LabelText.Image = bt;
                LabelText.Visible = false;
                LabelText.Visible = true;
            }        
            
        }

        public void PicFrame (PictureBox Pb, int CBool)
        {
            Color frameColor = Color.Gray;
            String[] Border = Pb.Tag.ToString().Split('#');

            Pen pf = new Pen(frameColor, 1);
            pf.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

            Rectangle rect = new Rectangle(0, 0, Pb.Width - 1, Pb.Height - 1);

            if (CBool == 0)
            {
                Pb.Image = Image.FromFile(Application.StartupPath + Pb.Text);
                DrawPicBorder(Pb, Border);
            }
            else if (CBool == 1)
            {
                Bitmap bt = new Bitmap(Pb.Image);  
                Graphics g = Graphics.FromImage(bt);
                g.DrawRectangle(pf, rect);
                Pb.Image = bt;
            }
        }

        public Bitmap BlackBorder(PictureBox pb)
        {
            Bitmap bt = new Bitmap(pb.Width, pb.Height);
            Point plt = new Point(0, 0);
            Point prt = new Point(pb.Width - 1, 0);
            Point plb = new Point(0, pb.Height - 1);
            Point prb = new Point(pb.Width - 1, pb.Height - 1);
            Graphics g = Graphics.FromImage(bt);
            g.FillRectangle(Brushes.Black, 0, 0, bt.Width, bt.Height);
            String[] Border = pb.Tag.ToString().Split('#');
            Color PenColor = EditLED.HEXtoC(Border[4]);
            Pen p = new Pen(PenColor, 1);
            if (Border[0] == "1")
            {
                g.DrawLine(p, plt, plb);
            }
            if (Border[1] == "1")
            {
                g.DrawLine(p, plt, prt);
            }
            if (Border[2] == "1")
            {
                g.DrawLine(p, prt, prb);
            }
            if (Border[3] == "1")
            {
                g.DrawLine(p, plb, prb);
            }
            return bt;
            
        }

        public Bitmap ImageBorder(PictureBox pb)
        {
            Bitmap bt = new Bitmap(pb.Image);
            Point plt = new Point(0, 0);
            Point prt = new Point(pb.Width - 1, 0);
            Point plb = new Point(0, pb.Height - 1);
            Point prb = new Point(pb.Width - 1, pb.Height - 1);
            Graphics g = Graphics.FromImage(bt);
            String[] Border = pb.Tag.ToString().Split('#');
            Color PenColor = EditLED.HEXtoC(Border[4]);
            Pen p = new Pen(PenColor, 1);
            if (Border[0] == "1")
            {
                g.DrawLine(p, plt, plb);
            }
            if (Border[1] == "1")
            {
                g.DrawLine(p, plt, prt);
            }
            if (Border[2] == "1")
            {
                g.DrawLine(p, prt, prb);
            }
            if (Border[3] == "1")
            {
                g.DrawLine(p, plb, prb);
            }
            return bt;
        }

        public Bitmap BlackBorderF(PictureBox pb)
        {
            Bitmap bt = new Bitmap(pb.Width, pb.Height);
            Point plt = new Point(0, 0);
            Point prt = new Point(pb.Width - 1, 0);
            Point plb = new Point(0, pb.Height - 1);
            Point prb = new Point(pb.Width - 1, pb.Height - 1);
            Graphics g = Graphics.FromImage(bt);
            g.FillRectangle(Brushes.Black, 0, 0, bt.Width, bt.Height);
            String[] Border = pb.Tag.ToString().Split('#');
            Color PenColor = EditLED.HEXtoC(Border[4]);
            Pen p = new Pen(PenColor, 1);
            if (Border[0] == "1")
            {
                g.DrawLine(p, plt, plb);
            }
            if (Border[1] == "1")
            {
                g.DrawLine(p, plt, prt);
            }
            if (Border[2] == "1")
            {
                g.DrawLine(p, prt, prb);
            }
            if (Border[3] == "1")
            {
                g.DrawLine(p, plb, prb);
            }

            Color frameColor = Color.Gray;
            Pen pf = new Pen(frameColor, 1);
            pf.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Rectangle rect = new Rectangle(0, 0, pb.Width - 1, pb.Height - 1);
            g.DrawRectangle(pf, rect);
            return bt;
         }

        public Bitmap ImageBorderF(PictureBox pb)
        {
            Bitmap bt = new Bitmap(pb.Image);
            Point plt = new Point(0, 0);
            Point prt = new Point(pb.Width - 1, 0);
            Point plb = new Point(0, pb.Height - 1);
            Point prb = new Point(pb.Width - 1, pb.Height - 1);
            Graphics g = Graphics.FromImage(bt);
            String[] Border = pb.Tag.ToString().Split('#');
            Color PenColor = EditLED.HEXtoC(Border[4]);
            Pen p = new Pen(PenColor, 1);
            if (Border[0] == "1")
            {
                g.DrawLine(p, plt, plb);
            }
            if (Border[1] == "1")
            {
                g.DrawLine(p, plt, prt);
            }
            if (Border[2] == "1")
            {
                g.DrawLine(p, prt, prb);
            }
            if (Border[3] == "1")
            {
                g.DrawLine(p, plb, prb);
            }
            Color frameColor = Color.Gray;
            Pen pf = new Pen(frameColor, 1);
            pf.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
            Rectangle rect = new Rectangle(0, 0, pb.Width - 1, pb.Height - 1);
            g.DrawRectangle(pf, rect);
            return bt;
        }
    }
}
