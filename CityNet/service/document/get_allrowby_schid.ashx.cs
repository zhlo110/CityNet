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
    /// get_allrowby_schid 的摘要说明
    /// </summary>
    /// 


    public class get_allrowby_schid : Security
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
            string sql = "select count(ID) from [TableRowScheme] where TableSchemeID = @tsid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tsid", ischid));
            int totalCount = DBAccess.QueryStatistic(sql, list);
            string children = "";
            int pagesize = limit;
            //分页
            sql =
                "select * from(" +
                "select ROW_NUMBER() OVER(Order by ts.No asc) as RowNum, " +
                "ts.*  from [TableRowScheme] ts " +
                "where ts.TableSchemeID = @tsid) as unions " +
                "where unions.RowNum between @start and @end";

            list.Clear();
            list.Add(new DictionaryEntry("@tsid", ischid));
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "TableRowScheme", list);

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    int ivalid = 1;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string rowname = DatabaseUtility.getStringValue(row, "Name");
                        string description = DatabaseUtility.getStringValue(row, "Description");
                        string colrel = DatabaseUtility.getStringValue(row, "ColumnRel");
                        int schemeid = DatabaseUtility.getIntValue(row, "TableSchemeID", -1);
                        int no = DatabaseUtility.getIntValue(row, "No", 0);
                        string type = DatabaseUtility.getStringValue(row, "type");
                        int alarmsign = DatabaseUtility.getIntValue(row, "alarmsign", 0);

                        string valid = "1";
                        if(no != i+1)
                        {
                            valid = "0";
                            ivalid = 0;
                        }

                        children += "{\"rowid\":" + ID.ToString() + ",\"valid\":" + valid + ",\"type\":'" + type + "',\"no\":" + no.ToString()
                            +",\"rowname\":\"" + rowname + "\",\"description\":\"" +
                           description + "\",\"colrel\": \"" + colrel + "\",\"alarmcheck\":" + alarmsign.ToString() + "},";
                    }
                    if (nCount == 0) //有数据
                    {
                        ivalid = 0;
                    }
                    sql = "update TableScheme set valid = @v where [ID] = @id";
                    list.Clear();
                    list.Add(new DictionaryEntry("@v", ivalid));
                    list.Add(new DictionaryEntry("@id", ischid));
                    DBAccess.NoQuery(sql, list);

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