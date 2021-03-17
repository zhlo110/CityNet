using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// get_approve_comment 的摘要说明
    /// </summary>
    public class get_approve_comment : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string approveid = context.Request["approveid"];
            int iapproveid = -1;
            if (!int.TryParse(approveid, out iapproveid))
            {
                iapproveid = -1;
            }

            string sql = "select Description from Approve where ID =@aid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@aid", iapproveid));

            DataSet ds = DBAccess.Query(sql, "Approve", list);
            string str = "";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i = 0;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        str += DatabaseUtility.getStringValue(row, "Description").Trim() + "<br/>";
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