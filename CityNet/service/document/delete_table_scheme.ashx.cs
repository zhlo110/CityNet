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
    /// delete_table_scheme 的摘要说明
    /// </summary>
    public class delete_table_scheme : Security
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

            string sql = "DECLARE @success char(50) " +
                         "EXEC dbo.delete_TableScheme @schemeid = @id, @success = @success OUTPUT " +
                         "SELECT	@success as 'returnvalue'";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", ischid));
            string result = DBAccess.QueryString(sql, list);
            context.Response.Write("{success:1,msg:'" + result + "'}");
        }
    }
}