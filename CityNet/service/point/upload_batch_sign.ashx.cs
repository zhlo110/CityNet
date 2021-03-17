using Aspose.Words;
using Aspose.Words.Tables;
using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CityNet.service.point
{
    /// <summary>
    /// upload_batch_sign 的摘要说明
    /// </summary>
    public class upload_batch_sign : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            HttpFileCollection files = context.Request.Files;
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            string progressid = context.Request["progressid"];
            //设定精度条
            this.initProgressBar(progressid, 1000);
            this.updateProgressText(progressid, "数据上传开始");
            this.updateProgress(progressid, 0);

            
            if (files.Count > 0)
            {
                //生成随机文件名
                string docname = Guid.NewGuid().ToString();
                //写文件
                string filePath = context.Server.MapPath("~/temps/");
                string fileName = files[0].FileName;
                string ext = Path.GetExtension(fileName);
                this.updateProgressText(progressid, "正在保存文件");
                this.updateProgress(progressid, 500);
                string docfilepath = Path.Combine(filePath, docname + ext);
                files[0].SaveAs(docfilepath);//保存到临时文件夹
                Document doc = new Document(docfilepath);

                this.updateProgressText(progressid, "文件保存完毕");
                this.updateProgress(progressid, 999);

                //读文件中的表
                SectionCollection sections = doc.Sections;
                int sectioncount = sections.Count;
                int s = 0;
                for (s = 0; s < sectioncount; s++) //读取每个section
                {
                    TableCollection tables = sections[s].Body.Tables;

                    this.initProgressBar(progressid, tables.Count);
                    this.updateProgressText(progressid, "正在读取第" + s.ToString() + "个section,共" + sectioncount.ToString() + "个");
                    this.updateProgress(progressid, 0);

                    for (int i = 0; i < tables.Count; i++)
                    {
                        this.updateProgress(progressid, i);
                        Table table = tables[i];//读取每一个表格
                        string html = table.ToString(SaveFormat.Html).Trim();
                       
                        IList list = new ArrayList();
                        double dlongitude = 0;
                        double dlatitude = 0;
                        string pointname = "";
                        parseTable(table, out pointname, out dlongitude, out dlatitude); //从表中解析数据
                        if (dlongitude > -900 && dlatitude > -900) //有效的数据
                        {
                            int pointid = getpointid(taskid, userid, pointname);
                            string sql = "update Point set longitude=@lon,latitude=@lat,Sign=@sign where ID=@id";
                            list.Clear();
                            list.Add(new DictionaryEntry("@lon", dlongitude));
                            list.Add(new DictionaryEntry("@lat", dlatitude));
                            list.Add(new DictionaryEntry("@sign", html));
                            list.Add(new DictionaryEntry("@id", pointid));
                            DBAccess.NoQuery(sql, list);
                        }
                    }
                }
                //删除临时文件
                if (File.Exists(docfilepath))
                {
                    File.Delete(docfilepath);
                }
            }
            System.GC.Collect();
            this.deleteProgress(progressid);
            returnInfo(context, "添加点之记成功");
            //pointname,latitude,file,
        }

        private int getpointid(int taskid,int userid,string pointname)
        {
            int ret = -1;
            string sql = "select top 1 PointID from Point_User_View where UserID=@uid and TaskID=@tid and PointName=@pname";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@pname", pointname));
            ret = DBAccess.QueryStatistic(sql, list);
            return ret;
        }
        private void parseTable(Table table,out string pontname, out double dlongitude, out double dlatitude)
        {
            pontname = "";
            dlongitude = -1000;
            dlatitude = -1000;

            RowCollection rows = table.Rows;
            for (int j = 0; j < rows.Count; j++)
            {
                CellCollection cells = rows[j].Cells;
                for (int k = 0; k < cells.Count; k++)
                {
                    string cellText = cells[k].ToString(SaveFormat.Text).Trim();
                    if (cellText.Equals("点名") && k + 1 < cells.Count)
                    {
                        pontname = cells[k + 1].ToString(SaveFormat.Text).Trim();
                        k++;
                    }
                    else
                    {
                        if (cellText.IndexOf('B') != -1 && cellText.IndexOf('=') != -1 && cellText.IndexOf('L') != -1)//初选
                        {
                            palseDegreeLine(cellText,out dlongitude,out dlatitude);
                        }
                    }
                }
            }
        }
        private void palseDegreeLine(string value, out double dlongitude, out double dlatitude)
        {
            //B=30°00′00.13″L=103°01′18.07″ 8个值
            //B=30.12 L=103.23 4个值
            dlongitude = -1000;
            dlatitude = -1000;

            string[] vec = value.Split(new char[] { '°', '′','\'', '〞', '"','″', '=', ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (vec.Length == 8)//度分秒格式
            {
                if (vec[0].Equals("B")) //纬度
                {
                    double degree = 0.0;
                    double minute = 0.0;
                    double second = 0.0;
                    bool s1 = double.TryParse(vec[1], out degree);
                    bool s2 = double.TryParse(vec[2], out minute);
                    bool s3 = double.TryParse(vec[3], out second);
                    if (s1 && s2 & s3)
                    {
                        dlatitude = degree + minute / 60.0 + second / 3600.0;
                    }
                    s1 = double.TryParse(vec[5], out degree);
                    s2 = double.TryParse(vec[6], out minute);
                    s3 = double.TryParse(vec[7], out second);
                    if (s1 && s2 & s3 && vec[4].Equals("L"))
                    {
                        dlongitude = degree + minute / 60.0 + second / 3600.0;
                    }
                }
                if (vec[0].Equals("L")) //纬度
                {
                    double degree = 0.0;
                    double minute = 0.0;
                    double second = 0.0;
                    bool s1 = double.TryParse(vec[1], out degree);
                    bool s2 = double.TryParse(vec[2], out minute);
                    bool s3 = double.TryParse(vec[3], out second);
                    if (s1 && s2 & s3)
                    {
                        dlongitude = degree + minute / 60.0 + second / 3600.0;
                    }
                    s1 = double.TryParse(vec[5], out degree);
                    s2 = double.TryParse(vec[6], out minute);
                    s3 = double.TryParse(vec[7], out second);
                    if (s1 && s2 & s3 && vec[4].Equals("B"))
                    {
                        dlatitude = degree + minute / 60.0 + second / 3600.0;
                    }
                }
            }
            else if(vec.Length == 4)//数字格式
            {
                if (vec[0].Equals("B")) //纬度
                {
                    if (!double.TryParse(vec[1], out dlatitude))
                    {
                        dlatitude = -1000;
                    }
                    if (vec[2].Equals("L"))
                    {
                        if (!double.TryParse(vec[3], out dlongitude))
                        {
                            dlongitude = -1000;
                        }
                    }
                }
                else if (vec[0].Equals("L"))
                {
                    if (!double.TryParse(vec[1], out dlongitude))
                    {
                        dlongitude = -1000;
                    }
                    if (vec[2].Equals("B"))
                    {
                        if (!double.TryParse(vec[3], out dlatitude))
                        {
                            dlatitude = -1000;
                        }
                    }
                }
            }
        }
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}