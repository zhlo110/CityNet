using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;
//内部访问（前后端不分离）
namespace CityNet.service.user
{
    /// <summary>
    /// log 的摘要说明
    /// </summary>
    public class log : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            String username = context.Request["username"];
            String password = context.Request["password"];
            String sdepartment = context.Request["department"];

            IList list = new ArrayList();
            list.Add(username);
            list.Add(password);
            list.Add(sdepartment);
            bool success = LogUtility.Login(username, password);

            DataTable dt = Security.getUserInformation(list);
            if (dt != null)
            {
                DataRow row = dt.Rows[0];
                string name = row["RealName"].ToString();
                string department = row["DepartmentName"].ToString();
                string id = row["ID"].ToString();
                string groupName = row["RealGroupName"].ToString();
                string group = row["GroupName"].ToString();
                string departmentid = row["DepartmentID"].ToString();

                //登录次数加1
                string addsql = "UPDATE [User] set LogTime = LogTime+1 where ID=@id";
                list.Clear();
                list.Add(new DictionaryEntry("@id", id));
                DBAccess.NoQuery(addsql, list);

                string urlparam = "username=" + username + "&password=" + password + "&department=" + departmentid;
                string enc = EncryptHelper.AESEncrypt(urlparam);

                string url = "../Home/Index?params=" + enc;
                string res = "{success:1 ,msg:'成功',id:" + id
                    + ",realname:'" + name + "',department:'" + department + "',groupname:'" + groupName
                    + "',username:'" + username + "',password:'" + password + "',managerurl:'" + url + "'}";
                context.Response.Write(res);
            }
            else
            {
                if (success)
                {
                    context.Response.Write("{success:0,msg:'账号已经注册成功，但还未通过管理员审核。'}");
                }
                else
                {
                    context.Response.Write("{success:0,msg:'用户名和密码错误！'}");
                }

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