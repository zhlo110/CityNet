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
    /// get_points_by_task_schemeid 的摘要说明
    /// </summary>
    public class get_points_by_task_schemeid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string staskid = context.Request["taskid"];
            int taskid = this.stringtoint(staskid, -1);

            string sschemeid = context.Request["schemeid"];
            int schemeid = this.stringtoint(sschemeid, -1);

            //分页
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            string sql =
            "select count(ID) " +
            "from Point P right join (select MAX(MeasureTime) as newtime,COUNT(ID) as Num,PointID from Point_User_View " +
            "where TaskID=@tid and SchemeID=@sid and UserID=@uid  group by PointID) " +
            "as U on U.PointID = P.ID";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@sid", schemeid));
            list.Add(new DictionaryEntry("@uid", userid));
            int totalCount = DBAccess.QueryStatistic(sql, list);

             sql =
                "select * from " +
                "(select ROW_NUMBER() OVER(Order by P.ID asc) as RowNum, P.ID,P.longitude,P.latitude,P.PointName,P.FirstMeasureTime,U.newtime,U.Num " +
                "from Point P right join (select MAX(MeasureTime) as newtime,COUNT(ID) as Num,PointID from Point_User_View " +
                "where TaskID=@tid and SchemeID=@sid and UserID=@uid  group by PointID) " +
                "as U on U.PointID = P.ID)  as unions2 where unions2.RowNum between  @start and @end ";
           
            
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "Point_User_View", list);

            string children = "";
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
                        int pointid = DatabaseUtility.getIntValue(row, "ID", -1);
                        string longitude = DatabaseUtility.getStringValue(row, "longitude");
                        string latitude = DatabaseUtility.getStringValue(row, "latitude");
                        string count = DatabaseUtility.getStringValue(row, "Num");
                        string pointName = DatabaseUtility.getStringValue(row, "PointName");
                        string firsttime = DatabaseUtility.getDatetimeValue(row, "FirstMeasureTime","yyyy-MM-dd HH:mm:ss");
                        string endtime = DatabaseUtility.getDatetimeValue(row, "newtime", "yyyy-MM-dd HH:mm:ss");

                        children += "{pointid:" + pointid.ToString()
                            + ",longitude:'" + longitude + "',latitude:'" + latitude
                            + "',number:" + count.ToString() + ",pointname:'" + pointName
                            + "',first:'" + firsttime + "',end:'"+endtime+"'},";

                    }
                }
            }
            string ret = getErrorMessage();
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