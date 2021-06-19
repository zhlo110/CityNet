using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// delete_table_scheme_alarm 的摘要说明
    /// </summary>
    public class delete_table_scheme_alarm : Security
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
            string schid = context.Request["schid"];
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }

            string alarmid = context.Request["alarmid"];
            int ialarmid = -1;
            if (!int.TryParse(alarmid, out ialarmid))
            {
                ialarmid = -1;
            }
            //在AlarmPoint中删除该规则
            string sql = "delete from AlarmPoint where [ID]= @id and Eluminated = 0 and AlarmSchemeID = @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", ialarmid));
            DBAccess.NoQuery(sql, list);

            sql = "delete from AlarmScheme where [ID]= @id and SchemeID = @tsid";
            list.Add(new DictionaryEntry("@tsid", ischid));
            DBAccess.NoQuery(sql, list);
            context.Response.Write("{success:1,msg:'删除节点成功.'}");
        }

    }
}