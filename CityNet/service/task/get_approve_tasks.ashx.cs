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
    /// get_approve_tasks 的摘要说明
    /// </summary>
    public class get_approve_tasks : Security
    {
         //获取该需要该用户审批的任务task
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            string mode = context.Request["mode"];
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            string children = "";
            int imode = 0;//0,提交模式 1审批模式
            if (!int.TryParse(mode, out imode))
            {
                imode = 0;
            }

            string tasktype = context.Request["tasktype"];
            int itasktype = 1;
            if (tasktype != null)
            {
                tasktype = tasktype.Trim();
                if (!int.TryParse(tasktype, out itasktype))
                {
                    itasktype = 1;
                }
            }

            string spriority = context.Request["priority"];
            int priority = 2;
            if (spriority != null)
            {
                spriority = spriority.Trim();
                if (!int.TryParse(spriority, out priority))
                {
                    priority = 2;
                }
            }

            //返修的除外
            string sql = "select count(ID) from Task_View where ID in(" +
                         "select distinct TaskID from Approve where SubmitTime in" +
                         "(select MAX(SubmitTime) as SubmitTime from Approve group by TaskID) and AuditID=@uid)" +
                         "and [priority] = @pri and taskType=@ttp";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@ttp", itasktype));
            list.Add(new DictionaryEntry("@pri", priority));
            int totalCount = DBAccess.QueryStatistic(sql, list);

            string insql = "select ID from Task_View where ID in(" +
                         "select distinct TaskID from Approve where SubmitTime in" +
                         "(select MAX(SubmitTime) as SubmitTime from Approve group by TaskID) and AuditID=@uid)" +
                         "and [priority] = @pri and taskType=@ttp";

            sql =
                "select * from( " +
                "select ROW_NUMBER() OVER(Order by tv.ID desc) as RowNum," +
                "tv.* from Task_View tv where tv.ID in(" + insql + ")) as unions " +
                "where unions.RowNum between @start and @end";

            list.Clear();
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@ttp", itasktype));
            list.Add(new DictionaryEntry("@pri", priority));
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));
            DataSet ds = DBAccess.Query(sql, "Task_view", list);
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
                        int ID = int.Parse(row["ID"].ToString());
                        string TaskName = row["TaskName"].ToString();
                        string dtime = ((DateTime)row["FirstSubmitTime"]).ToString("yyyy年MM月dd日");
                        string creator = row["createname"].ToString();
                        string createDep = row["CreateDName"].ToString();

                        string auditor = row["auditname"].ToString();
                        string auditdep = row["AuditDName"].ToString();
                        string state = row["statename"].ToString();
                        string statedes = row["stepDescription"].ToString();

                        //int j;

                        children += "{\"taskid\":" + ID.ToString() + ",\"taskname\":\"" + TaskName
                            + "\"," + "\"taskcreatedate\":\"" + dtime + "\",\"creator\":\"" + creator + "\",\"creatordep\":\"" +
                           createDep + "\",\"auditor\": \"" + auditor + "\",auditordep:\"" + auditdep + "\",buttonName:\"成果审批\",state:\"" + state + "(" + statedes + ")\"},";

                    }

                    if (children.Length > 0)
                    {
                        children = children.Substring(0, children.Length - 1);
                    }
                    ret = "{\"totalCount\":" + totalCount.ToString() + ",\"roots\":[" + children + "]}";
                }
            }
            context.Response.Write(ret);
        }

        protected override string getErrorMessage()
        {
            return "{\"totalCount\":0,\"roots\":[]}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }

    }
}