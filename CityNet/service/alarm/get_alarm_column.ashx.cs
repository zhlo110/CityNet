using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.alarm
{
    /// <summary>
    /// get_alarm_column 的摘要说明
    /// </summary>
    public class get_alarm_column : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string schid = context.Request["schid"];
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            string sql = "select ID,Name,ColumnRel from TableRowScheme where TableSchemeID =@shid and alarmsign = 1";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@shid", ischid));
            DataSet ds = DBAccess.Query(sql, "TableRowScheme", list);
            string json = "";

     
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
                        string rel = DatabaseUtility.getStringValue(row, "ColumnRel");
                        json += "{alarmid:" + ID.ToString() + ",alarmname:'" + schemename + "',alarmcol:'" + rel + "'},";
                    }
                }
            }
            if (json.Length > 0)
            {
                json = json.Substring(0, json.Length - 1);
            }

            context.Response.Write("[" + json + "]");
        }
        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }

    }
}