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
    /// get_alarm_error 的摘要说明
    /// </summary>
    public class get_alarm_error : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string schid = context.Request["alarmid"];
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            string ret = "";

            string sql = "select ErrorMsg from [AlarmScheme] where ID = @sid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@sid", ischid));
            DataSet ds = DBAccess.Query(sql, "AlarmScheme", list);

            if (ds != null)
            {
                 if (ds.Tables.Count > 0)
                 {
                     DataTable dt = ds.Tables[0];
                     int nCount = dt.Rows.Count;
                     if (nCount > 0)
                     {
                         ret = DatabaseUtility.getStringValue(dt.Rows[0], "ErrorMsg");
                     }
                 }
            }
            context.Response.Write(ret);
        }
        protected override string getErrorMessage()
        {
            return "";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
        
    }
}