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
    /// get_available_task 的摘要说明
    /// </summary>
    public class get_available_task : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);

            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string sql = "";
            string children = "";
            int pagesize = limit;
            IList list = new ArrayList();
            sql = "select COUNT(ID) from Task_View  where taskType=@ty and priority=@pri and stepDescription=@des";
            list.Add(new DictionaryEntry("@ty", 1));
            list.Add(new DictionaryEntry("@pri", 4));
            list.Add(new DictionaryEntry("@des", "已完成"));
            int totalCount = DBAccess.QueryStatistic(sql, list);

            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by tv.ID desc) as RowNum," +
                "tv.* from Task_View tv where tv.taskType=1 and tv.priority=4 and tv.stepDescription='已完成') as unions " +
                "where unions.RowNum between @start and @end";
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
                           createDep + "\",\"auditor\": \"" + auditor + "\",auditordep:\"" + auditdep + "\",buttonName:\"\",state:\"" + state + "(" + statedes + ")\"},";

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