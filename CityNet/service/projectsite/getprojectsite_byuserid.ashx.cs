using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsite
{
    /// <summary>
    /// getprojectsite_byuserid 的摘要说明
    /// </summary>
    /// 获取第一项
    public class getprojectsite_byuserid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            string sdepartment = parameters[2].ToString();
            int departmentid = this.stringtoint(sdepartment, -1);
            int userid = LogUtility.GetUserID(username, password);
            string taskname = "";
            int taskid = -1;
            if (departmentid > 0 && userid > 0)
            {
                string departmentname = LogUtility.getDepartmentName(departmentid);
                string sql = "select ID,TaskName from Task where ID in(select TaskID from Department_Task where DepartmentID=@did and State=2) " +
                    " and ID in(select TaskID from Task_Visible where UserID = @uid)";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@did", departmentid));
                list.Add(new DictionaryEntry("@uid", userid));

                DataSet dataset = DBAccess.Query(sql, "Task", list);
                if (dataset != null)
                {
                    int nCount = dataset.Tables.Count;
                    if (nCount > 0)
                    {
                        DataTable dt = dataset.Tables[0];
                        nCount = dt.Rows.Count;
                        if (nCount > 0)
                        {
                            DataRow row = dt.Rows[0];
                            taskname = DatabaseUtility.getStringValue(row, "TaskName");
                            taskid = DatabaseUtility.getIntValue(row, "ID", -1);
                        }
                    }
                }
                context.Response.Write("{departmentid:" + departmentid.ToString() + ",departmentname:'"
                    +departmentname+"',taskid:"+taskid.ToString()+",taskname:'"+taskname+"'}");
            }
            else
            {
                this.returnErrorInfo(context,"部门错误");
            }


        }
        protected override string getErrorMessage()
        {
            return "用户密码错误";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}