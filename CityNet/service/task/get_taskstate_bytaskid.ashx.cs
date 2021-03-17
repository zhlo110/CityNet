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
    /// get_taskstate_bytaskid 的摘要说明
    /// </summary>
    public class get_taskstate_bytaskid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }
            string sql = "select StateID,statename,priority,editable from Task_View where ID=@tid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", taskid));
            DataSet ds = DBAccess.Query(sql, "Task_view", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    if(nCount>0)
                    {
                        DataRow row = dt.Rows[0];
                        int stateid = DatabaseUtility.getIntValue(row, "StateID", -1);
                        string statename = DatabaseUtility.getStringValue(row, "statename");
                        int priority = DatabaseUtility.getIntValue(row, "priority", -1);
                        int editable = DatabaseUtility.getIntValue(row, "editable", -1);
                        ret = "{stateid:" + stateid.ToString() + ",statename:'"
                            + statename + "',priority:" + priority.ToString() + ",editable:" + editable.ToString() + "}";
                    }
                }
            }
            context.Response.Write(ret);

        }
        protected override string getErrorMessage()
        {
            return "{stateid:-1,statename:'',priority:0,editable:0}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}