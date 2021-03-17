using CityNet.Controllers;
using CityNet.Utility;
using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using CityNet.security;
using System.Data;

namespace CityNet.service.function
{
    /// <summary>
    /// get_allrole 的摘要说明
    /// </summary>
    public class get_allrole : Security
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
           

            //分页显示权限组
            string ret = getErrorMessage();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string sql = "select count(ID) from [Group]";
            int totalCount = DBAccess.QueryStatistic(sql, null);
            string children = "";
            int pagesize = limit;
            //分页
            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by g.ID desc) as RowNum, " +
                "g.*,u.ID as uerID ,u.RealName from [Group] g ,[User] u " +
                "where g.CreateID = u.ID) as unions " +
                "where unions.RowNum between @start and @end";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@start", start+1));
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
                if(!int.TryParse(currentuserid,out icurrentid))
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
                        string GroupName = row["GroupName"].ToString();
                        string RealName = row["RealGroupName"].ToString();
                        string description = row["Description"].ToString();
                        string createName = row["RealName"].ToString();
                        string userid = row["uerID"].ToString().Trim();
                        string rolecheck = "false";
                        sql = "select count(*) from User_Group where UserID=@uid and GroupID = @gid";
                        list.Clear();
                        list.Add(new DictionaryEntry("@uid", icurrentid));
                        list.Add(new DictionaryEntry("@gid", ID));
                        if (DBAccess.QueryStatistic(sql, list) > 0)
                        {
                            rolecheck = "true";
                            //rolecheck
                        }
                        children += "{\"roleid\":" + ID.ToString() + ",\"rolecheck\":" + rolecheck
                            +"," + "\"rolename\":\"" +GroupName.ToString() + "\",\"realname\":\"" + RealName + "\",\"description\":\"" +
                           description.ToString() + "\",\"rolecreator\": \"" + createName + "\"},";
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
    }
}