using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.basepoint
{
    /// <summary>
    /// deletebasepoint 的摘要说明
    /// </summary>
    public class deletebasepoint : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string ids = context.Request["ids"].Trim();
            if (ids.Length > 0)
            {
                ids = ids.Substring(0, ids.Length - 1);
                string sql = "delete from BasePoint where ID in("+ids+")";
                DBAccess.NoQuery(sql, null);
                returnInfo(context, "删除成功");
            }
            else
            {
                returnInfo(context, "请选中要删除的列");
            }
          //  ids
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