using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// get_pointlist_byuserid 的摘要说明
    /// </summary>
    public class get_pointlist_byuserid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userID = LogUtility.GetUserID(username,password);
            string taskid = context.Request["taskid"];
            string searchtext = context.Request["searchtext"];
            bool searchmode = false;
            string extsql = "";
            if (searchtext != null && searchtext.Length > 0)
            {
                extsql = " and PointName like  '%'+@seachtext+'%'";
                searchmode = true;
            }


            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }

            string sql = "select count(distinct PointID) " +
                "from Point_User_View where UserID=@uid and TaskID=@tid" + extsql;
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", userID));
            list.Add(new DictionaryEntry("@tid", itaskid));
            if (searchmode)
            {
                list.Add(new DictionaryEntry("@seachtext", searchtext.Trim()));
            }

            int totalCount = DBAccess.QueryStatistic(sql, list);
            string children = "";
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            int pagesize = limit;
            string ret = getErrorMessage();

            //分页
          // sql = "select * from( select ROW_NUMBER() OVER(Order by puv.ID desc) as RowNum," +
         //       "puv.ID,puv.PointName,puv.FirstTime,puv.L0,puv.h,puv.x,puv.y,puv.z,puv.longitude,puv.latitude,puv.description,puv.sharedes,puv.PointDes " +
           //     "from Point_User_View puv where puv.UserID=@uid) as unions " +
           //     "where unions.RowNum between @start and @end";
            sql = "select * from("+
                   "select ROW_NUMBER() OVER(Order by p.ID desc) as RowNum, p.ID,p.PointName,p.FirstMeasureTime,p.longitude,p.latitude " +
                   "from Point p where ID in "+
                   "(select distinct PointID from Point_User_View where UserID=@uid and TaskID=@tid" + extsql + ")) as unions " +
                   "where unions.RowNum between  @start and @end";
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));
         
            DataSet ds = DBAccess.Query(sql, "Point_User_View", list);
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
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);//点的ID
                        string pointname = DatabaseUtility.getStringValue(row, "PointName");
                        string firsttime = DatabaseUtility.getDatetimeValue(row, "FirstMeasureTime");
                        string longitude = DatabaseUtility.getStringValue(row, "longitude");
                        string latitude = DatabaseUtility.getStringValue(row, "latitude");

                   //     string x = DatabaseUtility.getStringValue(row, "x");
                      //  string y = DatabaseUtility.getStringValue(row, "y");
                     //   string z = DatabaseUtility.getStringValue(row, "z");
                    
                      //  string description = DatabaseUtility.getStringValue(row, "PointDes");
                      //  string sharedes = DatabaseUtility.getStringValue(row, "sharedes");
                     //   string L0 = DatabaseUtility.getStringValue(row, "L0");
                     //   string h = DatabaseUtility.getStringValue(row, "h");

                        children += "{id:" + ID.ToString() + ",taskid:" + itaskid.ToString() + ",pointname:'" + pointname + "',firsttime:'"
                            + firsttime + "',longitude:'" + longitude + "',latitude:'" + latitude + "'},";
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