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
    /// submit_task 的摘要说明
    /// </summary>
    public class submit_task : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            // throw new NotImplementedException();
          
            string staskid = context.Request["taskid"];
            int taskid = -1;
            if (!int.TryParse(staskid, out taskid))
            {
                taskid = -1;
            }
            string auditid = context.Request["auditid"];
            int iauditid = -1;
            if (!int.TryParse(auditid, out iauditid))
            {
                iauditid = -1;
            }

            string username = (string)parameters[0];
            string password = (string)parameters[1];
            int userid = LogUtility.GetUserID(username, password);

            string description = context.Request["description"];
            string copynodesid = context.Request["copynodesid"];

            Hashtable copies = new Hashtable();
            if (iauditid > 0)
            {
                copies[iauditid] = 2;
               // copies.Add(iauditid);
            }
            if (copynodesid != null) //抄送人
            {
                string[] vec = copynodesid.Split(new char[]{','});
                if(vec.Length > 0)
                {
                    int value = -1;
                    int i;
                    for (i = 0; i < vec.Length; i++)
                    {
                        if (int.TryParse(vec[i].Trim(),out value))
                        {
                            if (value > 0)
                            {
                                if (!copies.ContainsKey(value))
                                {
                                    copies[value] = 3;
                                }
                              //  copies.Add(value);
                            }
                        }
                    }
                }
            }
            //审核逻辑，无法对数据进行修改，但可以写评语，可以通知用户返修
            //查找下一个审核步骤stateID
            string sql = "select top 1 ID from SubmitState where [Priority]>" +
                         "(select [Priority] from SubmitState where ID = " +
                         "(select StateID from Task where ID=@tid))";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tid", taskid));
            int schid = DBAccess.QueryStatistic(sql, list);
            description = System.Web.HttpUtility.HtmlDecode(description);

            if (schid > 0)
            {
                //更改task表
                sql = "update Task set AuditID=@aid,SubmitTime=@st,StateID=@sid where ID=@tid";
                list.Add(new DictionaryEntry("@aid", auditid));
                list.Add(new DictionaryEntry("@st", DateTime.Now));
                list.Add(new DictionaryEntry("@sid", schid));
                DBAccess.NoQuery(sql, list);
                //在Approve表中增加步骤
                sql = "insert into Approve (TaskID,Description,AuditID,SubmitTime,SubmitID) values(@tid,@des,@aid,@st,@sid)";
                list.Clear();
                list.Add(new DictionaryEntry("@tid", taskid));
                list.Add(new DictionaryEntry("@des", description));
                list.Add(new DictionaryEntry("@aid", auditid));
                list.Add(new DictionaryEntry("@st", DateTime.Now));
                list.Add(new DictionaryEntry("@sid", userid));
                DBAccess.NoQuery(sql, list);


                //抄送人逻辑，仅仅对数据有查看的权限，不能修改不能返修
                 
                 foreach (int copyuserid in copies.Keys)
                 {
                     sql = "IF NOT EXISTS(select ID from Task_Visible where TaskID=@tid and UserID=@uid) " +
                           "insert into Task_Visible(TaskID,UserID,UserType) values(@tid,@uid,@ut)";
                     list.Clear();
                     list.Add(new DictionaryEntry("@tid", taskid));
                     list.Add(new DictionaryEntry("@uid", copyuserid));
                     list.Add(new DictionaryEntry("@ut", copies[copyuserid]));
                     DBAccess.NoQuery(sql, list);
                 }

                returnInfo(context, "提交成功");
            }
            else
            {
                returnErrorInfo(context,"获取下一步审核步骤出错，请与管理员联系");
            }


           

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