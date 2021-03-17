using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class Point
    {
        double talorent = 0.0001;
        public IList IDS = new ArrayList();
        public IList Names = new ArrayList();
        public HashSet<string> Colors = new HashSet<string>();
        public double longitude = -1000.0;
        public override bool Equals(object obj)
        {
            Point p = (Point)obj;
            if (Math.Abs(this.latitude - p.latitude) < talorent &&
                Math.Abs(this.longitude - p.longitude) < talorent)
            {
                this.IDS.Add(p.IDS[0]);
                this.Names.Add(p.Names[0]);

                foreach (string c in p.Colors)//复制color
                {
                    this.Colors.Add(c);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode()
        {
            return 1;
        }
        public double latitude = -1000.0;

        public static string getincondition(IList taskids)
        {
            int i;
            int nCount = taskids.Count;
            string inconditon = "";
            for (i = 0; i < nCount; i++)
            {
                inconditon += taskids[i].ToString() + ",";
            }
            if (inconditon.Length > 0)
            {
                inconditon = inconditon.Substring(0, inconditon.Length - 1);
            }
            return inconditon;

        }
        //获取一系列数据所有的报警数据
        public void calculateAlarm(out int alarmid, out string colors)
        {
            //默认
            alarmid = -1;
            colors = "";
            //查找PointAlarm 有没有该点
            //生成pointid
            if (IDS.Count > 0)
            {
                string strids = Point.getArrayIDStr(IDS,",");
                string insql = "select distinct ID from Point_View where PointID in (" + strids + ")";
                string sql = "select x,y,z,parallelbais,gravitybais,x_des,y_des,z_des,parallelbais_des,gravitybais_des"
                    +" from PointAlarm where MeasurePointID in (" + insql + ")";
                DataSet ds = DBAccess.Query(sql, "PointAlarm");
                HashSet<int> alarmset = new HashSet<int>();
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        int nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++)
                        {
                            DataRow row = dt.Rows[i];
                            addidtoset(alarmset, row, "x");
                            addidtoset(alarmset, row, "y");
                            addidtoset(alarmset, row, "z");
                            addidtoset(alarmset, row, "parallelbais");
                            addidtoset(alarmset, row, "gravitybais");
                        }
                    }
                }
                if (alarmset.Count>0)
                {
                    string sids = "";
                    foreach(int value in alarmset)
                    {
                        sids += value.ToString() + ",";
                    }
                    if (sids.Length > 0)
                    {
                        sids = sids.Substring(0,sids.Length-1);
                    }
                    if (sids.Length > 0)
                    {
                        //获取报警级别最高的那列
                      //  insql = "select Max(AlarmLevel) from AlarmScheme where ID in(" + sids + ")";
                      //  sql = "select top 1 ID,color from AlarmScheme where AlarmLevel = (" + insql + ")";
                        sql = "select top 1 ID,color from AlarmScheme where ID in(" + sids 
                            + ") order by AlarmLevel desc";
                        ds = DBAccess.Query(sql, "AlarmScheme");
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                DataTable dt = ds.Tables[0];
                                int nCount = dt.Rows.Count;
                                if (nCount > 0)
                                {
                                    DataRow row = dt.Rows[0];
                                    colors = DatabaseUtility.getStringValue(row, "color").Trim();
                                    alarmid = DatabaseUtility.getIntValue(row, "ID", -1);
                                }
                            }
                        }
                        
                    }

                }

            }
            

        }

        private void addidtoset(HashSet<int> alarmset,DataRow row,string column)
        {
            string des = DatabaseUtility.getStringValue(row, column + "_des").Trim();
            if (des.Length <= 0) //没有消警
            {
                int value = DatabaseUtility.getIntValue(row, column, -1);
                if (value > 0)
                {
                    alarmset.Add(value);
                }
            }
        }

        //更加taksid获取
        public static void getSchemeColor(CityNet.Utility.Point point, IList taskids)
        {
            int i;
            int nCount;
            string inconditon = Point.getincondition(taskids);
            
            string pointid = point.IDS[0].ToString();//新的pointID,如果经纬度一致，该ID要与其他的Point合并
            string sql = "select color from TableScheme where ID in"
                + "(select distinct SchemeID from Point_User_View where PointID=@pid and TaskID in(" + inconditon + "))";
         //   sql = "exec('"+sql+"')";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", pointid));
            list.Add(new DictionaryEntry("@inconditon", inconditon));
            DataSet ds = DBAccess.Query(sql, "TableScheme", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    nCount = dt.Rows.Count;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string color = DatabaseUtility.getStringValue(row, "color").Trim();
                        point.Colors.Add(color);
                    }
                }
            }
        }


        public static string getArrayColorStr(HashSet<string> Colors)
        {
            string str = "";
            int i;
            foreach (string c in Colors)
            {
                str += "\"" + c + "\",";
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            str = "[" + str + "]";
            return str;
        }

        public static string getArrayIDStr(IList list, string split)
        {
            string str = "";
            int i;
            int nCount = list.Count;
            for (i = 0; i < nCount; i++)
            {
                str += list[i].ToString() + split;
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        public static string getArrayListStr(IList list, string split)
        {
            string str = "";
            int i;
            int nCount = list.Count;
            for (i = 0; i < nCount; i++)
            {
                str += "\"" + list[i].ToString() + "\"" + split;
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            str = "[" + str + "]";
            return str;
        }

        public static string GenPie(int userid, string filename, HttpContext context, HashSet<string> Colors)
        {
            int picsize = 32;
            Bitmap image = new Bitmap(picsize, picsize);
            Graphics g = Graphics.FromImage(image);
            //消除锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //质量
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            //  Brush blackBrush = new SolidBrush();
            //  Pen blackPen = new Pen(blackBrush, 1);

            //填充区域内容--背景
            g.Clear(Color.FromArgb(0, 0, 0, 0));

            int radius = (picsize - 2) / 2;
            int len = Colors.Count;
            float angle = 360.0f / (float)len;
            int i = 0;
            foreach (string c in Colors)
            {
                float startangle = i * angle;
                float endangle = (i + 1) * angle;
                if (endangle > 360) endangle = 360;
                Color color = System.Drawing.ColorTranslator.FromHtml(c);
                //  color = Color.FromArgb(10, color);
                Brush brush = new SolidBrush(color);

                g.FillPie(brush, 2, 2, (int)2 * radius, (int)2 * radius, (int)startangle, (int)angle);
                brush.Dispose();
                i++;
                //  break;
            }

            string filePath = context.Server.MapPath("~/pies/");


            string filefullName = userid.ToString() + "_" + filename + ".png";
            filefullName = filefullName.Replace("\"", "");
            filefullName = filefullName.Replace("[", "");
            filefullName = filefullName.Replace("]", "");

            string docfilepath = Path.Combine(filePath, filefullName);
            try
            {
                if (File.Exists(docfilepath))
                {
                    File.Delete(docfilepath);
                }
                image.Save(docfilepath);
            }
            catch(Exception ex){
            }
            image.Dispose();
            return "\"../pies/" + filefullName + "\"";

            //g.FillPie

            // blackBrush.Dispose();
            //blackPen.Dispose();

        }

    }
    

}