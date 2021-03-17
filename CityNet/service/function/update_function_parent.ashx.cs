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
    /// update_function_parent 的摘要说明
    /// </summary>
    public class update_function_parent : Security
    {
        
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }


        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            String srrid = context.Request["funid"];
            String srrparentid = context.Request["parentid"];
            char[] splitc = new char[] { '_' };
            string[] vec2 = srrid.Split(splitc);
            string nodeid = vec2[2];
            vec2 = srrparentid.Split(splitc);
            string parentid = vec2[2];

            IList list = new ArrayList();
            string sql = "update ActionClass set ParentID = @parentid where [ID]= @id";
            list.Add(new DictionaryEntry("@parentid", parentid));
            list.Add(new DictionaryEntry("@id", nodeid));

            DBAccess.NoQuery(sql, list);
            context.Response.Write("{success:1,msg:'移动节点成功.'}");
        }
    }
}