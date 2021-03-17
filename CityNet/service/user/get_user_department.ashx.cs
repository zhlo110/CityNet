using CityNet.Controllers;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.user
{
    /// <summary>
    /// get_user_department 的摘要说明
    /// </summary>
    public class get_user_department : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            String username = context.Request["username"];
            String password = context.Request["password"];
            bool success = LogUtility.Login(username, password);
            if (success)
            {
                int userID = LogUtility.GetUserID(username, password);
                string sql = "select d.* from Department d,User_Department ud where d.ID = ud.DepartmentID and ud.UserID = @uid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@uid", userID));
                DataSet dataset = DBAccess.Query(sql, "Department", list);

                if (dataset != null)
                {
                    int nCount = dataset.Tables.Count;
                    if (nCount > 0)
                    {
                        DataTable table = dataset.Tables[0];
                        nCount = table.Rows.Count;
                        if(nCount > 0)
                        {
                            int i;
                            string ret = "";
                            for (i = 0; i < nCount; i++ )
                            {
                                DataRow row = table.Rows[i];
                                string departmentid = row["ID"].ToString();
                                string departmentname = row["DepartmentName"].ToString();
                                ret += "{departmentID:" + departmentid + ",departmentName:\"" + departmentname + "\"},";
                            }
                            if (ret.Length > 0)
                            {
                                ret = ret.Substring(0,ret.Length-1);
                            }
                            context.Response.Write("[" + ret + "]");
                        }
                        else
                        {
                            context.Response.Write("{success:0,msg:'该用户没有部门，请联系管理员'}");
                            context.Response.StatusCode = 500;
                        }
                    }
                    else
                    {
                        context.Response.Write("{success:0,msg:'该用户没有部门，请联系管理员'}");
                        context.Response.StatusCode = 500;
                    }
                }
                else
                {
                    context.Response.Write("{success:0,msg:'获取部门错误，请联系管理员！'}");
                    context.Response.StatusCode = 500;
                }
            }
            else
            {
                context.Response.Write("{success:0,msg:'用户名和密码错误！'}");
                context.Response.StatusCode = 500;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}