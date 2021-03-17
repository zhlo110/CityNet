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
    /// check_functionhaschild 的摘要说明
    /// </summary>
    public class check_functionhaschild : Security
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
            //判断功能组中有没有功能
            String function_id = context.Request["funid"];
            char[] splitc = new char[] { '_' };
            string[] vec2 = function_id.Split(splitc);
            string sql = "select count(*) from [Action] where ActionGroupID=@groupid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@groupid", vec2[2]));
            int size = DBAccess.QueryStatistic(sql, list);
            if (size > 0)
            {
                context.Response.Write("{success:1,haschildren:true,msg:'节点有孩子.'}");
            }
            else
            {
                context.Response.Write("{success:1,haschildren:false,msg:'节点没有孩子.'}");
            }
        }
    }
}