using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// submit_approve_3 的摘要说明 
    /// </summary>
    /// 从状态3提交，返修状态
    public class submit_approve_3 : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }
            string username = (string)parameters[0];
            string password = (string)parameters[1];
            string description = context.Request["description"];
            description = System.Web.HttpUtility.HtmlDecode(description);
            int userid = LogUtility.GetUserID(username, password);
            //向Approve添加记录
            //从Approve表中找最新的记录
            string sql = "select SubmitID from Approve where ID = " +
                "(select Max(ID) from Approve where TaskID=@tid and AuditID=@uid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@uid", userid));
            int auditid = DBAccess.QueryStatistic(sql, list);
            if (auditid > 0)
            {
                sql = "insert into Approve (TaskID,Description,AuditID,SubmitTime,SubmitID) values(@tid,@des,@aid,@st,@sid)";
                list.Clear();
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@des", description));
                list.Add(new DictionaryEntry("@aid", auditid));
                list.Add(new DictionaryEntry("@st", DateTime.Now));
                list.Add(new DictionaryEntry("@sid", userid));
                DBAccess.NoQuery(sql, list);
            }
            //更改task的状态
            sql = "select ID from SubmitState where Priority=2";
            int substateid = DBAccess.QueryStatistic(sql, null);
            sql = "update Task set StateID=@sid where ID=@tid";
            list.Clear();
            list.Add(new DictionaryEntry("@sid", substateid));
            list.Add(new DictionaryEntry("@tid", taskid));
            DBAccess.NoQuery(sql, list);
            returnInfo(context, "提交成功");
        }


        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }

    }
}