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
    /// get_html_bypointid 的摘要说明
    /// </summary>
    public class get_html_bypointid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string pointid = context.Request["pointid"];
            string sql = "select Sign from Point where ID in(" + pointid + ")";
            DataSet ds = DBAccess.Query(sql, "Point", null);
            string str = "";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i = 0;
                    for (i = 0; i < nCount; i++ )
                    {
                        DataRow row = dt.Rows[i];
                        str += DatabaseUtility.getStringValue(row, "Sign").Trim()+"<br/>";
                    }
                }
            }
            context.Response.Write(str);
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