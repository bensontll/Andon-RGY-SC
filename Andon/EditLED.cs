using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Andon
{
    public partial class EditLED : Form
    {
        Draw draw = new Draw();
        int textNo = 0;
        int mouseX;
        int mouseY;
        int picX;
        int picY;

        public EditLED()
        {
            InitializeComponent();
        }

        private void EditLEDForm_Load(object sender, EventArgs e)
        {
            Info.LEDWidth = 512;
            Info.LEDHeight = 512;
            comboBoxLineColor.DataSource = Info.colorText.Clone();
            comboBoxColor.DataSource = Info.colorText.Clone();
            draw.MapInitialize(pictureBoxBG);
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (textBoxWidth.Text != "" && textBoxHeight.Text != "")
            {
                pictureBoxBG.Width = int.Parse(textBoxWidth.Text);
                pictureBoxBG.Height = int.Parse(textBoxHeight.Text);
            }
            else
            {
                MessageBox.Show("输入不能为空");
            }
        }

        private void buttonText_Click(object sender, EventArgs e)
        {
            Color colorText = Color.Red;

            switch (comboBoxColor.Text)
            {
                case "R":
                    colorText = Color.Red;
                    break;
                case "G":
                    colorText = Color.Green;
                    break;
                case "Y":
                    colorText = Color.Yellow;
                    break;
                default:
                    break;
            }

            PictureBox pbText = new PictureBox();
            draw.DrawText(pbText, pictureBoxBG, textBoxContent.Text, int.Parse(textBoxSize.Text), textNo, int.Parse(textBoxX.Text), int.Parse(textBoxY.Text), colorText);
            pictureBoxBG.Controls.Add(pbText);
            pbText.BringToFront();
            pbText.MouseDown += new MouseEventHandler(this.MouseDown);
            pbText.MouseMove += new MouseEventHandler(this.MouseMove);
            pbText.MouseUp += new MouseEventHandler(this.MouseUp);
            pbText.MouseClick += new MouseEventHandler(this.MouseClick);
            textNo++;
            Info.PbText.Add(pbText);
        }

        private new void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseX = Cursor.Position.X;
                mouseY = Cursor.Position.Y;

                picX = ((PictureBox)sender).Left;
                picY = ((PictureBox)sender).Top;

            }

        }

        private new void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int y = Cursor.Position.Y - mouseY + picY;
                int x = Cursor.Position.X - mouseX + picX;

                ((PictureBox)sender).Top = y;
                ((PictureBox)sender).Left = x;

            }
        }

        private new void MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseX = 0;
                mouseY = 0;
                if (((PictureBox)sender).Location.X < 0)
                {
                    ((PictureBox)sender).Left = 0;
                }
                if (((PictureBox)sender).Location.Y < 0)
                {
                    ((PictureBox)sender).Top = 0;
                }
                if ((((PictureBox)sender).Left + ((PictureBox)sender).Width) > this.pictureBoxBG.Width)
                {
                    ((PictureBox)sender).Left = this.pictureBoxBG.Width - ((PictureBox)sender).Width;
                }
                if ((((PictureBox)sender).Top + ((PictureBox)sender).Height) > this.pictureBoxBG.Height)
                {
                    ((PictureBox)sender).Top = this.pictureBoxBG.Height - ((PictureBox)sender).Height;
                }

            }

        }



        private new void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                StringBuilder sbName = new StringBuilder(((PictureBox)sender).Tag.ToString());
                int count = Info.PbText.Count;
                for (int i = 0; i < count; i++)
                {
                    if (Info.PbText[i].Tag.ToString() == sbName.ToString())
                    {
                        Info.TextNo = i;
                    }
                }
                ControlInfo controlInfo = new ControlInfo();
                controlInfo.ShowDialog();
            }

        }

        private void buttonLine_Click(object sender, EventArgs e)
        {
            Color colorLine = Color.Green;
            String[] Line = new String[5];//0.sx ,1.sy ,2.ex ,3.ey ,4.color

            Line[0] = textBoxLSX.Text;
            Line[1] = textBoxLSY.Text;
            Line[2] = textBoxLEX.Text;
            Line[3] = textBoxLEY.Text;
            switch (comboBoxLineColor.Text)
            {
                case "R":
                    colorLine = Color.Red;
                    Line[4] = "R";
                    break;
                case "G":
                    colorLine = Color.Green;
                    Line[4] = "G";
                    break;
                case "Y":
                    colorLine = Color.Yellow;
                    Line[4] = "Y";
                    break;
                default:
                    break;
            }
            draw.DrawLine(pictureBoxBG, colorLine, int.Parse(textBoxLSX.Text), int.Parse(textBoxLSY.Text), int.Parse(textBoxLEX.Text), int.Parse(textBoxLEY.Text));
            pictureBoxBG.Image = pictureBoxBG.Image;
            Info.Line.Add(Line);
            int count = Info.Line.Count;
            String[] listLine = new String[count];
            for (int i = 0; i < count; i++)
            {
                String line = "起始点：" + Info.Line[i][0] + "," + Info.Line[i][1] + ";" + "终止点：" + Info.Line[i][2] + "," + Info.Line[i][3] + ";" + "颜色：" + Info.Line[i][4];
                listLine[i] = line;
            }
            comboBoxLine.DataSource = listLine;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int LineNo = comboBoxLine.SelectedIndex;
            String[] Line = Info.Line[LineNo];
            draw.DrawLine(pictureBoxBG, Color.Black, int.Parse(Line[0]), int.Parse(Line[1]), int.Parse(Line[2]), int.Parse(Line[3]));
            pictureBoxBG.Image = pictureBoxBG.Image;
            Info.Line.RemoveAt(LineNo);

            int count = Info.Line.Count;
            String[] listLine = new String[count];
            for (int i = 0; i < count; i++)
            {
                String line = "起始点：" + Info.Line[i][0] + "," + Info.Line[i][1] + ";" + "终止点：" + Info.Line[i][2] + "," + Info.Line[i][3] + ";" + "颜色：" + Info.Line[i][4];
                listLine[i] = line;
            }
            comboBoxLine.DataSource = listLine;
        }
    }
}
