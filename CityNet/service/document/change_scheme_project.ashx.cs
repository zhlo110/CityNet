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
    /// change_scheme_project 的摘要说明
    /// </summary>
    public class change_scheme_project : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string schid = context.Request["schid"];
            string editable = context.Request["editable"];

            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            int iedit = 0;
            if (editable.Trim().Equals("true"))
            {
                iedit = 1;
            }

            string sql = "update TableScheme set [hasprojection]=@ed where [ID] = @sid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@ed", iedit));
            list.Add(new DictionaryEntry("@sid", ischid));
            DBAccess.NoQuery(sql, list);
            context.Response.Write("{success:1,msg:'修改成功'}");
            // throw new NotImplementedException();
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