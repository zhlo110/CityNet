using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsite
{
    /// <summary>
    /// copy_tablescheme 的摘要说明
    /// </summary>
    public class copy_tablescheme : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string sdestid = context.Request["destid"];
            string soriid = context.Request["taskid"];
            int destid = this.stringtoint(sdestid, -1);
            int oriid = this.stringtoint(soriid,-1);
            if (destid > 0 && oriid > 0)
            {
                IList list = new ArrayList();
                string sql = " EXEC dbo.cope_tablescheme_to_task @oriid = @oid, @destid = @did";
                list.Add(new DictionaryEntry("@oid", oriid));
                list.Add(new DictionaryEntry("@did", destid));
                DBAccess.NoQuery(sql, list);
                returnInfo(context, "拷贝成功");
            }
            else
            {
                returnInfo(context,"读取ID出错");
            }
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override string getErrorMessage()
        {
            return "用户名密码错误，请重新登陆";
        }
    }
}