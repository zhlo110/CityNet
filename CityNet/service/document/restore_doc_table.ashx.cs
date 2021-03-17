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
    /// restore_doc_table 的摘要说明
    /// </summary>
    public class restore_doc_table : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string docid = context.Request["docid"];
            int idocid = 0;
            if (!int.TryParse(docid, out idocid))
            {
                idocid = 0;
            }

            string sql = "update DocumentTable set [deletestate] = 0 where DocumentID =@did";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@did", idocid));
            DBAccess.NoQuery(sql, list);
            returnInfo(context,"恢复成功");
        }
    }
}