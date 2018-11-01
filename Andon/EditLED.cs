using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using ScrollingTextControl;
using System.IO;
using OperateIni;
using TIPSTCPDLL;

namespace Andon
{
    public partial class EditLED : Form
    {
        Draw draw = new Draw();
        OperatDB operateDB = new OperatDB();
        TIPStcpDll tcpDll = new TIPStcpDll();
        Key key = new Key();
        int textNo = 0;
        int NameNo = 1;
        int mouseX;
        int mouseY;
        int picX;
        int picY;
        int selectedItem = 0;
        int lastItem = 0;
        Thread thBling;//闪烁线程

        public EditLED()
        {
            InitializeComponent();
        }

        private void EditLEDForm_Load(object sender, EventArgs e)
        {            
            Info.LEDWidth = 512;
            Info.LEDHeight = 128;
            textBoxWidth.Text = Info.LEDWidth.ToString();
            textBoxHeight.Text = Info.LEDHeight.ToString();
            controlEnabled(false);

            comboBoxColor.DataSource = Info.TwoColor.Clone();
            comboBoxBColor.DataSource = Info.TwoColor.Clone();
            comboBoxFColor.DataSource = Info.TwoColor.Clone();
            comboBoxRB.DataSource = Info.BlinkList.Clone();
            comboBoxDT.DataSource = Info.DTModel.Clone();
            ConnDB.Conn.Open();
            ListViewInitialize(0);
            if(listViewProject.SelectedItems.Count > 0)
            {
                controlEnabled(true);
                Info.UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(listViewProject.SelectedItems[0].SubItems[0].Text));
                LoadProject(Info.UTCTime);
            }
            draw.MapInitialize(pictureBoxBG);
            thBling = new Thread(blink);
            thBling.Start();
        }


