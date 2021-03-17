using System;
using System.Collections.Generic;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;
using System.Collections;
using System.Data;

namespace CityNet.service.function
{
    /// <summary>
    /// getfunctionbyclass 的摘要说明
    /// </summary>
    public class getfunctionbyclass : Security
    {
        protected override string getErrorMessage()
        {
            return "{\"totalCount\":0,\"roots\":[]}";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //分页显示功能
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            int groupid = -1;
            if (context.Request["groupid"] != null)
            {
                if (!int.TryParse(context.Request["groupid"], out groupid))
                {
                    groupid = -1;
                }
            }
            string ret = getErrorMessage();

            string sql = "select count(ID) from Action";
            int totalCount = DBAccess.QueryStatistic(sql, null);
            string children = "";
            int pagesize = limit;

            sql =
            "select * from(" +
            "select ROW_NUMBER() OVER(Order by ac.ID desc) as RowNum, " +
            "ac.*,u.ID as uerID ,u.RealName from [Action] ac ,[User] u " +
            "where ac.CreateID = u.ID) as unions " +
            "where unions.RowNum between @start and @end";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));
            DataSet ds = DBAccess.Query(sql, "Group", list);

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
                         string ActionName = row["ActionName"].ToString();
                         string ActionUrl = row["ActionUrl"].ToString();
                         string CreateTime = ((DateTime)row["ActionCreateTime"]).ToString("yyyy年MM月dd日");
                         string description = row["Description"].ToString();
                         string createName = row["RealName"].ToString();
                         string priority = row["Priority"].ToString();
                         string functioncheck = "false";
                         int aid = int.Parse(row["ActionGroupID"].ToString());
                         if (aid == groupid && groupid != -1)
                         {
                             functioncheck = "true";
                         }
                         children += "{\"functionid\":" + ID.ToString() + "," + "\"functioncheck\":" +
                           functioncheck.ToString() + ",\"functionname\":\"" + ActionName + "\",\"functionurl\":\"" +
                           ActionUrl.ToString() + "\",\"functiondate\": \"" + CreateTime + "\",\"priority\":" + priority 
                           + ",\"functiondcreator\":\"" + createName + "\",\"description\":\"" + description + "\"},";

                     }
                     if (children.Length > 0)
                     {
                         children = children.Substring(0, children.Length - 1);
                     }
                     ret = "{\"totalCount\":\"" + totalCount.ToString() + "\",\"roots\":[" + children + "]}";
                 }
             }
             context.Response.Write(ret);
        }
        
    }
}