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
    /// get_alltablescheme 的摘要说明
    /// </summary>
    public class get_alltablescheme : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            string taskid = context.Request["taskid"];
            string condition = "";//"and ts.TaskID is NULL";
            //"and NOT EXISTS(select TableSchemeID from Task_TableScheme where TaskID = " + itaskid.ToString() + ")";
            if (taskid == null)
            {
                taskid = "";
            }
            taskid = taskid.Trim();
            int itaskid = -1;
            if (int.TryParse(taskid, out itaskid))
            {
                // condition = "and ts.TaskID ="+itaskid.ToString();
                condition = "and ts.ID in (select TableSchemeID from Task_TableScheme where TaskID = " + itaskid.ToString() + ")";
            }
            else
            {
                condition = "and NOT EXISTS(select ID from Task_TableScheme where TableSchemeID = ts.ID)";
            }

            string sql = "select count(ID) from [TableScheme]";
            int totalCount = DBAccess.QueryStatistic(sql, null);
            string children = "";
            int pagesize = limit;
            //分页
            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by ts.ID desc) as RowNum, " +
                "ts.*,u.ID as uerID ,u.RealName from [TableScheme] ts ,[User] u " +
                "where ts.creatorID = u.ID " + condition + ") as unions " +
                "where unions.RowNum between @start and @end";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "TableScheme", list);
  
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
                        string schemename = DatabaseUtility.getStringValue(row, "Name");
                        string description = DatabaseUtility.getStringValue(row, "Description");
                        int Priority = DatabaseUtility.getIntValue(row, "Priority", 0);
                        string createTime = DatabaseUtility.getDatetimeValue(row, "createTime");
                        int creatorID = DatabaseUtility.getIntValue(row, "creatorID", 0);
                        string creator = DatabaseUtility.getStringValue(row, "RealName");
                        string color = DatabaseUtility.getStringValue(row, "color").Trim();
                        int valid = DatabaseUtility.getIntValue(row, "valid", 0);
                        int hasprojection = DatabaseUtility.getIntValue(row, "hasprojection", 0);
                        string schecked = "true";
                        if (Priority <= 0)
                        {
                            schecked = "false";
                        }
                        //hasprojection

                        children += "{\"schid\":" + ID.ToString() + ",\"valid\":" + valid.ToString() + ",\"priority\":" + schecked
                            + "," + "\"schname\":\"" + schemename + "\",\"creator\":\"" + creator + "\",\"description\":\"" +
                           description + "\",\"createdata\": \"" + createTime + "\",\"hasprojection\":" 
                           + hasprojection.ToString() + ",\"color\":\""+color+"\"},";
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