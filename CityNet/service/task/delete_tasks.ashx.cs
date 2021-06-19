using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// delete_tasks 的摘要说明
    /// </summary>
    public class delete_tasks : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
           // throw new NotImplementedException();
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid,out itaskid))
            {
                itaskid = -1;
            }
            //1 删除Session_Task中的task关联 --
            string sql = "delete from Session_Task where TaskID=@taskid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@taskid", itaskid));
            DBAccess.NoQuery(sql, list);


            //2 删除Task_Visible --
            sql = "delete from Task_Visible where TaskID=@taskid";
            DBAccess.NoQuery(sql, list);

            //3 删除Approve --
            sql = "delete from Approve where TaskID=@taskid";
            DBAccess.NoQuery(sql, list);

            //4 删除TempApproveDocument --
            sql = "delete from TempApproveDocument where TaskID=@taskid";
            DBAccess.NoQuery(sql, list);

            //5 删除MeasurePoint (该表关联PointAlarm)
            //5.1 删除PointAlarm 不算的
            string insql = "select distinct ID from MeasurePoint where TaskID = @taskid";
            sql = "delete from PointAlarm where MeasurePointID in(" + insql + ")";
            DBAccess.NoQuery(sql, list);
            //5.2 删除MeasurePoint  --
            sql = "delete from MeasurePoint where TaskID = @taskid";
            DBAccess.NoQuery(sql, list);

            //6 删除Document --
            sql = "select ID from [Document] where TaskID = @taskid";
            DataSet ds = DBAccess.Query(sql, "Document", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0;  i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int idocid = DatabaseUtility.getIntValue(row, "ID", -1);
                        if (idocid > 0)
                        {
                            deletedocumentbyid(idocid, context);
                        }
                    }
                }
            }
            //7,删除关联的工作基点 --
            sql = "delete from BasePoint where TaskID = @taskid";
            DBAccess.NoQuery(sql, list);

            //8.删除关联的ProjectSite--
            sql = "delete from ProjectSite where TaskID = @taskid";
            DBAccess.NoQuery(sql, list);

            //9 删除关联的的Department_Task --
            sql = "delete from Department_Task where TaskID = @taskid";
            DBAccess.NoQuery(sql, list);



            //10 删除关联的的TableScheme --
           // sql = "delete from TableScheme where TaskID = @taskid";
           // DBAccess.NoQuery(sql, list);

            // ... 后续关联的资源删除网后加
            //最后 删除task表
            sql = "delete from Task where ID = @taskid";
            DBAccess.NoQuery(sql, list);
            returnInfo(context, "删除任务成功");
        }

        protected void deletedocumentbyid(int idocid,HttpContext context)
        {
       
            //删除 DocumentDataRow
            string sql = "delete from DocumentDataRow where ID in" +
                "(select ddr.ID from DocumentDataRow ddr left join " +
                "DocumentTable dt on dt.ID = ddr.TableID where dt.DocumentID = @docid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@docid", idocid));
            DBAccess.NoQuery(sql, list);

            sql = "delete from DocumentTable where DocumentID = @docid";
            DBAccess.NoQuery(sql, list);

            sql = "delete from ApproveDocument where DocumentID = @docid";
            DBAccess.NoQuery(sql, list);

            sql = "delete from TempApproveDocument where DocumentID = @docid";
            DBAccess.NoQuery(sql, list);


            sql = "select DocumentUrl,DocumentName from Document where ID = @docid";
            //DBAccess.Query(sql, list);
            DataSet ds = DBAccess.Query(sql, "Document", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        string url = DatabaseUtility.getStringValue(row, "DocumentUrl");
                        string name = DatabaseUtility.getStringValue(row, "DocumentName");

                        string filePath = context.Server.MapPath("~/documents/");
                        string ext = Path.GetExtension(name);
                        string[] vec = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        string filewithoutext = "";
                        if (vec.Length > 0)
                        {
                            filewithoutext = vec[vec.Length - 1];
                        }
                        string filename = filewithoutext + ext;
                        if (File.Exists(filePath + "\\" + filename))
                        {
                            File.Delete(filePath + "\\" + filename);
                        }
                        if (Directory.Exists(filePath + "\\" + filewithoutext))
                        {
                            Directory.Delete(filePath + "\\" + filewithoutext, true);
                        }
                    }
                }
            }

            sql = "delete from Document where ID = @docid";
            DBAccess.NoQuery(sql, list);
            
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}