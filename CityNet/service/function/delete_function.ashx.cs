using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.function
{
    /// <summary>
    /// delete_function 的摘要说明
    /// </summary>
    public class delete_function : Security
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
            //删除功能
            string funid = context.Request["functionid"];
            int ifunid = int.Parse(funid);
            string sql = "delete from Action where [ID]= @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", ifunid));
            DBAccess.NoQuery(sql, list);

            sql = "delete from Action_Group where [ActionID]= @id";
            DBAccess.NoQuery(sql, list);

            context.Response.Write("{success:1,msg:'删除节点成功.'}");
        }
        
    }
}