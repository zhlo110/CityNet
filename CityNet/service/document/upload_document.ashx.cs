using CityNet.security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Aspose.Words;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using System.Collections;
using CityNet.Utility;
using CityNet.Controllers;
using System.Threading;

namespace CityNet.service.document
{
    /// <summary>
    /// upload_document 的摘要说明
    /// </summary>
    public class upload_document : Security
    {
      /*  protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string progressid = context.Request["progressid"];
            int i;
            this.initProgressBar(progressid,1000);
            this.updateProgressText(progressid, "aaaa");
            for (i = 0; i < 1000; i++)
            {
                this.updateProgress(progressid,i);
                Thread.Sleep(100);
            }
            this.updateProgressText(progressid, "bbbb");
            this.initProgressBar(progressid, 1000);
            for (i = 0; i < 1000; i++)
            {
                this.updateProgress(progressid, i);
                Thread.Sleep(100);
            }

            this.deleteProgress(progressid);
            returnInfo(context, "上传成功");
        }*/

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
      //  protected  void fuctionimp1(HttpContext context, System.Collections.IList parameters)
        {
            //文件上传
            HttpFileCollection files = context.Request.Files;
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }

            string progressid = context.Request["progressid"];
            //设定精度条
            this.initProgressBar(progressid, 1000);
            this.updateProgressText(progressid, "正在读取文件");
            this.updateProgress(progressid, 0);

            string snotintask = context.Request["notintask"];
            int notintask = 0;
            if (!int.TryParse(snotintask, out notintask))
            {
                notintask = 0;
            }

            string fileline = context.Request["fileline"];
            int headline = 1;
            if (!int.TryParse(fileline, out headline))
            {
                headline = 1;
            }
            string result = context.Request["isresult"]; //是否为成果数据 1是，0否
            int iresult = 0;
            if (!int.TryParse(result, out iresult))
            {
                iresult = 0;
            }
            string insert = context.Request["inserttemptable"];
            int iinsert = 0;
            if (!int.TryParse(insert, out iinsert))//是否插入临时表
            {
                iinsert = 0;
            }

            if(files.Count > 0)
            {
                //生成随机文件名
                string docname = Guid.NewGuid().ToString();
                //写文件
                string filePath = context.Server.MapPath("~/documents/");
                string tempdir = context.Server.MapPath("~/temps/");

                string fileName = files[0].FileName;
                string ext = Path.GetExtension(fileName);
                string docfilepath = Path.Combine(filePath, docname + ext);
                files[0].SaveAs(docfilepath);
                
                //根据随机文件名生成临时目录
                string picpath = filePath + docname;
                if (!Directory.Exists(picpath))
                    Directory.CreateDirectory(picpath);

                //将word 渲染成一张张图片，并保存
                System.GC.Collect();

                Document doc = new Document(docfilepath);
                this.updateProgress(progressid, 500);
                int nCount = doc.PageCount;
                this.updateProgress(progressid, 999);
                int i;
                ImageSaveOptions imageOptions = new ImageSaveOptions(SaveFormat.Jpeg);
                imageOptions.JpegQuality = 100;
                imageOptions.PageCount = 1;

                this.initProgressBar(progressid, nCount);
                this.updateProgressText(progressid, "正在保存文件");
                this.updateProgress(progressid, 0);

                for (i = 0; i < nCount; i++)
                {
                    imageOptions.PageIndex = i;
                    this.updateProgress(progressid, i);
                    doc.Save(picpath+"\\"+ i.ToString() + ".jpg", imageOptions);
                }


                //插入document表
                string sql = "insert [Document](DocumentName,DocumentUrl,UpdataUserID,Description,PageCount,TaskID,IsResult,DepartmentID,notinTask) "
                    +"values(@doc,@docurl,@uid,@des,@pc,@tid,@res,@depid,@nointask)";

                string username = parameters[0].ToString();
                string password = parameters[1].ToString();
                int userid = LogUtility.GetUserID(username, password);
                string departmentid = parameters[2].ToString();
               // ../documents/d8601d99-3c14-43e1-8a10-74f37bca8830/
                string url = "../documents/"+docname+"/";

                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@doc", fileName));
                list.Add(new DictionaryEntry("@docurl", url));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@des", ""));
                list.Add(new DictionaryEntry("@pc", nCount));
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@res", iresult));
                list.Add(new DictionaryEntry("@depid", departmentid));
                list.Add(new DictionaryEntry("@nointask", notintask));
                DBAccess.NoQuery(sql, list);

                if (iresult > 0) //是否为成果数据 1是，0否
                {
                    sql = "select Max(ID) from Document";

                    int documentid = DBAccess.QueryStatistic(sql, null);
                    //读取表格数据

                    readDocTable(doc, headline, documentid, progressid);
                    
                }
                if (iinsert > 0) 
                {
                    sql = "select Max(ID) from Document";
                    int documentid = DBAccess.QueryStatistic(sql, null);
                    //插入临时表
                    list.Clear();
                    sql = "insert into TempApproveDocument(TaskID,UserID,DocumentID) values(@tid,@uid,@did)";

                    list.Add(new DictionaryEntry("@tid", taskid));
                    list.Add(new DictionaryEntry("@uid", userid));
                    list.Add(new DictionaryEntry("@did", documentid));
                    DBAccess.NoQuery(sql, list);


                }
                this.deleteProgress(progressid);
                returnInfo(context, "上传成功");
            }
            System.GC.Collect();
            if (files[0] != null && files[0].InputStream != null)
            {
                files[0].InputStream.Close();
                files[0].InputStream.Dispose();
            }

        }

