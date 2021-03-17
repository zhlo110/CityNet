using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// submit_supervisor_task 的摘要说明
    /// </summary>
    public class submit_supervisor_task : Security
    {


        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string auditid = context.Request["auditid"];//整改负责人
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
            string theme = context.Request["theme"].Trim();


            Hashtable copies = new Hashtable();//添加整改负责人
            //添加创建人
            copies[userid] = 1;
            if (iauditid > 0)//添加整改负责人
            {
                copies[iauditid] = 2;
            }
            if (copynodesid != null) //添加抄送人
            {
                string[] vec = copynodesid.Split(new char[] { ',' });
                if (vec.Length > 0)
                {
                    int value = -1;
                    int i;
                    for (i = 0; i < vec.Length; i++)
                    {
                        if (int.TryParse(vec[i].Trim(), out value))
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
            //查找stateid，整改阶段
            string sql = "select Max(ID) from SubmitState where Priority = 5";
            int stateid = DBAccess.QueryStatistic(sql, null);

            //创建一个Task
            sql = "insert into Task(TaskName,CreateID,StateID,FirstSubmitTime,step,SubmitTime,stepDescription,AuditID,isEnd,taskType) "+
                "values(@theme,@uid,@sid,@ftime,1,@sbtime,'未完成',@aid,0,2)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@theme", theme));
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@sid", stateid));
            list.Add(new DictionaryEntry("@ftime", DateTime.Now));
            list.Add(new DictionaryEntry("@sbtime", DateTime.Now));
            list.Add(new DictionaryEntry("@aid", iauditid));
            DBAccess.NoQuery(sql, list);
            sql = "select Max(ID) from Task";
            int taskid = DBAccess.QueryStatistic(sql, null);//查找TaskID
            //填充TaskVisible表用copies
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
            //在Approve表中增加步骤
            sql = "insert into Approve (TaskID,Description,AuditID,SubmitTime,SubmitID) values(@tid,@des,@aid,@st,@sid)";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", taskid));
            list.Add(new DictionaryEntry("@des", description));
            list.Add(new DictionaryEntry("@aid", iauditid));
            list.Add(new DictionaryEntry("@st", DateTime.Now));
            list.Add(new DictionaryEntry("@sid", userid));
            DBAccess.NoQuery(sql, list);
            sql = "select Max(ID) from Approve";
            int approveid = DBAccess.QueryStatistic(sql, null);
            //附件TempApproveDocument 表移植到ApproveDocument中

            sql = "select DocumentID from TempApproveDocument where TaskID=@tid and UserID=@uid";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", -1));
            list.Add(new DictionaryEntry("@uid", userid));

            DataSet ds = DBAccess.Query(sql, "TempApproveDocument", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        int DocumentID = DatabaseUtility.getIntValue(row, "DocumentID", -1);
                        sql = "select count(ID) from ApproveDocument where ApproveID = @aid and DocumentID = @did";
                        list.Clear();
                        list.Add(new DictionaryEntry("@aid", approveid));
                        list.Add(new DictionaryEntry("@did", DocumentID));
                        int length = DBAccess.QueryStatistic(sql, list);
                        if (DocumentID > 0 && length == 0)
                        {
                            sql = "insert into ApproveDocument(ApproveID,DocumentID) values(@aid,@did)";
                            DBAccess.NoQuery(sql, list);
                        }

                        //更改Document中的 TaskID
                        sql = "update Document set TaskID=@tid where ID = @did";
                        list.Clear();
                        list.Add(new DictionaryEntry("@tid", taskid));
                        list.Add(new DictionaryEntry("@did", DocumentID));
                        DBAccess.NoQuery(sql, list);
                    }
                }
            }
            //删除TempApproveDocument
            sql = "delete from TempApproveDocument where TaskID=@tid and UserID=@uid";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", -1));
            list.Add(new DictionaryEntry("@uid", userid));
            DBAccess.NoQuery(sql, list);

            returnInfo(context, "提交成功");
        }
    }
}