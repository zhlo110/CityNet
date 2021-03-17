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
    /// get_approvelist_bytaskid 的摘要说明
    /// </summary>
    public class get_approvelist_bytaskid : Security
    {
        //       { name: 'approveid', type: 'int' },
        //   { name: 'date', type: 'string' },
        //   { name: 'name', type: 'string' },
        //   { name: 'department', type: 'string' }
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = this.getErrorMessage();
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }
            string sql = "select ID,SubmitTime,creator,auditor from Approve_User_View where TaskID=@tid order by SubmitTime desc";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", taskid));
            DataSet ds = DBAccess.Query(sql, "Task_view", list);
            string str = "";
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
                        int ID = DatabaseUtility.getIntValue(row,"ID",-1);
                        string creator = DatabaseUtility.getStringValue(row, "creator");
                        string datetime = DatabaseUtility.getDatetimeValue(row, "SubmitTime");
                        string auditor = DatabaseUtility.getStringValue(row, "auditor");
                        str += "{approveid:" + ID.ToString() + ",date:'" + datetime + "',creator:'" + creator + "',auditor:'" + auditor + "'},";
                    }
                }
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
                ret = "[" + str + "]";
            }
            context.Response.Write(ret);

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