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
    /// check_taskstate_is_used 的摘要说明
    /// </summary>
    public class check_taskstate_is_used : Security
    {
        //判断state是否在使用
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string stateid = context.Request["stateid"];
            int istateid = -1;
            if (int.TryParse(stateid, out istateid))
            {  // 
                string sql = "select count([ID]) from [Task] where [StateID] = @sid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@sid", istateid));
                int nCount2 = DBAccess.QueryStatistic(sql, list);
                context.Response.Write("{success:1,msg:'查询成功',userednum:'" + nCount2.ToString() + "'}");
            }
            else
            {
                returnErrorInfo(context, "部门ID解析出错!");
            }
           // throw new NotImplementedException();
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}