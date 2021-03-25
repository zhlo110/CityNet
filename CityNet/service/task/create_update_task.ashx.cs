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
    /// create_update_task 的摘要说明
    /// </summary>
    public class create_update_task : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }
            IList list = new ArrayList();
            string sql = "";
            string taskname = context.Request["task_name"];
            taskname = taskname.Trim();
            if (taskid == -1) //新建任务
            {
                //新建一个任务
               
                string userName = parameters[0].ToString();
                string password = parameters[1].ToString();
                int userid = LogUtility.GetUserID(userName, password);
                sql = "select MIN(ID) from SubmitState";
                int stateid = DBAccess.QueryStatistic(sql, null);

                sql = "insert into Task(TaskName,CreateID,StateID,FirstSubmitTime,step,stepDescription,isEnd,taskType) " +
                    "values(@tn,@cid,@sid,@time,@st,@stdes,@end,@ttp)";

                list.Add(new DictionaryEntry("@tn", taskname));
                list.Add(new DictionaryEntry("@cid", userid));
                list.Add(new DictionaryEntry("@sid", stateid));
                list.Add(new DictionaryEntry("@time", DateTime.Now));
                list.Add(new DictionaryEntry("@st", 1));
                list.Add(new DictionaryEntry("@stdes", "未完成"));
                list.Add(new DictionaryEntry("@end", 0));
                list.Add(new DictionaryEntry("@ttp", 1));
                DBAccess.NoQuery(sql, list);

                //得到taskID
                sql = "select Max(ID) from Task";
                taskid = DBAccess.QueryStatistic(sql, null);
                //查询插入，不得插入重复数据

                sql = "IF NOT EXISTS(select ID from Task_Visible where TaskID=@tid and UserID=@uid) " +
                           "insert into Task_Visible(TaskID,UserID,UserType) values(@tid,@uid,@type)";
             //   sql = "insert Task_Visible(TaskID,UserID) values(@tid,@uid)";
                list.Clear();
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@uid", userid));
                list.Add(new DictionaryEntry("@type", 1)); //type为创建者
                DBAccess.NoQuery(sql, list);
            }
            else //更新任务
            {
                sql = "update Task set TaskName=@tn where [ID]=@taskid";
                list.Add(new DictionaryEntry("@tn", taskname));
                list.Add(new DictionaryEntry("@taskid", taskid));
                DBAccess.NoQuery(sql, list);
            }
            

            context.Response.Write("{success:1,taskid:" + taskid.ToString() + ",msg:'新建成功！'}");
            //throw new NotImplementedException();
            // this.returnErrorInfo(context, "数据提交错误");
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}