        private string getTableHead(Table table,int headline)
        {
            string ret = "";
            if (headline > 0)
            {
                Node pNode = table;
                int step = headline;
                while (step > 0)
                {
                    if (pNode != null) pNode = pNode.PreviousSibling;
                    step--;
                }
                if (pNode != null)
                {
                    ret = pNode.ToString(SaveFormat.Text);
                }
            }
            else
            {
                ret = "没有读到表名";
            }
            return ret;
        }


        private void parseL0h(Table table,out string l0,out string h)
        {
            l0 = "";
            h = "";
            Node pNode = table.PreviousSibling;
            if (pNode != null)
            {
                string value = pNode.ToString(SaveFormat.Text);
                int index1 = value.IndexOf('（');
                int index2 = value.IndexOf('）');
                if (index1 >= 0 && index2 >= 0 && (index2 - index1) > 2)
                {
                    value = value.Substring(index1 + 1, index2 - index1 - 1);
                    string[] vec = value.Split(new char[] { '=', ' ', '　', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (vec.Length >= 4)
                    {
                        l0 = vec[1];
                        h = vec[3];
                    }
                    else if (vec.Length >= 2)
                    {
                        l0 = vec[0];
                        h = vec[1];
                    }
                }
            }
        }

        private string parseDescription(Table table)
        {
            string ret = "";
            Node pNode = table.NextSibling;
            if (pNode != null)
            {
                string value = pNode.ToString(SaveFormat.Text);
                ret = HttpUitls.String2Json(value);
            }
            
            return ret;
        }


        private void readDocTable(Document doc, int headline, int documentid, string progressid)
        {
            SectionCollection sections = doc.Sections;
            int sectioncount = sections.Count;
            int s = 0;
            for (s = 0; s < sectioncount; s++) //读取每个section
            {
                TableCollection tables = sections[s].Body.Tables;
                // Iterate through all tables in the document

                this.initProgressBar(progressid, tables.Count);
                this.updateProgressText(progressid, "正在解析第" + s.ToString() + "个section,共" + sectioncount.ToString() + "个");
                this.updateProgress(progressid, 0);

                for (int i = 0; i < tables.Count; i++)
                {
                    this.updateProgress(progressid, i);
                    Table table = tables[i];//读取每一个表格
                    string tablename = getTableHead(table, headline);
                    string l0 = "";
                    string h = "";
                    string html = table.ToString(SaveFormat.Html);//原来数据
                    parseL0h(table, out l0, out h);
                    string descriontion = parseDescription(table);
                    //插入DocumentTable表
                    string sql = "insert into DocumentTable(TableName,TableHtml,Description,DocumentID,deletestate,L0,h) " +
                        "values(@tn,@html,@des,@did,@dstate,@l0,@h)";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@tn", tablename));
                    list.Add(new DictionaryEntry("@html", html));
                    list.Add(new DictionaryEntry("@des", descriontion));
                    list.Add(new DictionaryEntry("@did", documentid));
                    list.Add(new DictionaryEntry("@dstate", 0));
                    list.Add(new DictionaryEntry("@l0", l0));
                    list.Add(new DictionaryEntry("@h", h));
                    DBAccess.NoQuery(sql, list);
                    
                    sql = "select Max(ID) from DocumentTable";
                    int tableid = DBAccess.QueryStatistic(sql, null);

                    //读取表中的每一行
                    RowCollection rows = table.Rows;
                    int maxcolumn = 0;
                    for (int j = 0; j < rows.Count; j++) //读取每一行
                    {
                        CellCollection cells = rows[j].Cells;
                        string rowtext = "";
                        bool isdata = false; //过滤表头
                        
                        for (int k = 0; k < cells.Count; k++) //读取每一个单元格
                        {
                            string cellText = cells[k].ToString(SaveFormat.Text).Trim();
                            double value = 0.0;
                            if (double.TryParse(cellText, out value))
                            {
                                if (maxcolumn < cells.Count) maxcolumn = cells.Count; //获取最大的列数
                                isdata = true;
                            }
                            rowtext += "["+cellText + "]#";
                        }
                        if (isdata) //插入数据
                        {
                            if (rowtext.Length > 0)
                            {
                                rowtext = rowtext.Substring(0, rowtext.Length - 1);
                                sql = "insert into DocumentDataRow(TableID,[Row],qtip) values(@tid,@row,@qtip)";
                                list.Clear();
                                list.Add(new DictionaryEntry("@tid", tableid));
                                list.Add(new DictionaryEntry("@row", rowtext));
                                list.Add(new DictionaryEntry("@qtip", "数据待入库"));
                                DBAccess.NoQuery(sql, list);
                            }
                        }
                    }

                    //更新最大列数
                    sql = "update DocumentTable set MaxColumns = @mc where [ID] = @id";
                    list.Clear();
                    list.Add(new DictionaryEntry("@mc", maxcolumn));
                    list.Add(new DictionaryEntry("@id", tableid));
                    DBAccess.NoQuery(sql, list);

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