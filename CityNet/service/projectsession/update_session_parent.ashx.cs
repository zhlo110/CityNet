using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// update_session_parent 的摘要说明
    /// </summary>
    public class update_session_parent : Security
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
            String sessionid = context.Request["sessionid"];
            String parentid = context.Request["parentid"];
            int isessionid = -1;
            int iparentid = -1;
            if (!int.TryParse(sessionid,out isessionid))
            {
                isessionid = -1;
            }

            if (!int.TryParse(parentid, out iparentid))
            {
                iparentid = -1;
            }

            IList list = new ArrayList();
            string sql = "update Project_Session set ParentID = @parentid where [ID]= @id";
            list.Add(new DictionaryEntry("@parentid", parentid));
            list.Add(new DictionaryEntry("@id", isessionid));

            DBAccess.NoQuery(sql, list);
            context.Response.Write("{success:1,msg:'移动节点成功.'}");
        }
    }
}