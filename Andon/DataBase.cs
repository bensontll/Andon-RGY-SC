using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using ScrollingTextControl;
using System.Data;

namespace Andon
{
    public class ConnDB
    {
        static String strConn = "Data Source =" + Application.StartupPath + "\\Andon.db; Pooling = true; FailIfMissing = false";
        public static SQLiteConnection Conn = new SQLiteConnection(strConn);
    }


    public class OperatDB
    {      

        public void InsertProject( SQLiteConnection Conn, String Name , int Date , int Width,int Height,String Style)
        {
            String sqlInsert = String.Format(@"INSERT INTO Project (Name, Date, Width, Height, Style) VALUES ('{0}', {1}, {2}, {3}, '{4}')", Name, Date, Width, Height, Style);
            SQLiteCommand sqltcmd = new SQLiteCommand(sqlInsert, Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void CreateInfo(SQLiteConnection Conn, int Date)
        {
            String sqlCreate = String.Format(@"CREATE TABLE Info{0} ( ID  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,TextNo  TEXT, Content  TEXT, Width  INTEGER,Height  INTEGER, Left  INTEGER, Top  INTEGER, FontSize  INTEGER, FontColor  TEXT, BackColor  TEXT, BorderL TEXT,BorderT TEXT, BorderR  TEXT, BorderB  TEXT, BorderColor  TEXT, Type  TEXT, Blink  TEXT ); ", Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sqlCreate, Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void ClearInfo(SQLiteConnection Conn,int Date)
        {
            String sql = String.Format("DELETE FROM Info{0}", Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void InserInfo(SQLiteConnection Conn ,int Date, List<Label> ListLabel)
        {
            String sql = String.Format("INSERT INTO Info{0} (TextNo, Content, Width, Height, Left, Top, FontSize, FontColor, BackColor, BorderL, BorderT, BorderR, BorderB, BorderColor, Type, Blink) VALUES ",Date);
            StringBuilder sbSql = new StringBuilder(sql);
            foreach (Label lb in ListLabel)
            {
                String[] info = lb.Tag.ToString().Split('#');
                sbSql.Append("('");
                sbSql.Append(lb.Name);
                sbSql.Append("','");
                sbSql.Append(lb.Text);
                sbSql.Append("',");
                sbSql.Append(lb.Width);
                sbSql.Append(",");
                sbSql.Append(lb.Height);
                sbSql.Append(",");
                sbSql.Append(lb.Left);
                sbSql.Append(",");
                sbSql.Append(lb.Top);
                sbSql.Append(",");
                sbSql.Append(lb.Font.Size);
                sbSql.Append(",'");
                sbSql.Append(info[7]);
                sbSql.Append("','");
                sbSql.Append(info[8]);
                sbSql.Append("','");
                sbSql.Append(info[0]);
                sbSql.Append("','");
                sbSql.Append(info[1]);
                sbSql.Append("','");
                sbSql.Append(info[2]);
                sbSql.Append("','");
                sbSql.Append(info[3]);
                sbSql.Append("','");
                sbSql.Append(info[4]);
                sbSql.Append("','");
                sbSql.Append(info[5]);
                sbSql.Append("','");
                sbSql.Append(info[6]);
                sbSql.Append("'),");
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            SQLiteCommand sqltcmd = new SQLiteCommand(sbSql.ToString(), Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void InserRInfo(SQLiteConnection Conn, int Date, List<ScrollingText> ListScroll)
        {
            String sql = String.Format("INSERT INTO Info{0} (TextNo, Content, Width, Height, Left, Top, FontSize, FontColor, BackColor, BorderL, BorderT, BorderR, BorderB, BorderColor, Type, Blink) VALUES ", Date);
            StringBuilder sbSql = new StringBuilder(sql);
            foreach (ScrollingText st in ListScroll)
            {
                String[] info = st.Tag.ToString().Split('#');
                sbSql.Append("('");
                sbSql.Append(st.Name);
                sbSql.Append("','");
                sbSql.Append(st.ScrollText);
                sbSql.Append("',");
                sbSql.Append(st.Width);
                sbSql.Append(",");
                sbSql.Append(st.Height);
                sbSql.Append(",");
                sbSql.Append(st.Left);
                sbSql.Append(",");
                sbSql.Append(st.Top);
                sbSql.Append(",");
                sbSql.Append(st.Font.Size);
                sbSql.Append(",'");
                sbSql.Append(info[7]);
                sbSql.Append("','");
                sbSql.Append(info[8]);
                sbSql.Append("','");
                sbSql.Append(info[0]);
                sbSql.Append("','");
                sbSql.Append(info[1]);
                sbSql.Append("','");
                sbSql.Append(info[2]);
                sbSql.Append("','");
                sbSql.Append(info[3]);
                sbSql.Append("','");
                sbSql.Append(info[4]);
                sbSql.Append("','");
                sbSql.Append(info[5]);
                sbSql.Append("','");
                sbSql.Append(info[6]);
                sbSql.Append("'),");
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            SQLiteCommand sqltcmd = new SQLiteCommand(sbSql.ToString(), Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void InserPInfo(SQLiteConnection Conn, int Date, List<PictureBox> ListPb)
        {
            String sql = String.Format("INSERT INTO Info{0} (TextNo, Content, Width, Height, Left, Top, FontSize, FontColor, BackColor, BorderL, BorderT, BorderR, BorderB, BorderColor, Type, Blink) VALUES ", Date);
            StringBuilder sbSql = new StringBuilder(sql);
            foreach (PictureBox pb in ListPb)
            {
                String[] info = pb.Tag.ToString().Split('#');
                sbSql.Append("('");
                sbSql.Append(pb.Name);
                sbSql.Append("','");
                sbSql.Append(pb.Text);
                sbSql.Append("',");
                sbSql.Append(pb.Width);
                sbSql.Append(",");
                sbSql.Append(pb.Height);
                sbSql.Append(",");
                sbSql.Append(pb.Left);
                sbSql.Append(",");
                sbSql.Append(pb.Top);
                sbSql.Append(",");
                sbSql.Append(pb.Font.Size);
                sbSql.Append(",'");
                sbSql.Append(info[7]);
                sbSql.Append("','");
                sbSql.Append(info[8]);
                sbSql.Append("','");
                sbSql.Append(info[0]);
                sbSql.Append("','");
                sbSql.Append(info[1]);
                sbSql.Append("','");
                sbSql.Append(info[2]);
                sbSql.Append("','");
                sbSql.Append(info[3]);
                sbSql.Append("','");
                sbSql.Append(info[4]);
                sbSql.Append("','");
                sbSql.Append(info[5]);
                sbSql.Append("','");
                sbSql.Append(info[6]);
                sbSql.Append("'),");
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            SQLiteCommand sqltcmd = new SQLiteCommand(sbSql.ToString(), Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void ShowProject(SQLiteConnection Conn, ListView listView)
        {
            String sql =String.Format(@"SELECT * FROM Project");
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            SQLiteDataReader dr = sqltcmd.ExecuteReader();
            listView.BeginUpdate();
            while (dr.Read())
            {
                ListViewItem lvi = new ListViewItem();
                if (dr["Date"].ToString() == "")
                {
                    lvi.Text = "";
                }
                else
                {
                    lvi.Text = UTC.ConvertLongDateTime(Convert.ToInt32(dr["Date"])).ToString("yyyy/MM/dd HH:mm:ss");
                }
                lvi.SubItems.Add(Convert.ToString(dr["Name"]));
                lvi.SubItems.Add(Convert.ToString(dr["Width"]));
                lvi.SubItems.Add(Convert.ToString(dr["Height"]));
                if(dr["Style"].ToString() == "T")
                {
                    lvi.SubItems.Add("双色");
                }
                else if(dr["Style"].ToString() == "F")
                {
                    lvi.SubItems.Add("全彩");
                }
                listView.Items.Add(lvi);
            }
            listView.EndUpdate();
            for (int i = 0; i < 4; i++)
            {
                listView.Columns[i].Width = -2;
            }//-2为自适应列的内容宽度 ，-1为自适应标题的宽度
            dr.Close();
        }

        public void LEDSize(SQLiteConnection Conn, int Date)
        {
            String sql = String.Format(@"SELECT Name,Width,Height,Style FROM Project WHERE Date = {0}", Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            SQLiteDataReader dr = sqltcmd.ExecuteReader();
            while (dr.Read())
            {
                Info.Name = dr.GetString(0);
                Info.LEDWidth = dr.GetInt32(1);
                Info.LEDHeight = dr.GetInt32(2);
                Info.LEDStyle = dr.GetString(3);
            }
            dr.Close();
        }

        public void UpdateProject(SQLiteConnection Conn, String Name, int Date,int Width,int Height)
        {
            String sql = String.Format(@"UPDATE Project SET Name = '{0}', Width ={1}, Height ={2} WHERE Date = {3}", Name, Width, Height, Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void DeleteProject(SQLiteConnection Conn, int Date)
        {
            String sql = String.Format(@"DELETE FROM Project WHERE Date = {0}", Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public void DropInfo(SQLiteConnection Conn, int Date)
        {
            String sql = String.Format(@"Drop TABLE Info{0} ", Date);
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            sqltcmd.ExecuteNonQuery();
        }

        public List<String> Dire(SQLiteConnection Conn)
        {
            List<string> listDire = new List<string>();
            String sql = string.Format(@"SELECT Date FROM Project ");
            SQLiteCommand sqltcmd = new SQLiteCommand(sql, Conn);
            SQLiteDataReader dr = sqltcmd.ExecuteReader();
            while (dr.Read())
            {
                listDire.Add(dr.GetInt64(0).ToString());
            }
            return listDire;
        }
        
        public DataTable SlecType(long UtcTime)
        {
            String sql = string.Format(@"SELECT * FROM Info{0}",UtcTime);
            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, ConnDB.Conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;            
        }      

    }
}
