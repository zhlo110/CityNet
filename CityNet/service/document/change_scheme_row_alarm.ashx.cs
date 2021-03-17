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
    /// change_scheme_row_alarm 的摘要说明
    /// </summary>
    public class change_scheme_row_alarm : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string rowid = context.Request["rowid"];
            string editable = context.Request["editable"];

            int irowid = -1;
            if (!int.TryParse(rowid, out irowid))
            {
                irowid = -1;
            }
            int iedit = 0;
            if (editable.Trim().Equals("true"))
            {
                iedit = 1;
            }

            string sql = "update TableRowScheme set [alarmsign]=@ed where [ID] = @rowid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@ed", iedit));
            list.Add(new DictionaryEntry("@rowid", irowid));
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