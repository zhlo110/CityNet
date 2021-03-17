using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// delete_task_state 的摘要说明
    /// </summary>
    public class delete_task_state : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //删除功能
            string stateid = context.Request["stateid"];
            int istateid = -1;
            if (!int.TryParse(stateid, out istateid))
            {
                istateid = -1;
            }

            string sql = "delete from SubmitState where [ID]= @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", istateid));
            DBAccess.NoQuery(sql, list);

            sql = "update [Task] set [StateID] = -1 where [StateID]= @id";
            DBAccess.NoQuery(sql, list);

            context.Response.Write("{success:1,msg:'删除节点成功.'}");
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
    }
}