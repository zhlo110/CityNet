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
    /// endtask 的摘要说明
    /// </summary>
    public class endtask : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }

            string sql = "update Task set isEnd = 1,stepDescription=@des where ID=@tid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@des", "已终止"));
            list.Add(new DictionaryEntry("@tid", taskid));
            DBAccess.NoQuery(sql,list);

            returnInfo(context,"任务终止完成！");

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