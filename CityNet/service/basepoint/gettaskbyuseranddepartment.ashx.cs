using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.basepoint
{
    /// <summary>
    /// gettaskbyuseranddepartment 的摘要说明
    /// </summary>
    public class gettaskbyuseranddepartment : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            string deparmentid = context.Request["departmentid"];
            if (deparmentid == null) 
                deparmentid = "";
            int idepartmentid = this.stringtoint(deparmentid, -1);
            IList list = new ArrayList();
            int userid = LogUtility.GetUserID(username, password);
            string sql = "select * from " +
                        "(select Base.*,ss.Priority from( " +
                        "select ID,TaskName,StateID from Task where ID in( " +
                        "select TaskID from Task_Visible where UserID=@uid) "
                        + "and ID in (select TaskID from Department_Task where DepartmentID = @did and State=2)" 
                        + " ) Base left join SubmitState ss on Base.StateID = ss.ID) " +
                        "unions where unions.Priority = 4";
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@did", idepartmentid));
            
            string children = "";
            DataSet ds = DBAccess.Query(sql, "Task_view", list);
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
                        string Name = DatabaseUtility.getStringValue(row, "TaskName");
                        children += "{taskid:\"" + ID.ToString() + "\",taskname:\"" + Name + "\"},";

                    }
                }
            }
            if (children.Length > 0)
            {
                children = children.Substring(0, children.Length - 1);
            }
            else
            {
                children = "{}";
            }

            ret = "{totalCount:1,data:[" + children + "]}";
            context.Response.Write(ret);

        }


        protected override string getErrorMessage()
        {
            return "{totalCount:0,data:[]}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}