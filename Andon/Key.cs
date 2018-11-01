using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Andon
{
    class Key
    {
        /// <summary>
        /// 限制输入，应该是只允许数字输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IntNumber(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x08)
            {
                return;
            }
            else if (e.KeyChar <= 0x39 && e.KeyChar >= 0x30)
            {
                try
                {
                    Int32.Parse(((TextBox)sender).Text + e.KeyChar.ToString());
                }
                catch
                {
                    e.KeyChar = (char)0;   //处理非法字符
                }
            }
            else
            {
                e.KeyChar = (char)0;   //禁止空格键
            }
        }

    }
}
