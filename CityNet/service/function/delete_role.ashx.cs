using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.function
{
    /// <summary>
    /// delete_role 的摘要说明
    /// </summary>
    public class delete_role : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string roleid = context.Request["roleid"];
            int iroleid = int.Parse(roleid);
            string sql = "delete from [Group] where [ID]= @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", iroleid));
            DBAccess.NoQuery(sql, list);

            sql = "delete from Action_Group where [GroupID]= @id";
            DBAccess.NoQuery(sql, list);

            sql = "delete from User_Group where [GroupID]= @id";
            DBAccess.NoQuery(sql, list);

            context.Response.Write("{success:1,msg:'删除角色成功.'}");
        }
       
    }
}