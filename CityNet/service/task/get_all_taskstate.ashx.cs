using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// get_all_taskstate 的摘要说明
    /// </summary>
    public class get_all_taskstate : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string sql = "select count(ID) from [SubmitState]";
            int totalCount = DBAccess.QueryStatistic(sql, null);
            string children = "";
            int pagesize = limit;

            //分页
            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by ss.ID desc) as RowNum, " +
                "ss.*,u.ID as uerID ,u.RealName from [SubmitState] ss ,[User] u " +
                "where ss.CreateID = u.ID) as unions " +
                "where unions.RowNum between @start and @end";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "Group", list);

            string currentuserid = context.Request["userid"];
            int icurrentid = -1;
            if (currentuserid == null)
            {
                currentuserid = "";
            }
            else
            {
                if (!int.TryParse(currentuserid, out icurrentid))
                {
                    icurrentid = -1;
                }
            }
            list.Clear();
            currentuserid = currentuserid.Trim();
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
                        string StateName = row["Name"].ToString();
                        string description = row["Description"].ToString();
                        string createName = row["RealName"].ToString();
                        string dtime = ((DateTime)row["CreateTime"]).ToString("yyyy年MM月dd日");
                        string editable = row["Editable"].ToString();
                        string priority = row["Priority"].ToString();
                        string beditable = "false";
                        int iedit = 0;
                        if (!int.TryParse(editable, out iedit))
                        {
                            iedit = 0;
                        }
                        if (iedit > 0)
                        {
                            beditable = "true";
                        }

                        children += "{\"stateid\":" + ID.ToString() + ",\"statename\":\"" + StateName
                            + "\"," + "\"statepriority\":" + priority + ",\"creator\":\"" + createName + "\",\"description\":\"" +
                           description + "\",editable:" + beditable + ",\"createdate\": \"" + dtime + "\"},";
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