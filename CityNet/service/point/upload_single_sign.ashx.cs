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
    /// upload_single_sign 的摘要说明
    /// </summary>
    public class upload_single_sign : Security
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
            string spointid = context.Request["pointid"];
            int pointid = -1;
            if (!int.TryParse(spointid, out pointid))
            {
                pointid = -1;
            }
            string longitude = context.Request["longitude"];
            string latitude = context.Request["latitude"];
            double dlongitude = palseDegree(longitude);
            double dlatitude = palseDegree(latitude);

            string progressid = context.Request["progressid"];
            //设定精度条
            this.initProgressBar(progressid, 1000);
            this.updateProgressText(progressid, "数据上传开始");
            this.updateProgress(progressid, 0);

            if (dlongitude > -999.0 && dlatitude > -999.0) //数据有效
            {
                string html = "";
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

                    this.initProgressBar(progressid, sectioncount);
                   
                    this.updateProgress(progressid, 0);
                    for (s = 0; s < sectioncount; s++) //读取每个section
                    {
                        this.updateProgressText(progressid, "正在读取第" + s.ToString() + "个section,共" + sectioncount.ToString() + "个");
                        this.updateProgress(progressid, s);
                        TableCollection tables = sections[s].Body.Tables;
                        if (tables.Count > 0) //只读第一个表
                        {
                            Table table = tables[0];
                            html = table.ToString(SaveFormat.Html).Trim();
                            break;
                        }
                    }
                    //删除临时文件
                    if (File.Exists(docfilepath))
                    {
                        File.Delete(docfilepath);
                    }
                }
                string sql = "update Point set longitude=@lon,latitude=@lat,Sign=@sign where ID=@id";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@lon", dlongitude));
                list.Add(new DictionaryEntry("@lat", dlatitude));
                list.Add(new DictionaryEntry("@sign",html));
                list.Add(new DictionaryEntry("@id", pointid));
                DBAccess.NoQuery(sql,list);

            }
            this.deleteProgress(progressid);
            System.GC.Collect();
            returnInfo(context, "添加点之记成功");
            //pointname,latitude,file,
        }
        private double palseDegree(string value)
        {
            double ret = -1000.0;
            if (!double.TryParse(value, out ret))
            {
                string[] vec = value.Split(new char[] {'°','′','″'}, StringSplitOptions.RemoveEmptyEntries);
                if (vec.Length == 3)
                {
                    double degree = 0.0;
                    double minute = 0.0;
                    double second = 0.0;
                    double.TryParse(vec[0], out degree);
                    double.TryParse(vec[1], out minute);
                    double.TryParse(vec[2], out second);

                    ret = degree + minute / 60.0 + second / 3600.0;
                }
            }
            return ret;
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