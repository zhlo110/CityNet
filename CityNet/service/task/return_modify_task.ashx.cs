using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// return_modify_task 的摘要说明
    /// </summary>
    ///taskid: taskid,
    ///description: htmlvalue 

    //退回修改
    public class return_modify_task : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }

            string sauditid = context.Request["auditid"];
            int iauditid = -1;
            if (!int.TryParse(sauditid, out iauditid))
            {
                iauditid = -1;
            }

            

            string username = (string)parameters[0];
            string password = (string)parameters[1];
            string description = context.Request["description"];
            description = System.Web.HttpUtility.HtmlDecode(description);

            int userid = LogUtility.GetUserID(username, password);
            
            int auditid = -1;
            IList list = new ArrayList();
            string sql = "";

            //查询task的最新状态
            sql = "select Max(Priority) from SubmitState where ID=(select StateID from Task where ID=@tid)";
            list.Add(new DictionaryEntry("@tid", taskid));
            int istatepriority = DBAccess.QueryStatistic(sql, list);

            //1插入Approve表
            //(1)查找创建人ID
            if (iauditid <= 0)
            {
                if (istatepriority < 5)
                {
                    sql = "select CreateID from Task where ID =@tid";
                    list.Clear();
                    list.Add(new DictionaryEntry("@tid", taskid));
                    auditid = DBAccess.QueryStatistic(sql, list);//返修的对象
                }
                else
                {
                    sql = "select SubmitID from Approve where SubmitTime = "+
                        "(select Max(SubmitTime) from Approve where  TaskID = @tid)";
                    list.Clear();
                    list.Add(new DictionaryEntry("@tid", taskid));
                    auditid = DBAccess.QueryStatistic(sql, list);//返修的对象
                }
            }
            else
            {
                auditid = iauditid;
            }

            sql = "insert into Approve (TaskID,Description,AuditID,SubmitTime,SubmitID) values(@tid,@des,@aid,@st,@sid)";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@des", description));
            list.Add(new DictionaryEntry("@aid", auditid));
            list.Add(new DictionaryEntry("@st", DateTime.Now));
            list.Add(new DictionaryEntry("@sid", userid));
            DBAccess.NoQuery(sql, list);
            sql = "select Max(ID) from Approve";
            //获取最新Approve ID
            int approveid =  DBAccess.QueryStatistic(sql,null);
            bool finished = false;//是否完成了审核
           

             if (iauditid <= 0) //退修
             {
                 sql = "select ID from SubmitState where Priority=3";
             }
             else if (iauditid == userid)//审核完成
             {
                 if (istatepriority == 5)
                 {
                     sql = "select ID from SubmitState where Priority=6";// 完成审核
                 }
                 else
                 {
                     sql = "select ID from SubmitState where Priority=4";// 完成审核
                 }
                 finished = true;
             }
             else
             {
                 sql = "select ID from SubmitState where Priority=2";//提交下个审核人
             }
             
            
            int substateid = DBAccess.QueryStatistic(sql, null);
            if (!finished) //未完成
            {
                sql = "update Task set StateID=@sid where ID=@tid";
            }
            else //完成
            {
                sql = "update Task set StateID=@sid,stepDescription='已完成' where ID=@tid";
            }
            if (finished || istatepriority != 5)
            {
                list.Clear();
                list.Add(new DictionaryEntry("@sid", substateid));
                // list.Add(new DictionaryEntry("@aid", auditid));
                list.Add(new DictionaryEntry("@tid", taskid));
                DBAccess.NoQuery(sql, list);
            }
            

            //修改Task的审核人
            sql = "update Task set AuditID=@aid where ID=@tid and CreateID <> @aid";
            list.Clear();
            list.Add(new DictionaryEntry("@aid", auditid));
            list.Add(new DictionaryEntry("@tid", taskid));
            DBAccess.NoQuery(sql, list);

            //将审核人添加到Task_Visible中
            if (auditid > 0)
            {
                sql = "IF NOT EXISTS(select ID from Task_Visible where TaskID=@tid and UserID=@uid) " +
                              "insert into Task_Visible(TaskID,UserID,UserType) values(@tid,@uid,@type)";
                list.Clear();
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@uid", auditid));
                list.Add(new DictionaryEntry("@type", 2)); //type为审核者
                DBAccess.NoQuery(sql, list);
            }

            //把TempApproveDocument 的内容移植到 ApproveDocument表
            sql = "select DocumentID from TempApproveDocument where TaskID=@tid and UserID=@uid";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@uid", userid));

           
            DataSet ds = DBAccess.Query(sql, "TempApproveDocument", list);
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
                        int DocumentID = DatabaseUtility.getIntValue(row, "DocumentID", -1);
                        sql = "select count(ID) from ApproveDocument where ApproveID = @aid and DocumentID = @did";
                        list.Clear();
                        list.Add(new DictionaryEntry("@aid", approveid));
                        list.Add(new DictionaryEntry("@did", DocumentID));
                        int length = DBAccess.QueryStatistic(sql, list);
                        if (DocumentID > 0 && length == 0)
                        {
                            sql = "insert into ApproveDocument(ApproveID,DocumentID) values(@aid,@did)";
                            DBAccess.NoQuery(sql, list);
                        }
                    }
                }
            }
            
            //删除TempApproveDocument
            sql = "delete from TempApproveDocument where TaskID=@tid and UserID=@uid";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@uid", userid));
            DBAccess.NoQuery(sql, list);
            returnInfo(context,"提交成功");
        }
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}