        private void listViewProject_MouseClick(object sender, MouseEventArgs e)
        {
            listViewChange(listViewProject, lastItem);
            if (listViewProject.SelectedItems.Count != 0)
            {
                selectedItem = listViewProject.SelectedItems[0].Index;
                if (textBoxName.Text != "")
                {
                    Save();               
                }
                else
                {
                    MessageBox.Show("样式名称不能为空");
                }
                lastItem = selectedItem;
                controlEnabled(true);
                Info.UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(listViewProject.SelectedItems[0].SubItems[0].Text));
                LoadProject(Info.UTCTime);
            }
            else if (listViewProject.SelectedItems.Count == 0)
            {
                //listViewProject.SelectedItems[0]
            }
        }

        /// <summary>
        /// 修改行数保存地址
        /// </summary>
        /// <param name="listView">listView</param>
        /// <param name="k">行数</param>
        private void listViewChange(ListView listView ,int k)
        {
            listView.Items[k].SubItems[1].Text = textBoxName.Text;
            listView.Items[k].SubItems[2].Text = Info.LEDWidth.ToString();
            listView.Items[k].SubItems[3].Text = Info.LEDHeight.ToString();
        }

        /// <summary>
        /// 添加时间标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDT_Click(object sender, EventArgs e)
        {
            Label pbDT = new Label();
            pbDT.Name = textNo.ToString();
            String strDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            pbDT.Text = strDT;
            pbDT.Width = 150;
            pbDT.TextAlign = ContentAlignment.MiddleCenter;
            pbDT.ForeColor = Color.FromArgb(255, 0, 0);
            pbDT.BackColor = Color.FromArgb(0, 0, 0);
            pbDT.Font = new Font("宋体", 10);
            pbDT.Tag = "0#0#0#0#FF0000#3#yyyy-MM-dd HH:mm:ss#FF0000#000000";
            pbDT.Image = new Bitmap(pbDT.Width, pbDT.Height);
            pictureBoxBG.Controls.Add(pbDT);
            pbDT.BringToFront();
            pbDT.MouseDown += new MouseEventHandler(this.mouseDown);
            pbDT.MouseMove += new MouseEventHandler(this.mouseMove);
            pbDT.MouseUp += new MouseEventHandler(this.mouseUp);
            pbDT.MouseClick += new MouseEventHandler(this.mouseClick);
            textNo++;
            Info.LabelDT.Add(pbDT);

        }

        /// <summary>
        /// 添加单元格标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonText_Click(object sender, EventArgs e)
        {
            Label Lb = new Label();
            Lb.Name = textNo.ToString();
            Lb.Text = "标签" + textNo;
            Lb.TextAlign = ContentAlignment.MiddleCenter;
            Lb.ForeColor = Color.FromArgb(255, 0, 0);
            Lb.BackColor = Color.FromArgb(0, 0, 0);

            Lb.Font = new Font("宋体", 10);

            Lb.Tag = "0#0#0#0#FF0000#0#0#FF0000#000000";//一至四位依次为 左，上，右，下。0代表无边框，1代表有边框，第五位为边框颜色,第六位 类型，0单元格，1滚动条，2图片，第七位为闪烁，0不闪，1文字闪，2背景闪，3一起闪，第八位为fontcolor，第九位为backcolor。
            Lb.Image = new Bitmap(Lb.Width, Lb.Height);
            //Bitmap bt = new Bitmap(Lb)
            pictureBoxBG.Controls.Add(Lb);
            Lb.BringToFront();
            Lb.MouseDown += new MouseEventHandler(this.mouseDown);
            Lb.MouseMove += new MouseEventHandler(this.mouseMove);
            Lb.MouseUp += new MouseEventHandler(this.mouseUp);
            Lb.MouseClick += new MouseEventHandler(this.mouseClick);
            textNo++;
            Info.LabelText.Add(Lb);
        }


        /// <summary>
        /// 添加滚动字标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonScroll_Click(object sender, EventArgs e)
        {
            ScrollingText rollText = new ScrollingText();
            rollText.Name = textNo.ToString();
            rollText.ScrollText = "标签" + textNo;
            rollText.ForeColor = Color.Red;
            rollText.BackColor = Color.Black;
            rollText.Font = new Font("宋体", 10);
            Bitmap bt = new Bitmap(75, 20);
            rollText.Tag = "0#0#0#0#FF0000#1#0#FF0000#000000";//一至四位依次为 左，上，右，下。0代表无边框，1代表有边框，第五位为边框颜色,第六位 类型，0单元格，1滚动条，2图片，第七位为闪烁，0不闪，1文字闪，2背景闪，3一起闪，第八位为fontcolor，第九位为backcolor。
            pictureBoxBG.Controls.Add(rollText);
            rollText.BringToFront();
            rollText.MouseDown += new MouseEventHandler(this.mouseDown);
            rollText.MouseMove += new MouseEventHandler(this.mouseMove);
            rollText.MouseUp += new MouseEventHandler(this.mouseUp);
            rollText.MouseClick += new MouseEventHandler(this.mouseClick);
            textNo++;
            Info.RollText.Add(rollText);
        }

        /// <summary>
        /// 添加图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPic_Click(object sender, EventArgs e)
        {
            openFileDialogPic.Filter = "位图文件(*.png)|*.png";
            openFileDialogPic.FilterIndex = 1;
            if (openFileDialogPic.ShowDialog() == DialogResult.OK)
            {
                String SourcePath = openFileDialogPic.FileName;
                String DirePath = Application.StartupPath + "\\Pic\\" + Info.UTCTime.ToString();
                if (!Directory.Exists(DirePath))
                {
                    Directory.CreateDirectory(DirePath);
                }
                String DestPath = DirePath + "\\" + textNo + ".png";
                File.Copy(SourcePath, DestPath, true);
                PictureBox pb = new PictureBox();
                pb.Name = textNo.ToString();
                pb.Text = "\\Pic\\" + Info.UTCTime.ToString() + "\\" + textNo + ".png";
                pb.Image = new Bitmap(DestPath);
                pb.SizeMode = PictureBoxSizeMode.CenterImage;
                //pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Tag = "0#0#0#0#FF0000#2#0#000000#000000";//一至四位依次为 左，上，右，下。0代表无边框，1代表有边框，第五位为边框颜色,第六位 类型，0单元格，1滚动条，2图片，第七位为闪烁，0不闪，1文字闪，2背景闪，3一起闪，第八位为fontcolor，第九位为backcolor。
                pb.BackColor = Color.Transparent;
                pb.Parent = pictureBoxBG;
                pictureBoxBG.Controls.Add(pb);
                pb.BringToFront();
                pb.MouseDown += new MouseEventHandler(this.mouseDown);
                pb.MouseMove += new MouseEventHandler(this.mouseMove);
                pb.MouseUp += new MouseEventHandler(this.mouseUp);
                pb.MouseClick += new MouseEventHandler(this.mouseClick);
                textNo++;
                                
                Info.PicBox.Add(pb);
                Info.BtBlack.Add(draw.BlackBorder(pb));
                Info.BtBlackF.Add(draw.BlackBorderF(pb));
                Info.BtBitmap.Add(draw.ImageBorder(pb));
                Info.BtBitmapF.Add(draw.ImageBorderF(pb));

            }

         
        }

        /// <summary>
        /// 修改LED长宽高和名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChange_Click(object sender, EventArgs e)
        {
            String width = textBoxWidth.Text;
            String height = textBoxHeight.Text;
            if (textBoxWidth.Text != "" && textBoxHeight.Text != "")
            {
                if (int.Parse(textBoxWidth.Text) % 8 == 0)
                {
                    Info.LEDWidth = int.Parse(textBoxWidth.Text);
                    Info.LEDHeight = int.Parse(textBoxHeight.Text);
                    pictureBoxBG.Width = Info.LEDWidth;
                    pictureBoxBG.Height = Info.LEDHeight;
                }
                else
                {
                    MessageBox.Show("宽度必须为8的倍数！");
                    textBoxWidth.Text = Info.LEDWidth.ToString();
                    textBoxHeight.Text = Info.LEDHeight.ToString();
                }

            }
            else
            {
                MessageBox.Show("输入不能为空");
                textBoxWidth.Text = Info.LEDWidth.ToString();
                textBoxHeight.Text = Info.LEDHeight.ToString();
            }
        }

        /// <summary>
        /// 确认按钮修改便签参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLConfirm_Click(object sender, EventArgs e)
        {

            if (textBoxContent.Text != "" && textBoxLWidth.Text != "" && textBoxLHeight.Text != "" && textBoxX.Text != "" && textBoxY.Text != "")
            {
                if (int.Parse(textBoxLWidth.Text) > Info.LEDWidth || int.Parse(textBoxLHeight.Text) > Info.LEDHeight)
                {
                    MessageBox.Show("单元格的长度或宽度不能大于LED屏幕！");
                }
                else if (int.Parse(textBoxLWidth.Text) + int.Parse(textBoxX.Text) > Info.LEDWidth || int.Parse(textBoxLHeight.Text) + int.Parse(textBoxY.Text) > Info.LEDHeight)
                {
                    MessageBox.Show("单元格位置超过LED屏幕范围！");
                }
                else
                {
                    if (Info.LabelStyle == "Label")
                    {
                        if (Info.LabelText.Count > 0 && Info.LabelText[Info.TextNo] != null)
                        {

                            Label pbdt = Info.LabelText[Info.TextNo];

                            pbdt.Text = textBoxContent.Text;
                            pbdt.Left = int.Parse(textBoxX.Text);
                            pbdt.Top = int.Parse(textBoxY.Text);
                            pbdt.Width = int.Parse(textBoxLWidth.Text);
                            pbdt.Height = int.Parse(textBoxLHeight.Text);
                            pbdt.Font = new Font("宋体", int.Parse(numericUpDownFont.Text));
                            pbdt.ForeColor = StoC(comboBoxColor.Text);
                            pbdt.BackColor = StoC(comboBoxBColor.Text);

                            StringBuilder sbBorder = new StringBuilder();

                            int count = checkedListBoxBorder.Items.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (checkedListBoxBorder.GetItemChecked(i))
                                {
                                    sbBorder.Append("1#");
                                }
                                else
                                {
                                    sbBorder.Append("0#");
                                }
                            }

                            sbBorder.Append(CtoHEX(StoC(comboBoxFColor.Text)));

                            Info.BlinkFont.Remove(pbdt);
                            Info.BlinkBack.Remove(pbdt);
                            Info.BlinkAll.Remove(pbdt);
                            switch (comboBoxRB.Text)
                            {
                                case "无":
                                    sbBorder.Append("#0#0");
                                    break;
                                case "字体闪烁":
                                    sbBorder.Append("#0#1");
                                    Info.BlinkFont.Add(pbdt);
                                    break;
                                case "背景闪烁":
                                    sbBorder.Append("#0#2");
                                    Info.BlinkBack.Add(pbdt);
                                    break;
                                case "同时闪烁":
                                    sbBorder.Append("#0#3");
                                    Info.BlinkAll.Add(pbdt);
                                    break;
                            }


                            sbBorder.Append("#");
                            sbBorder.Append(CtoHEX(StoC(comboBoxColor.Text)));
                            sbBorder.Append("#");
                            sbBorder.Append(CtoHEX(StoC(comboBoxBColor.Text)));

                            pbdt.Tag = sbBorder.ToString();

                            String[] borderInfo = sbBorder.ToString().Split('#');
                            draw.DrawBorder(pbdt, borderInfo);
                            draw.Frame(pbdt, 1);
                        }
                    }
                    else if (Info.LabelStyle == "LabelDT")
                    {
                        if (Info.LabelDT.Count > 0 && Info.LabelDT[Info.TextNo] != null)
                        {

                            Label pbdt = Info.LabelDT[Info.TextNo];

                            pbdt.Text = DateTime.Now.ToString(DTtoS(comboBoxDT.Text));
                            pbdt.Left = int.Parse(textBoxX.Text);
                            pbdt.Top = int.Parse(textBoxY.Text);
                            pbdt.Width = int.Parse(textBoxLWidth.Text);
                            pbdt.Height = int.Parse(textBoxLHeight.Text);
                            pbdt.Font = new Font("宋体", int.Parse(numericUpDownFont.Text));
                            pbdt.ForeColor = StoC(comboBoxColor.Text);
                            pbdt.BackColor = StoC(comboBoxBColor.Text);

                            StringBuilder sbBorder = new StringBuilder();

                            int count = checkedListBoxBorder.Items.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (checkedListBoxBorder.GetItemChecked(i))
                                {
                                    sbBorder.Append("1#");
                                }
                                else
                                {
                                    sbBorder.Append("0#");
                                }
                            }

                            sbBorder.Append(CtoHEX(StoC(comboBoxFColor.Text)));
                            sbBorder.Append("#3#");
                            sbBorder.Append(DTtoS(comboBoxDT.Text));
                            sbBorder.Append("#");
                            sbBorder.Append(CtoHEX(StoC(comboBoxColor.Text)));
                            sbBorder.Append("#");
                            sbBorder.Append(CtoHEX(StoC(comboBoxBColor.Text)));

                            pbdt.Tag = sbBorder.ToString();

                            String[] borderInfo = sbBorder.ToString().Split('#');
                            draw.DrawBorder(pbdt, borderInfo);
                            draw.Frame(pbdt, 1);
                        }
                    }
                    else if (Info.LabelStyle == "ScrollingText")
                    {
                        if (Info.RollText.Count > 0 && Info.RollText[Info.TextNo] != null)
                        {

                            ScrollingText rollText = Info.RollText[Info.TextNo];

                            rollText.ScrollText = textBoxContent.Text;
                            rollText.Left = int.Parse(textBoxX.Text);
                            rollText.Top = int.Parse(textBoxY.Text);
                            rollText.Width = int.Parse(textBoxLWidth.Text);
                            rollText.Height = int.Parse(textBoxLHeight.Text);
                            rollText.Font = new Font("宋体", int.Parse(numericUpDownFont.Text));

                            rollText.ForeColor = StoC(comboBoxColor.Text);
                            rollText.BackColor = StoC(comboBoxBColor.Text);


                            StringBuilder sbInfo = new StringBuilder("0#0#0#0#FF0000#1#0");

                            sbInfo.Append("#");
                            sbInfo.Append(CtoHEX(StoC(comboBoxColor.Text)));
                            sbInfo.Append("#");
                            sbInfo.Append(CtoHEX(StoC(comboBoxBColor.Text)));



                            rollText.Tag = sbInfo.ToString();

                            String[] borderInfo = sbInfo.ToString().Split('#');
                        }
                    }
                    else if (Info.LabelStyle == "PictureBox")
                    {
                        if (Info.PicBox.Count > 0 && Info.PicBox[Info.TextNo] != null)
                        {
                            PictureBox pb = Info.PicBox[Info.TextNo];

                            pb.Left = int.Parse(textBoxX.Text);
                            pb.Top = int.Parse(textBoxY.Text);
                            pb.Width = int.Parse(textBoxLWidth.Text);
                            pb.Height = int.Parse(textBoxLHeight.Text);

                            StringBuilder sbBorder = new StringBuilder();

                            int count = checkedListBoxBorder.Items.Count;
                            for (int i = 0; i < count; i++)
                            {
                                if (checkedListBoxBorder.GetItemChecked(i))
                                {
                                    sbBorder.Append("1#");
                                }
                                else
                                {
                                    sbBorder.Append("0#");
                                }
                            }
                            sbBorder.Append(CtoHEX(StoC(comboBoxFColor.Text)));



                            Info.BlinkPicName.Remove(pb.Name);
                            switch (comboBoxRB.Text)
                            {
                                case "无":
                                    sbBorder.Append("#2#0");
                                    break;
                                case "闪烁":
                                    sbBorder.Append("#2#1");
                                    Info.BlinkPicName.Add(pb.Name);
                                    break;
                            }

                            sbBorder.Append("#");
                            sbBorder.Append("FF0000");
                            sbBorder.Append("#");
                            sbBorder.Append("000000");

                            pb.Tag = sbBorder.ToString();

                            String[] borderInfo = sbBorder.ToString().Split('#');
                            draw.DrawPicBorder(pb, borderInfo);

                            Info.BtBlack.RemoveAt(Info.TextNo);
                            Info.BtBlack.Insert(Info.TextNo, draw.BlackBorder(pb));

                            Info.BtBitmap.RemoveAt(Info.TextNo);
                            Info.BtBitmap.Insert(Info.TextNo, draw.ImageBorder(pb));

                            Info.BtBlackF.RemoveAt(Info.TextNo);
                            Info.BtBlackF.Insert(Info.TextNo, draw.BlackBorderF(pb));

                            Info.BtBitmapF.RemoveAt(Info.TextNo);
                            Info.BtBitmapF.Insert(Info.TextNo, draw.ImageBorderF(pb));
                            draw.PicFrame(pb, 1);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("参数不能为空！");
            }
        }

        /// <summary>
        /// 删除按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLDelete_Click(object sender, EventArgs e)
        {
            if (Info.LabelStyle == "Label")
            {
                if (Info.LabelText.Count > 0 && Info.LabelText[Info.TextNo] != null)
                {
                    Info.LabelText[Info.TextNo].Dispose();
                    Info.LabelText.RemoveAt(Info.TextNo);
                }
            }
            if (Info.LabelStyle == "ScrollingText")
            {
                if (Info.RollText.Count > 0 && Info.RollText[Info.TextNo] != null)
                {
                    Info.RollText[Info.TextNo].Dispose();
                    Info.RollText.RemoveAt(Info.TextNo);
                }
            }
            if (Info.LabelStyle == "PictureBox")
            {
                if (Info.PicBox.Count > 0 && Info.PicBox[Info.TextNo] != null)
                {
                    String filePath = Application.StartupPath + Info.PicBox[Info.TextNo].Text;
                    Info.PicBox[Info.TextNo].Dispose();
                    

                    foreach (String PicName in Info.BlinkPicName )
                    {
                        if(Info.PicBox[Info.TextNo].Name == PicName)
                        Info.BlinkPicName.Remove(Info.PicBox[Info.TextNo].Name);
                        break;
                    }
                    Info.PicBox.RemoveAt(Info.TextNo);
                    Info.BtBlack.RemoveAt(Info.TextNo);
                    Info.BtBlackF.RemoveAt(Info.TextNo);
                    Info.BtBitmap.RemoveAt(Info.TextNo);
                    Info.BtBitmapF.RemoveAt(Info.TextNo);
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if(textBoxName.Text != "")
            {
                Save();
                selectedItem = listViewProject.SelectedItems[0].Index;
                listViewChange(listViewProject, selectedItem);
            }
            else
            {
                MessageBox.Show("样式名称不能为空");
            }
        }

        private void Save()
        {
            try
            {
                if (ConnDB.Conn.State != ConnectionState.Open)
                {
                    ConnDB.Conn.Open();
                }

                Info.Name = textBoxName.Text;
                operateDB.UpdateProject(ConnDB.Conn, Info.Name, Info.UTCTime, Info.LEDWidth, Info.LEDHeight);
                operateDB.ClearInfo(ConnDB.Conn, Info.UTCTime);
                if (Info.LabelText.Count != 0)
                {
                    operateDB.InserInfo(ConnDB.Conn, Info.UTCTime, Info.LabelText);
                }
                if (Info.RollText.Count != 0)
                {
                    operateDB.InserRInfo(ConnDB.Conn, Info.UTCTime, Info.RollText);
                }
                if (Info.PicBox.Count != 0)
                {
                    operateDB.InserPInfo(ConnDB.Conn, Info.UTCTime, Info.PicBox);
                }
                if (Info.LabelDT.Count != 0)
                {
                    operateDB.InserInfo(ConnDB.Conn, Info.UTCTime, Info.LabelDT);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库丢失！");
            }
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {            
            textBoxName.Text = "样式" + NameNo;
            Info.UTCTime = UTC.ConvertDateTimeLong(DateTime.Now);
            Info.LEDWidth = 512;
            Info.LEDHeight = 128;
            try
            {
                if (ConnDB.Conn.State != ConnectionState.Open)
                {
                    ConnDB.Conn.Open();
                }
                operateDB.InsertProject(ConnDB.Conn, textBoxName.Text, Info.UTCTime, Info.LEDWidth, Info.LEDHeight, "T");
                operateDB.CreateInfo(ConnDB.Conn, Info.UTCTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库丢失！");
            }
            int countNew = listViewProject.Items.Count;
            listViewProject.Clear();
            ListViewInitialize(countNew);
            NameNo++;
            controlEnabled(true);
            ClearLabel();

            draw.MapInitialize(pictureBoxBG);
            Thread.Sleep(1000);
            
        }

        private void buttonNewF_Click(object sender, EventArgs e)
        {
            textBoxName.Text = "样式" + NameNo;
            Info.UTCTime = UTC.ConvertDateTimeLong(DateTime.Now);
            Info.LEDWidth = 512;
            Info.LEDHeight = 128;
            try
            {
                if (ConnDB.Conn.State != ConnectionState.Open)
                {
                    ConnDB.Conn.Open();
                }
                operateDB.InsertProject(ConnDB.Conn, textBoxName.Text, Info.UTCTime, Info.LEDWidth, Info.LEDHeight, "F");
                operateDB.CreateInfo(ConnDB.Conn, Info.UTCTime);
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库丢失！");
            }
            int countNew = listViewProject.Items.Count;
            listViewProject.Clear();
            ListViewInitialize(countNew);
            NameNo++;
            controlEnabled(true);
            ClearLabel();

            draw.MapInitialize(pictureBoxBG);
            Thread.Sleep(1000);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if(Info.UTCTime != 0)
            {
                if (ConnDB.Conn.State != ConnectionState.Open)
                {
                    ConnDB.Conn.Open();
                }
                operateDB.DeleteProject(ConnDB.Conn, Info.UTCTime);
                operateDB.DropInfo(ConnDB.Conn, Info.UTCTime);
                ClearLabel();
                if(Directory.Exists(Application.StartupPath + "\\Pic\\" + Info.UTCTime.ToString()))
                {
                    Directory.Delete(Application.StartupPath + "\\Pic\\" + Info.UTCTime.ToString(), true);
                }

                controlEnabled(false);
                listViewProject.Clear();
                ListViewInitialize(0);
                if (listViewProject.SelectedItems.Count > 0)
                {
                    controlEnabled(true);
                    Info.UTCTime = UTC.ConvertDateTimeLong(Convert.ToDateTime(listViewProject.SelectedItems[0].SubItems[0].Text));
                    LoadProject(Info.UTCTime);
                }
            }
            lastItem = 0;           

        }

        private void textBoxWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            key.IntNumber(sender, e);
        }

        private void textBoxHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            key.IntNumber(sender, e);
        }

        private void textBoxX_KeyPress(object sender, KeyPressEventArgs e)
        {
            key.IntNumber(sender, e);
        }

        private void textBoxY_KeyPress(object sender, KeyPressEventArgs e)
        {
            key.IntNumber(sender, e);
        }

        private void textBoxLWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            key.IntNumber(sender, e);
        }

        private void textBoxLHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            key.IntNumber(sender, e);
        }

        //拖动控件按下鼠标的操作
        private void mouseDown(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                if (sender.GetType().Name == "Label")
                {
                    mouseX = Cursor.Position.X;
                    mouseY = Cursor.Position.Y;

                    picX = ((Label)sender).Left;
                    picY = ((Label)sender).Top;


                }
                else if (sender.GetType().Name == "ScrollingText")
                {
                    mouseX = Cursor.Position.X;
                    mouseY = Cursor.Position.Y;

                    picX = ((ScrollingText)sender).Left;
                    picY = ((ScrollingText)sender).Top;

             
                }
                else if(sender.GetType().Name == "PictureBox")
                {
                    mouseX = Cursor.Position.X;
                    mouseY = Cursor.Position.Y;

                    picX = ((PictureBox)sender).Left;
                    picY = ((PictureBox)sender).Top;
               
                }
            }          
        }

        //拖动鼠标移动鼠标的操作
        private void mouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender.GetType().Name == "Label")
                {
                    int y = Cursor.Position.Y - mouseY + picY;
                    int x = Cursor.Position.X - mouseX + picX;

                    ((Label)sender).Top = y;
                    ((Label)sender).Left = x;
                }
                else if (sender.GetType().Name == "ScrollingText")
                {
                    int y = Cursor.Position.Y - mouseY + picY;
                    int x = Cursor.Position.X - mouseX + picX;

                    ((ScrollingText)sender).Top = y;
                    ((ScrollingText)sender).Left = x;
                }
                else  if(sender.GetType().Name == "PictureBox")
                {
                    int y = Cursor.Position.Y - mouseY + picY;
                    int x = Cursor.Position.X - mouseX + picX;

                    ((PictureBox)sender).Top = y;
                    ((PictureBox)sender).Left = x;
                }

            }
        }

        //拖动鼠标放开按钮的操作
        private void mouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender.GetType().Name == "Label")
                {
                    mouseX = 0;
                    mouseY = 0;
                    if (((Label)sender).Location.X < 0)
                    {
                        ((Label)sender).Left = 0;
                    }
                    if (((Label)sender).Location.Y < 0)
                    {
                        ((Label)sender).Top = 0;
                    }
                    if ((((Label)sender).Left + ((Label)sender).Width) > this.pictureBoxBG.Width)
                    {
                        ((Label)sender).Left = this.pictureBoxBG.Width - ((Label)sender).Width;
                    }
                    if ((((Label)sender).Top + ((Label)sender).Height) > this.pictureBoxBG.Height)
                    {
                        ((Label)sender).Top = this.pictureBoxBG.Height - ((Label)sender).Height;
                    }

                    if(((Label)sender).Tag.ToString().Split('#')[5] == "0")
                    {
                        String labelName = ((Label)sender).Name;
                        int count = Info.LabelText.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (Info.LabelText[i].Name.ToString() == labelName)
                            {
                                Info.TextNo = i;
                            }
                        }
                        Info.LabelStyle = "Label";
                        ShowInfo(Info.TextNo);
                    }
                    else if(((Label)sender).Tag.ToString().Split('#')[5] == "3")
                    {
                        String labelName = ((Label)sender).Name;
                        int count = Info.LabelDT.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (Info.LabelDT[i].Name.ToString() == labelName)
                            {
                                Info.TextNo = i;
                            }
                        }
                        Info.LabelStyle = "LabelDT";
                        ShowDTInfo(Info.TextNo);
                    }

                }

                else if (sender.GetType().Name == "ScrollingText")
                {
                    mouseX = 0;
                    mouseY = 0;
                    if (((ScrollingText)sender).Location.X < 0)
                    {
                        ((ScrollingText)sender).Left = 0;
                    }
                    if (((ScrollingText)sender).Location.Y < 0)
                    {
                        ((ScrollingText)sender).Top = 0;
                    }
                    if ((((ScrollingText)sender).Left + ((ScrollingText)sender).Width) > this.pictureBoxBG.Width)
                    {
                        ((ScrollingText)sender).Left = this.pictureBoxBG.Width - ((ScrollingText)sender).Width;
                    }
                    if ((((ScrollingText)sender).Top + ((ScrollingText)sender).Height) > this.pictureBoxBG.Height)
                    {
                        ((ScrollingText)sender).Top = this.pictureBoxBG.Height - ((ScrollingText)sender).Height;
                    }
                    String labelName = ((ScrollingText)sender).Name;
                    int count = Info.RollText.Count;
                    for (int i = 0; i < count; i++)
                    {
                        ScrollingText sc = Info.RollText[i];
                        if (Info.RollText[i].Name.ToString() == labelName)
                        {
                            Info.TextNo = i;
                        }
                        sc.Visible = false;
                        sc.Visible = true;
                    }
                    Info.LabelStyle = "ScrollingText";
                    ShowRInfo(Info.TextNo);
                }
                else if (sender.GetType().Name == "PictureBox")
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
                    String PicName = ((PictureBox)sender).Name;
                    int count = Info.PicBox.Count;
                    for (int i = 0; i < count; i++)
                    {
                        PictureBox pb = Info.PicBox[i];
                        if (Info.PicBox[i].Name.ToString() == PicName)
                        {
                            Info.TextNo = i;
                        }
                    }
                    Info.LabelStyle = "PictureBox";
                    ShowPInfo(Info.TextNo);
                }
            }
        }

        //单击鼠标的操作，类似于画边框之类的
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (sender.GetType().Name == "Label")
                {
                    if(((Label)sender).Tag.ToString().Split('#')[5] == "0")
                    {
                        String labelName = ((Label)sender).Name;
                        int count = Info.LabelText.Count;
                        for (int i = 0; i < count; i++)
                        {
                            draw.Frame(Info.LabelText[i], 0);
                            if (Info.LabelText[i].Name.ToString() == labelName)
                            {
                                Info.TextNo = i;
                                draw.Frame(Info.LabelText[i], 1);
                            }
                        }
                        foreach (ScrollingText sc in Info.RollText)
                        {
                            sc.ShowBorder = false;
                            sc.Visible = false;
                            sc.Visible = true;
                        }
                        foreach (PictureBox pb in Info.PicBox)
                        {
                            draw.PicFrame(pb, 0);
                        }
                        foreach (Label pbdt in Info.LabelDT)
                        {
                            draw.Frame(pbdt, 0);
                        }
                    }
                    else if (((Label)sender).Tag.ToString().Split('#')[5] == "3")
                    {
                        String labelName = ((Label)sender).Name;
                        int count = Info.LabelDT.Count;
                        for (int i = 0; i < count; i++)
                        {
                            draw.Frame(Info.LabelDT[i], 0);
                            if (Info.LabelDT[i].Name.ToString() == labelName)
                            {
                                Info.TextNo = i;
                                draw.Frame(Info.LabelDT[i], 1);
                            }
                        }
                        foreach (ScrollingText sc in Info.RollText)
                        {
                            sc.ShowBorder = false;
                            sc.Visible = false;
                            sc.Visible = true;
                        }
                        foreach (PictureBox pb in Info.PicBox)
                        {
                            draw.PicFrame(pb, 0);
                        }
                        foreach (Label pb in Info.LabelText)
                        {
                            draw.Frame(pb, 0);
                        }
                    }
                }
                else if (sender.GetType().Name == "ScrollingText")
                {
                    String scName = ((ScrollingText)sender).Name;
                    int count = Info.RollText.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Info.RollText[i].ShowBorder = false;
                        if (Info.RollText[i].Name.ToString() == scName)
                        {
                            Info.TextNo = i;
                            Info.RollText[i].BorderColor = Color.Gray;
                            Info.RollText[i].ShowBorder = true;
                        }
                        Info.RollText[i].Visible = false;
                        Info.RollText[i].Visible = true;
                    }
                    foreach (Label pb in Info.LabelText)
                    {
                        draw.Frame(pb, 0);
                    }
                    foreach (PictureBox pb in Info.PicBox)
                    {
                        draw.PicFrame(pb, 0);
                    }
                    foreach (Label pbdt in Info.LabelDT)
                    {
                        draw.Frame(pbdt, 0);
                    }
                }
                else if (sender.GetType().Name == "PictureBox")
                {

                    String pbName = ((PictureBox)sender).Name;
                    int count = Info.PicBox.Count;
                    for (int i = 0; i < count; i++)
                    {
                        draw.PicFrame(Info.PicBox[i], 0);
                        if (Info.PicBox[i].Name.ToString() == pbName)
                        {
                            Info.TextNo = i;
                            draw.PicFrame(Info.PicBox[i], 1);
                        }
                    }
                    foreach (ScrollingText sc in Info.RollText)
                    {
                        sc.ShowBorder = false;
                        sc.Visible = false;
                        sc.Visible = true;
                    }
                    foreach (Label pb in Info.LabelText)
                    {
                        draw.Frame(pb, 0);
                    }
                    foreach (Label pbdt in Info.LabelDT)
                    {
                        draw.Frame(pbdt, 0);
                    }
                }
            }
        }

        private void ListViewInitialize(int slitem)
        {
            listViewProject.View = View.Details;
            listViewProject.MultiSelect = false;
            listViewProject.HideSelection = false;
            listViewProject.Columns.Add("创建日期");
            listViewProject.Columns.Add("版式名称");
            listViewProject.Columns.Add("LED宽");
            listViewProject.Columns.Add("LED高");
            listViewProject.Columns.Add("类型");

            listViewProject.FullRowSelect = true;
            listViewProject.HideSelection = false;


            try
            {
                if (ConnDB.Conn.State != ConnectionState.Open)
                {
                    ConnDB.Conn.Open();
                }
                operateDB.ShowProject(ConnDB.Conn, listViewProject);

                if(listViewProject.Items.Count > 0)
                {
                    listViewProject.Items[slitem].Selected = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库丢失！");
            }
            
        }


        delegate void DelegateDT(Label Lb,String DTstr);

        public void DT(Label Lb, String DTstr)
        {
            Lb.Text = DTstr;
        }

        private void blink()
        {
            while (true)
            {
                try
                {
                    GC.Collect();
                    Info.ColorFont.Clear();
                    Info.ColorBack.Clear();
                    Info.ColorAllF.Clear();
                    Info.ColorAllB.Clear();
                    //Info.BtImage.Clear();
                    if (Info.LabelDT.Count > 0)
                    {
                        foreach(Label pbdt in Info.LabelDT)
                        {
                            String dtStr = DateTime.Now.ToString(pbdt.Tag.ToString().Split('#')[6]);
                            DelegateDT delegateDT = new DelegateDT(DT);
                            pbdt.BeginInvoke(delegateDT, new object[] { pbdt, dtStr });
                        }
                    }
                    Thread.Sleep(1000);

                    if (Info.BlinkFont.Count > 0)
                    {
                        foreach (Label pb in Info.BlinkFont)
                        {
                            Info.ColorFont.Add(pb.ForeColor);
                            pb.ForeColor = Color.Black;
                        }
                    }

                    if (Info.BlinkBack.Count > 0)
                    {
                        foreach (Label pb in Info.BlinkBack)
                        {
                            Info.ColorBack.Add(pb.BackColor);
                            pb.BackColor = Color.Black;
                        }
                    }

                    if (Info.BlinkAll.Count > 0)
                    {
                        foreach (Label pb in Info.BlinkAll)
                        {
                            Info.ColorAllF.Add(pb.ForeColor);
                            Info.ColorAllB.Add(pb.BackColor);
                            pb.ForeColor = Color.Black;
                            pb.BackColor = Color.Black;
                        }
                    }
                    if (Info.BlinkPicName.Count > 0)
                    {
                        foreach (String PicName in Info.BlinkPicName)
                        {
                            int PicCount = Info.PicBox.Count;
                            for(int i = 0; i < PicCount; i++)
                            {
                                if(PicName == Info.PicBox[i].Name)
                                {
                                    if(PicName == labelNo.Text)
                                    {
                                        Info.PicBox[i].Image = Info.BtBlackF[i];
                                    }
                                    else
                                    {
                                        Info.PicBox[i].Image = Info.BtBlack[i];
                                    }                                    
                                }
                            }
                        }
                    }

                    if (Info.LabelDT.Count > 0)
                    {
                        foreach (Label pbdt in Info.LabelDT)
                        {
                            String dtStr = DateTime.Now.ToString(pbdt.Tag.ToString().Split('#')[6]);
                            DelegateDT delegateDT = new DelegateDT(DT);
                            pbdt.BeginInvoke(delegateDT, new object[] { pbdt, dtStr });
                        }
                    }

                    Thread.Sleep(1000);

                    int countF = Info.ColorFont.Count;
                    int countB = Info.ColorBack.Count;
                    int countA = Info.ColorAllF.Count;
                    //int countI = Info.BtImage.Count;

                    if (Info.BlinkFont.Count == Info.ColorFont.Count)
                    {
                        for (int i = 0; i < countF; i++)
                        {
                            Info.BlinkFont[i].ForeColor = Info.ColorFont[i];
                        }
                    }

                    if (Info.BlinkBack.Count == Info.ColorBack.Count)
                    {
                        for (int i = 0; i < countB; i++)
                        {
                            Info.BlinkBack[i].BackColor = Info.ColorBack[i];
                        }
                    }

                    if ((Info.BlinkAll.Count == Info.ColorAllF.Count) && (Info.BlinkAll.Count == Info.ColorAllB.Count))
                    {
                        for (int i = 0; i < countA; i++)
                        {
                            Info.BlinkAll[i].ForeColor = Info.ColorAllF[i];
                            Info.BlinkAll[i].BackColor = Info.ColorAllB[i];
                        }
                    }

                    if (Info.BlinkPicName.Count > 0) 
                    {
                        foreach (String PicName in Info.BlinkPicName)
                        {
                            int PicCount = Info.PicBox.Count;
                            for (int i = 0; i < PicCount; i++)
                            {
                                if (PicName == Info.PicBox[i].Name)
                                {
                                    if (PicName == labelNo.Text)
                                    {
                                        Info.PicBox[i].Image = Info.BtBitmapF[i];
                                    }
                                    else
                                    {
                                        Info.PicBox[i].Image = Info.BtBitmap[i];
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }                
        }


        //清除预览界面
        private void ClearLabel()
        {
            foreach (Label pb in Info.LabelText)
            {
                pb.Dispose();
            }
            foreach (Label pbdt in Info.LabelDT)
            {
                pbdt.Dispose();
            }
            foreach (ScrollingText st in Info.RollText)
            {
                st.Dispose();
            }
            foreach(PictureBox pb in Info.PicBox)
            {
                pb.Dispose();
            }

            Info.LabelText.Clear();
            Info.RollText.Clear();
            Info.PicBox.Clear();
            Info.LabelDT.Clear();

            Info.BlinkFont.Clear();
            Info.BlinkBack.Clear();
            Info.BlinkAll.Clear();
            Info.BlinkPicName.Clear();
            Info.BtBitmap.Clear();
            Info.BtBitmapF.Clear();
            Info.BtBlack.Clear();
            Info.BtBlackF.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Bool">控制控件是否可用</param>
        private void controlEnabled(bool Bool)
        {
            if (Bool == false)
            {
                textBoxName.Enabled = false;
                buttonSave.Enabled = false;
                buttonText.Enabled = false;
                buttonScroll.Enabled = false;
                buttonPic.Enabled = false;
                groupBoxLED.Enabled = false;
                groupBoxText.Enabled = false;
            }
            else
            {
                textBoxName.Enabled = true;
                buttonSave.Enabled = true;
                buttonText.Enabled = true;
                buttonScroll.Enabled = true;
                buttonPic.Enabled = true;
                groupBoxLED.Enabled = true;
                groupBoxText.Enabled = true;
            }
        }

        /// <summary>
        /// 读取数据,以utctime为标识
        /// </summary>
        /// <param name="Date"></param>
        private void LoadProject(int Date)
        {
            ClearLabel();
            if (ConnDB.Conn.State != ConnectionState.Open)
            {
                ConnDB.Conn.Open();
            }
            try
            {
                operateDB.LEDSize(ConnDB.Conn, Date);
                if(Info.LEDStyle == "T")
                {

                    comboBoxColor.DataSource = Info.TwoColor.Clone();
                    comboBoxBColor.DataSource = Info.TwoColor.Clone();
                    comboBoxFColor.DataSource = Info.TwoColor.Clone();
                }
                else if(Info.LEDStyle == "F")
                {

                    comboBoxColor.DataSource = Info.FullColor.Clone();
                    comboBoxBColor.DataSource = Info.FullColor.Clone();
                    comboBoxFColor.DataSource = Info.FullColor.Clone();
                }
                textBoxName.Text = Info.Name;
                textBoxWidth.Text = Info.LEDWidth.ToString();
                textBoxHeight.Text = Info.LEDHeight.ToString();
                pictureBoxBG.Width = Info.LEDWidth;
                pictureBoxBG.Height = Info.LEDHeight;
                AddLabel(ConnDB.Conn, pictureBoxBG, Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库丢失！");
            }
            textNo = 0;
            foreach (Label pb in Info.LabelText)
            {
                if (textNo <= int.Parse(pb.Name))
                {
                    textNo = int.Parse(pb.Name) + 1;
                }
            }
            foreach (ScrollingText sc in Info.RollText)
            {
                if (textNo <= int.Parse(sc.Name))
                {
                    textNo = int.Parse(sc.Name) + 1;
                }
            }
            foreach (PictureBox pb in Info.PicBox)
            {
                if (textNo <= int.Parse(pb.Name))
                {
                    textNo = int.Parse(pb.Name) + 1;
                }
            }
        }

        /// <summary>
        /// 读取界面
        /// </summary>
        /// <param name="Conn"></param>
        /// <param name="pb"></param>
        /// <param name="Date"></param>
        private void AddLabel(SQLiteConnection Conn, PictureBox pb, long Date)
        {
            String sql = String.Format(@"SELECT TextNo, Content, Width, Height, Left, Top, FontSize, FontColor, BackColor, BorderL, BorderT, BorderR, BorderB, BorderColor, Type, Blink FROM Info{0}", Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            SQLiteDataReader dr = sqltcmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr.GetString(14) == "1")
                {
                    ScrollingText scrollText = new ScrollingText();
                    scrollText.Name = dr.GetString(0);
                    scrollText.ScrollText = dr.GetString(1);
                    scrollText.Width = dr.GetInt32(2);
                    scrollText.Height = dr.GetInt32(3);
                    scrollText.Left = dr.GetInt32(4);
                    scrollText.Top = dr.GetInt32(5);
                    scrollText.Font = new Font("宋体", dr.GetInt32(6));
                    scrollText.ForeColor = HEXtoC(dr.GetString(7));
                    scrollText.BackColor = HEXtoC(dr.GetString(8));
                    scrollText.Tag = dr.GetString(9) + "#" + dr.GetString(10) + "#" + dr.GetString(11) + "#" + dr.GetString(12) + "#" + dr.GetString(13) + "#" + dr.GetString(14) + "#" + dr.GetString(15) + "#" + dr.GetString(7) + "#" + dr.GetString(8);

                    String[] borderInfo = scrollText.Tag.ToString().Split('#');

                    pb.Controls.Add(scrollText);
                    scrollText.BringToFront();
                    scrollText.MouseDown += new MouseEventHandler(this.mouseDown);
                    scrollText.MouseMove += new MouseEventHandler(this.mouseMove);
                    scrollText.MouseUp += new MouseEventHandler(this.mouseUp);
                    scrollText.MouseClick += new MouseEventHandler(this.mouseClick);

                    Info.RollText.Add(scrollText);
                }
                else if (dr.GetString(14) == "0")
                {
                    Label Lb = new Label();
                    Lb.Name = dr.GetString(0);
                    Lb.Text = dr.GetString(1);
                    Lb.Width = dr.GetInt32(2);
                    Lb.Height = dr.GetInt32(3);
                    Lb.Left = dr.GetInt32(4);
                    Lb.Top = dr.GetInt32(5);
                    Lb.Font = new Font("宋体", dr.GetInt32(6));
                    Lb.TextAlign = ContentAlignment.MiddleCenter;
                    Lb.ForeColor = HEXtoC(dr.GetString(7));
                    Lb.BackColor = HEXtoC(dr.GetString(8));
                    Lb.Tag = dr.GetString(9) + "#" + dr.GetString(10) + "#" + dr.GetString(11) + "#" + dr.GetString(12) + "#" + dr.GetString(13) + "#" + dr.GetString(14) + "#" + dr.GetString(15) + "#" + dr.GetString(7) + "#" + dr.GetString(8);

                    String[] borderInfo = Lb.Tag.ToString().Split('#');
                    draw.DrawBorder(Lb, borderInfo);

                    switch (borderInfo[6])
                    {
                        case "0":
                            break;
                        case "1":
                            Info.BlinkFont.Add(Lb);
                            break;
                        case "2":
                            Info.BlinkBack.Add(Lb);
                            break;
                        case "3":
                            Info.BlinkAll.Add(Lb);
                            break;
                        default:
                            break;
                    }

                    pb.Controls.Add(Lb);
                    Lb.BringToFront();
                    Lb.MouseDown += new MouseEventHandler(this.mouseDown);
                    Lb.MouseMove += new MouseEventHandler(this.mouseMove);
                    Lb.MouseUp += new MouseEventHandler(this.mouseUp);
                    Lb.MouseClick += new MouseEventHandler(this.mouseClick);

                    Info.LabelText.Add(Lb);
                }
                else if (dr.GetString(14) == "2")
                {
                    PictureBox Pic = new PictureBox();
                    Pic.Name = dr.GetString(0);
                    Pic.Text = dr.GetString(1);
                    Pic.Width = dr.GetInt32(2);
                    Pic.Height = dr.GetInt32(3);
                    Pic.Left = dr.GetInt32(4);
                    Pic.Top = dr.GetInt32(5);
                    Pic.Image = new Bitmap (Application.StartupPath + Pic.Text);
                    Pic.SizeMode = PictureBoxSizeMode.CenterImage;
                    Pic.ForeColor = StoC(dr.GetString(7));
                    Pic.BackColor = Color.Transparent;
                    Pic.Parent = pictureBoxBG;
                    Pic.Tag = dr.GetString(9) + "#" + dr.GetString(10) + "#" + dr.GetString(11) + "#" + dr.GetString(12) + "#" + dr.GetString(13) + "#" + dr.GetString(14) + "#" + dr.GetString(15) + "#" + dr.GetString(7) + "#" + dr.GetString(8);

                    String[] borderInfo = Pic.Tag.ToString().Split('#');
                    draw.DrawPicBorder(Pic, borderInfo);

                    switch (borderInfo[6])
                    {
                        case "0":
                            break;
                        case "1":
                            Info.BlinkPicName.Add(Pic.Name);
                            break;
                        default:
                            break;
                    }

                    pb.Controls.Add(Pic);
                    Pic.BringToFront();
                    Pic.MouseDown += new MouseEventHandler(this.mouseDown);
                    Pic.MouseMove += new MouseEventHandler(this.mouseMove);
                    Pic.MouseUp += new MouseEventHandler(this.mouseUp);
                    Pic.MouseClick += new MouseEventHandler(this.mouseClick);

                    Info.PicBox.Add(Pic);
                    Info.BtBlack.Add(draw.BlackBorder(Pic));
                    Info.BtBitmap.Add(draw.ImageBorder(Pic));
                    Info.BtBlackF.Add(draw.BlackBorderF(Pic));
                    Info.BtBitmapF.Add(draw.ImageBorderF(Pic));
                }
                else if (dr.GetString(14) == "3")
                {
                    Label pbdt = new Label();
                    pbdt.Name = dr.GetString(0);

                    pbdt.Width = dr.GetInt32(2);
                    pbdt.Height = dr.GetInt32(3);
                    pbdt.Left = dr.GetInt32(4);
                    pbdt.Top = dr.GetInt32(5);
                    pbdt.Font = new Font("宋体", dr.GetInt32(6));
                    pbdt.TextAlign = ContentAlignment.MiddleCenter;
                    pbdt.ForeColor = HEXtoC(dr.GetString(7));
                    pbdt.BackColor = HEXtoC(dr.GetString(8));
                    pbdt.Tag = dr.GetString(9) + "#" + dr.GetString(10) + "#" + dr.GetString(11) + "#" + dr.GetString(12) + "#" + dr.GetString(13) + "#" + dr.GetString(14) + "#" + dr.GetString(15) + "#" + dr.GetString(7) + "#" + dr.GetString(8);

                    String[] borderInfo = pbdt.Tag.ToString().Split('#');
                    draw.DrawBorder(pbdt, borderInfo);
                    pbdt.Text = DateTime.Now.ToString(borderInfo[6]);

                    pb.Controls.Add(pbdt);
                    pbdt.BringToFront();
                    pbdt.MouseDown += new MouseEventHandler(this.mouseDown);
                    pbdt.MouseMove += new MouseEventHandler(this.mouseMove);
                    pbdt.MouseUp += new MouseEventHandler(this.mouseUp);
                    pbdt.MouseClick += new MouseEventHandler(this.mouseClick);

                    Info.LabelDT.Add(pbdt);
                }

            }
        }

        private void ShowInfo(int TextNo)
        {
            labelText.Text = "文字内容";
            Label Lb = Info.LabelText[Info.TextNo];
            textBoxContent.Visible = true;
            textBoxContent.Enabled = true;
            comboBoxColor.Enabled = true;
            comboBoxBColor.Enabled = true;
            comboBoxFColor.Enabled = true;
            comboBoxRB.Enabled = true;
            numericUpDownFont.Enabled = true;
            checkedListBoxBorder.Enabled = true;
            comboBoxDT.Visible = false;

            comboBoxRB.DataSource = Info.BlinkList.Clone();

           
            labelNo.Text = Lb.Name;
            textBoxContent.Text = Lb.Text;
            textBoxX.Text = Lb.Left.ToString();
            textBoxY.Text = Lb.Top.ToString();
            numericUpDownFont.Text = (Lb.Font.Size).ToString();
            textBoxLWidth.Text = Lb.Width.ToString();
            textBoxLHeight.Text = Lb.Height.ToString();

            String[] borderInfo = Lb.Tag.ToString().Split('#');
            

            for (int i = 0; i < 4; i++)
            {
                if (borderInfo[i] == "1")
                {
                    checkedListBoxBorder.SetItemChecked(i, true);
                }
                else
                {
                    checkedListBoxBorder.SetItemChecked(i, false);
                }
            }

            switch (borderInfo[6])
            {
                case "0":
                    comboBoxRB.SelectedIndex = 0;
                    break;
                case "1":
                    comboBoxRB.SelectedIndex = 1;
                    break;
                case "2":
                    comboBoxRB.SelectedIndex = 2;
                    break;
                case "3":
                    comboBoxRB.SelectedIndex = 3;
                    break;
                default:
                    break;
            }

            comboBoxFColor.Text = HEXtoS(borderInfo[4]);
            comboBoxColor.Text = HEXtoS(borderInfo[7]);
            comboBoxBColor.Text = HEXtoS(borderInfo[8]);
        }

        private void ShowDTInfo(int TextNo)
        {
            labelText.Text = "时间格式";
            textBoxContent.Visible = false;
            textBoxContent.Enabled = true;
            comboBoxDT.Visible = true;
            comboBoxColor.Enabled = true;
            comboBoxBColor.Enabled = true;
            comboBoxFColor.Enabled = true;
            comboBoxRB.Enabled = false;
            numericUpDownFont.Enabled = true;
            checkedListBoxBorder.Enabled = true;


            Label pbdt = Info.LabelDT[Info.TextNo];
            comboBoxDT.Text = DTtoS(pbdt.Tag.ToString().Split('#')[6]);
            labelNo.Text = pbdt.Name;
            textBoxContent.Text = pbdt.Text;
            textBoxX.Text = pbdt.Left.ToString();
            textBoxY.Text = pbdt.Top.ToString();
            numericUpDownFont.Text = (pbdt.Font.Size).ToString();
            textBoxLWidth.Text = pbdt.Width.ToString();
            textBoxLHeight.Text = pbdt.Height.ToString();
            String[] borderInfo = pbdt.Tag.ToString().Split('#');


            for (int i = 0; i < 4; i++)
            {
                if (borderInfo[i] == "1")
                {
                    checkedListBoxBorder.SetItemChecked(i, true);
                }
                else
                {
                    checkedListBoxBorder.SetItemChecked(i, false);
                }
            }

            switch (borderInfo[6])
            {
                case "0":
                    comboBoxRB.SelectedIndex = 0;
                    break;
                case "1":
                    comboBoxRB.SelectedIndex = 1;
                    break;
                case "2":
                    comboBoxRB.SelectedIndex = 2;
                    break;
                case "3":
                    comboBoxRB.SelectedIndex = 3;
                    break;
                default:
                    break;
            }

            comboBoxFColor.Text = HEXtoS(borderInfo[4]);
            comboBoxColor.Text = HEXtoS(borderInfo[7]);
            comboBoxBColor.Text = HEXtoS(borderInfo[8]);
        }


        private void ShowRInfo(int TextNo)
        {
            labelText.Text = "文字内容";
            textBoxContent.Visible = true;
            textBoxContent.Enabled = true;
            numericUpDownFont.Enabled = true;
            comboBoxBColor.Enabled = true;
            comboBoxColor.Enabled = true;

            comboBoxDT.Visible = false;
            comboBoxFColor.Enabled = false;
            comboBoxRB.Enabled = false;
            checkedListBoxBorder.Enabled = false;

            ScrollingText RT = Info.RollText[Info.TextNo];
            labelNo.Text = RT.Name;
            textBoxContent.Text = RT.ScrollText;
            textBoxX.Text = RT.Left.ToString();
            textBoxY.Text = RT.Top.ToString();
            numericUpDownFont.Text = (RT.Font.Size).ToString();
            textBoxLWidth.Text = RT.Width.ToString();
            textBoxLHeight.Text = RT.Height.ToString();

            String[] borderInfo = RT.Tag.ToString().Split('#');


            comboBoxColor.Text = HEXtoS(borderInfo[7]);
            comboBoxBColor.Text = HEXtoS(borderInfo[8]);
        }      

        private void ShowPInfo(int TextNo)
        {
            labelText.Text = "内容";
            comboBoxFColor.Enabled = true;
            comboBoxRB.Enabled = true;
            checkedListBoxBorder.Enabled = true;
            textBoxContent.Visible = true;

            comboBoxDT.Visible = false;

            comboBoxColor.Enabled = false;
            comboBoxBColor.Enabled = false;
            textBoxContent.Enabled = false;
            numericUpDownFont.Enabled = false;

            comboBoxRB.DataSource = Info.BlinkPicList.Clone();

            PictureBox pbdt = Info.PicBox[Info.TextNo];
            labelNo.Text = pbdt.Name;
            textBoxContent.Text = "图片";
            textBoxX.Text = pbdt.Left.ToString();
            textBoxY.Text = pbdt.Top.ToString();
            textBoxLWidth.Text = pbdt.Width.ToString();
            textBoxLHeight.Text = pbdt.Height.ToString();

            String[] borderInfo = pbdt.Tag.ToString().Split('#');
            comboBoxFColor.Text = HEXtoS(borderInfo[4]);

            for (int i = 0; i < 4; i++)
            {
                if (borderInfo[i] == "1")
                {
                    checkedListBoxBorder.SetItemChecked(i, true);
                }
                else
                {
                    checkedListBoxBorder.SetItemChecked(i, false);
                }
            }

            switch (borderInfo[6])
            {
                case "0":
                    comboBoxRB.SelectedIndex = 0;
                    break;
                case "1":
                    comboBoxRB.SelectedIndex = 1;
                    break;
            }            
        }

        public static string CtoS(Color color)
        {
            String colorName = "R";
            switch (color.Name)
            {
                case "Red":
                    colorName = "红色";
                    break;
                case "Green":
                    colorName = "绿色";
                    break;
                case "Yellow":
                    colorName = "黄色";
                    break;
                case "Black":
                    colorName = "黑色";
                    break;
            }
            return colorName;
        }

        public static Color StoC(String colorName)
        {
            Color color = Color.Red;
            switch (colorName)
            {
                case "红色":
                    color = HEXtoC("FF0000");
                    break;
                case "绿色":
                    color = HEXtoC("008000");
                    break;
                case "黄色":
                    color = HEXtoC("FFFF00");
                    break;
                case "蓝色":
                    color = HEXtoC("0000FF");
                    break;
                case "黑色":
                    color = HEXtoC("000000");
                    break;
                case "白色":
                    color = HEXtoC("FFFFFF");
                    break;
                case "紫色":
                    color = HEXtoC("FF00FF");
                    break;
                case "青色":
                    color = HEXtoC("00FFFF");
                    break;
            }
            return color;
        }

        public static String CtoHEX(Color color)
        {
            String R = Convert.ToString(color.R, 16).ToUpper();
            if (R == "0") 
            R = "00";
            String G = Convert.ToString(color.G, 16).ToUpper();
            if (G == "0") 
            G = "00";
            String B = Convert.ToString(color.B, 16).ToUpper();
            if (B == "0") 
            B = "00";

            return R + G + B;
        }

        public static Color HEXtoC(String ColorHEX)
        {
            int r = Convert.ToInt32("0x" + ColorHEX.Substring(0, 2), 16);
            int g = Convert.ToInt32("0x" + ColorHEX.Substring(2, 2), 16);
            int b = Convert.ToInt32("0x" + ColorHEX.Substring(4, 2), 16);

            return Color.FromArgb(r, g, b);
        }

        public static String HexToRGB(String ColorHEX)
        {
            int r = Convert.ToInt32("0x" + ColorHEX.Substring(0, 2), 16);
            int g = Convert.ToInt32("0x" + ColorHEX.Substring(2, 2), 16);
            int b = Convert.ToInt32("0x" + ColorHEX.Substring(4, 2), 16);
            return r.ToString() + "," + g.ToString() + "," + b.ToString();
        }
        

        public static String HEXtoS(String ColorHEX)
        {
            String colorName = "红色";
            switch (ColorHEX)
            {
                case "FF0000":
                    colorName = "红色";
                    break;
                case "008000":
                    colorName = "绿色";
                    break;
                case "FFFF00":
                    colorName = "黄色";
                    break;
                case "000000":
                    colorName = "黑色";
                    break;
                case "00FFFF":
                    colorName = "青色";
                    break;
                case "0000FF":
                    colorName = "蓝色";
                    break;
                case "FF00FF":
                    colorName = "紫色";
                    break;
                case "FFFFFF":
                    colorName = "白色";
                    break;
            }
            return colorName;
        }

        private void EditLED_FormClosing(object sender, FormClosingEventArgs e)
        {            
            try
            {
                if (textBoxName.Text != "")
                {
                    Save();
                    selectedItem = listViewProject.SelectedItems[0].Index;
                    listViewChange(listViewProject, selectedItem);
                }
                else
                {
                    MessageBox.Show("样式名称不能为空");
                }

                thBling.Abort();
                //if (ConnDB.Conn.State != ConnectionState.Open)
                //{
                //    ConnDB.Conn.Open();
                //}
                //List<String> ExistDire = operateDB.Dire(ConnDB.Conn);
                //DirectoryInfo folder = new DirectoryInfo(Application.StartupPath + "\\Pic" );
                //DirectoryInfo[] direInfo = folder.GetDirectories();
                //List<DirectoryInfo> AllDire = direInfo.ToList();
                //int i = 0;

                //while (i < AllDire.Count)
                //{
                //    int j = 0;
                //    foreach (String a in ExistDire)
                //    {
                //        if(AllDire[i].Name == a)
                //        {
                //            AllDire.RemoveAt(i);
                //            j = 1;
                //        }
                //    }
                //    if (j == 0)
                //    {
                //        i++;
                //    }
                //}

                //foreach(DirectoryInfo a in AllDire)
                //{
                //    a.Delete(true);
                //}
                


                if (ConnDB.Conn.State == ConnectionState.Open)
                    {
                        ConnDB.Conn.Close();
                    }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Application.ExitThread();
            }


        }

        private void buttonColor(Color color, Button button)
        {
            Bitmap bt = new Bitmap(70, 16);
            Graphics g = Graphics.FromImage(bt);
            Brush brush = new SolidBrush(color);
            g.FillRectangle(brush, 0, 0, 69, 16);
            button.Image = bt;
        }

        private void CreateIni(long UtcTime)
        {
            string dicPath = Application.StartupPath + "\\IniFile";
            string filePath = dicPath + "\\TIPS.ini";

            if (!Directory.Exists(dicPath))
            {
                Directory.CreateDirectory(dicPath);
            }
            if (!File.Exists(filePath))
            {
                DateTime dt = DateTime.Now;
                File.Create(filePath);
                Console.Write(DateTime.Now - dt);
            }
            else
            {
                StreamWriter sw = new StreamWriter(filePath);
                sw.Flush();
                sw.Close();
            }

            int areaCount = Info.LabelText.Count + Info.RollText.Count + Info.LabelDT.Count + Info.PicBox.Count;
            int staticCount = Info.LabelText.Count + Info.LabelDT.Count;
            int timeArea = Info.LabelDT.Count;


            while (!File.Exists(filePath))
            {
                Thread.Sleep(10);
                Console.WriteLine(1);
            }
            IniFile.WriteIniData("COMMAND", "TYPE", "1", filePath);
            IniFile.WriteIniData("COMMAND", "BACKGROUND", "", filePath);
            IniFile.WriteIniData("COMMAND", "AREANUM", areaCount.ToString(), filePath);
            IniFile.WriteIniData("COMMAND", "LONGTIME", "0", filePath);
            IniFile.WriteIniData("COMMAND", "STATICCONTROL", staticCount.ToString(), filePath);
            IniFile.WriteIniData("COMMAND", "CLEANTIME", "50", filePath);
            IniFile.WriteIniData("COMMAND", "DEAMONTIME", "10", filePath);
            IniFile.WriteIniData("COMMAND", "TIMEAREA", timeArea.ToString(), filePath);
            IniFile.WriteIniData("COMMAND", "SHOWSCREEN", "0", filePath);

            int AreaNum = 1;
            foreach(Label lb in Info.LabelText)
            {
                string[] lbInfo = lb.Tag.ToString().Split('#');
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TYPE", "2", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "RECT", lb.Left.ToString()+ "," + lb.Top.ToString() + "," + (lb.Left + lb.Width).ToString() + "," + (lb.Top + lb.Height).ToString(), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "ShowID", "0", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "COLOR", HexToRGB(lbInfo[7]), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BACKCOLOR", HexToRGB(lbInfo[8]), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TEXT", lb.Text, filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "FONTSIZE", lb.Font.Size.ToString(), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BORDER", lbInfo[0]+lbInfo[1] + lbInfo[2]+ lbInfo[3], filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BORDERCOLOR", HexToRGB(lbInfo[4]), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BLINK", lbInfo[6], filePath);
                AreaNum++;
            }

            foreach(ScrollingText st in Info.RollText)
            {
                string[] stInfo = st.Tag.ToString().Split('#');
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TYPE", "3", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "RECT", st.Left.ToString() + "," + st.Top.ToString() + "," + (st.Left + st.Width).ToString() + "," +(st.Top + st.Height), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "ShowID", "0", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "COLOR", HexToRGB(stInfo[7]), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BACKCOLOR", HexToRGB(stInfo[8]), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TEXT", st.ScrollText, filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "SCROLLTYPE", "1", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "SPEED", "5", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "FONTSIZE", st.Font.Size.ToString(), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BORDER", stInfo[0] + stInfo[1] + stInfo[2] + stInfo[3], filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BORDERCOLOR", HexToRGB(stInfo[4]), filePath);
                AreaNum++;
            }

            foreach (Label pb in Info.LabelDT)
            {
                string[] pbInfo = pb.Tag.ToString().Split('#');
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TYPE", "4", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "RECT", pb.Left.ToString() + "," + pb.Top.ToString() + "," + (pb.Left + pb.Width).ToString() + "," +(pb.Top + pb.Height), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "ShowID", "10", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "COLOR", HexToRGB(pbInfo[7]), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BACKCOLOR", HexToRGB(pbInfo[8]), filePath);
                //IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TEXT", pb.Text, filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "FONTSIZE", pb.Font.Size.ToString(), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BORDER", pbInfo[0] + pbInfo[1] + pbInfo[2] + pbInfo[3], filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BORDERCOLOR", HexToRGB(pbInfo[4]), filePath);

                AreaNum++;
            }


            foreach (PictureBox pb in Info.PicBox)
            {
                string[] pbInfo = pb.Tag.ToString().Split('#');
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "TYPE", "1", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "RECT", pb.Left.ToString() + "," + pb.Top.ToString() + "," + (pb.Left + pb.Width).ToString() + "," + (pb.Top + pb.Height), filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "ShowID", "0", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "PICNAME", pb.Text.Split('\\')[3], filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "SHOWTIME", "5", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1","DRAWTYPE", "2", filePath);
                IniFile.WriteIniData("AREA_" + AreaNum.ToString() + "_1", "BLINK", pbInfo[6], filePath);
                AreaNum++;
            }

        }

        private String DTtoS(String dt)
        {
            string dtStr = "";
            switch (dt)
            {
                case "年-月-日 时:分:秒":
                    dtStr = "yyyy-MM-dd HH:mm:ss";
                    break;
                case "年-月-日 时:分":
                    dtStr = "yyyy-MM-dd HH:mm";
                    break;
                case "月-日 时:分:秒":
                    dtStr = "MM-dd HH:mm:ss";
                    break;
                case "月-日 时:分":
                    dtStr = "MM-dd HH:mm";
                    break;
                case "年-月-日":
                    dtStr = "yyyy-MM-dd";
                    break;
                case "月-日":
                    dtStr = "MM-dd";
                    break;
                case "时:分:秒":
                    dtStr = "HH:mm:ss";
                    break;
                case "时:分":
                    dtStr = "HH:mm";
                    break;
                case "yyyy-MM-dd HH:mm:ss":
                    dtStr = "年-月-日 时:分:秒";
                    break;
                case "yyyy-MM-dd HH:mm":
                    dtStr = "年-月-日 时:分";
                    break;
                case "MM-dd HH:mm:ss":
                    dtStr = "月-日 时:分:秒";
                    break;
                case "MM-dd HH:mm":
                    dtStr = "月-日 时:分";
                    break;
                case "yyyy-MM-dd":
                    dtStr = "年-月-日";
                    break;
                case "MM-dd":
                    dtStr = "月-日";
                    break;
                case "HH:mm:ss":
                    dtStr = "时:分:秒";
                    break;
                case "HH:mm":
                    dtStr = "时:分";
                    break;
            }
            return dtStr;
        }

        private void buttonConn_Click(object sender, EventArgs e)
        {
            try
            {
                if (tcpDll.isConnected)
                {
                    tcpDll.disConnect();
                }
                tcpDll.isConnected = tcpDll.connect(textBox1.Text, 9999);
                MessageBox.Show("链接成功！");
            }
            catch
            {
                MessageBox.Show("链接失败");
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {

            try
            {
                foreach (PictureBox pb in Info.PicBox)
                {
                    tcpDll.sendFile(Application.StartupPath + pb.Text);
                    Thread.Sleep(100);
                }
                CreateIni(Info.UTCTime);
                Thread.Sleep(50);
                tcpDll.sendFile(Application.StartupPath + "\\IniFile\\TIPS.ini");
                MessageBox.Show("发送成功！");
            }
            catch
            {
                MessageBox.Show("发送失败！");
            }

        }
    }
}
