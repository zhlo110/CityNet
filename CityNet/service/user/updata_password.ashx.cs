using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.user
{
    /// <summary>
    /// updata_password 的摘要说明
    /// </summary>
    public class updata_password : Security
    {

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            String newpassword = context.Request["newpassword1"];
            string sql = "update [User] set PassWord=@pw where UserName = @un";
            IList list = new ArrayList();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            string logpassword = context.Request["oldpassword"];
            if (logpassword == password)
            {
                list.Add(new DictionaryEntry("@pw", newpassword));
                list.Add(new DictionaryEntry("@un", username));
                DBAccess.NoQuery(sql, list);
                context.Response.Write("{success:1,msg:'修改密码成功,请重新登录！'}");
            }
            else
            {
                context.Response.Write(getErrorMessage());
                context.Response.StatusCode = getErrorCode();
            }
        }
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
    }
}