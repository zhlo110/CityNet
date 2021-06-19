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
    /// get_alarmby_schid 的摘要说明
    /// </summary>
    public class get_alarmby_schid : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            string schid = context.Request["schid"];
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            string sql = "select count(ID) from [AlarmScheme] where SchemeID = @tsid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tsid", ischid));
            int totalCount = DBAccess.QueryStatistic(sql, list);
            string children = "";
            int pagesize = limit;
            //分页
            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by ts.ID desc) as RowNum, " +
                "ts.*  from [AlarmScheme] ts " +
                "where ts.SchemeID = @tsid) as unions " +
                "where unions.RowNum between @start and @end";

            list.Clear();
            list.Add(new DictionaryEntry("@tsid", ischid));
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "AlarmScheme", list);

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
                        string name = DatabaseUtility.getStringValue(row, "Name");
                        string rule = DatabaseUtility.getStringValue(row, "Rules");
                        int schemeid = DatabaseUtility.getIntValue(row, "SchemeID", -1);
                        string color = DatabaseUtility.getStringValue(row, "color");
                        string unit = DatabaseUtility.getStringValue(row, "unit");
                        string description = DatabaseUtility.getStringValue(row, "Description");
                        string errorstr = DatabaseUtility.getStringValue(row, "ErrorMsg");
                        if (errorstr == null)
                        {
                            errorstr = "";
                        }
                        errorstr = errorstr.Trim();
                        string valid = "0";
                        if (errorstr.Length == 0)
                        {
                            valid = "1";
                        }
                        else
                        {
                            valid = "0";
                        }
                        
                        int alarmvalue = DatabaseUtility.getIntValue(row, "AlarmLevel", 0);
                        children += "{\"alarmid\":" + ID.ToString() + ",\"alarmname\":'" + name + "',\"rule\":'" 
                            + System.Web.HttpUtility.HtmlEncode(rule)
                            + "',\"color\":'" + color + "',\"description\":'" +
                           description + "',\"unit\": '" + unit + "',alarmlevel:" + alarmvalue + ",valid:" + valid 
                           + ",qtip:''},";
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