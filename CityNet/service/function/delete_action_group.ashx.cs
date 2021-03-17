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
    /// delete_action_group 的摘要说明
    /// </summary>
    public class delete_action_group : Security
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
            //从权限组中删除功能
            string groupid = context.Request["groupid"];
            string actoinid = context.Request["actoinid"];

            if (groupid != null && actoinid != null)
            {
                int igroupid = 0;
                int iactionid = 0;
                bool suc1 = int.TryParse(groupid, out igroupid);
                bool suc2 = int.TryParse(actoinid, out iactionid);
                if (suc1 && suc2)
                {
                    string sql = "delete from Action_Group where [ActionID] = @aid and [GroupID] =@gid";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@aid", iactionid));
                    list.Add(new DictionaryEntry("@gid", igroupid));
                    DBAccess.NoQuery(sql, list);
                }
            }
            else
            {
                context.Response.Write("{success:0,msg:'表单格式错误'}");
                context.Response.StatusCode = getErrorCode();
            }
        }

       
    }
}