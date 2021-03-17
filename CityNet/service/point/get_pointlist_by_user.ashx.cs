using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.point
{
    /// <summary>
    /// get_pointlist_by_user 的摘要说明
    /// </summary>
    public class get_pointlist_by_user : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = getErrorMessage();
            string children = "";
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string searchtext = context.Request["searchtext"];
            bool searchmode = false;
            string extsql = "";
            if (searchtext != null && searchtext.Length > 0)
            {
                extsql = " and PointName like '%'+@seachtext+'%'";
                searchmode = true;
            }

            int userid = LogUtility.GetUserID(username, password);


            // 从Task_Visible取得TaskID
            // task必须是完成的审批的项目
            string sql = "select tv.TaskID,taskview.priority from Task_Visible tv left join Task_View taskview on tv.TaskID=taskview.ID " +
                         "where tv.UserID=@uid and taskview.priority=4";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userid));
            HashSet<int> taskset = new HashSet<int>();//所有的TaskID
            DataSet ds = DBAccess.Query(sql, "Task_Visible", list);
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int taskid = DatabaseUtility.getIntValue(row, "TaskID", -1);
                        taskset.Add(taskid);
                    }
                }
            }
            IList taskids = new ArrayList();
            foreach (int value in taskset)
            {
                taskids.Add(value);
            }
            string inconditon = CityNet.Utility.Point.getincondition(taskids);

            sql = "select count(distinct PointID) from Point_User_View where TaskID in(" + inconditon + ") and UserID=@uid" + extsql;
            list.Clear();

            list.Add(new DictionaryEntry("@uid", userid));
            if (searchmode)
            {
                list.Add(new DictionaryEntry("@seachtext", searchtext.Trim()));
            }
            int totalCount = DBAccess.QueryStatistic(sql, list);

            //根据userid 和taskid找点
            sql = "select * from( " +
                  "select unions.*,ROW_NUMBER() OVER(Order by ord asc) as RowNum from( " +
                  "select ID,PointName,longitude,latitude," +
                  "case when longitude is NULL or LEN(longitude)=0 then 0 else 1 end as ord " +
                  "from Point where ID in(select distinct PointID from Point_User_View where TaskID in(" + inconditon + ") and UserID=@uid" + extsql + ")) as unions) as unions2 " +
                  "where unions2.RowNum between @start and @end";
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            ds = DBAccess.Query(sql, "Point_User_View", list);

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
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string PointName = DatabaseUtility.getStringValue(row, "PointName").Trim();
                        string longitude = DatabaseUtility.getStringValue(row, "longitude").Trim();
                        string latitude = DatabaseUtility.getStringValue(row, "latitude").Trim();
                        string Sign = "noreturn";

                        children += "{pointid:" + ID.ToString() + ",pointname:'" + PointName
                            + "',longitude:'" + longitude + "',latitude:'" + latitude + "',sign:'" + Sign + "'},";

                    }
                }
            }

            if (children.Length > 0)
            {
                children = children.Substring(0, children.Length - 1);
                ret = "{\"totalCount\":" + totalCount.ToString() + ",\"roots\":[" + children + "]}";
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