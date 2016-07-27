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
    public partial class ControlInfo : Form
    {
        public ControlInfo()
        {
            InitializeComponent();
        }
        PictureBox pb;
        Draw draw = new Draw();
        private void ControlInfo_Load(object sender, EventArgs e)
        {
            comboBoxColor.DataSource = Info.colorText.Clone();
            pb = Info.PbText[Info.TextNo];
            textBoxContent.Text = pb.Name;
            textBoxX.Text = pb.Left.ToString();
            textBoxY.Text = pb.Top.ToString();
            textBoxSize.Text = (pb.Height / 0.8).ToString();
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            pb.Left = int.Parse(textBoxX.Text);
            pb.Top = int.Parse(textBoxY.Text);
            Color colorText = Color.Green;

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
            }
            draw.reText(pb, textBoxContent.Text, colorText, float.Parse(textBoxX.Text), float.Parse(textBoxY.Text), Convert.ToInt32(textBoxSize.Text));

        }
    }
}
