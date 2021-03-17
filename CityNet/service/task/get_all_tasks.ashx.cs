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
    /// get_all_tasks 的摘要说明
    /// </summary>
    public class get_all_tasks : Security
    {
        //根据用户的权限获取所有的task
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //分页显示权限组
            string ret = getErrorMessage();
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            string mode = context.Request["mode"];
            string tasktype = context.Request["tasktype"];
            int itasktype = 1;
            if (tasktype != null)
            {
                tasktype = tasktype.Trim();
                if (!int.TryParse(tasktype,out itasktype))
                {
                    itasktype = 1;
                }
            }

            int imode = 0;//0,提交模式 1审批模式
            if (!int.TryParse(mode, out imode))
            {
                imode = 0;
            }
            string buttonName = "";
            if (itasktype == 1)
            {
                if (imode == 0)
                {
                    buttonName = "查看详请";
                }
                else
                {
                    buttonName = "成果审查";
                }
            }
            else
            {
                buttonName = "立即处理";
            }

            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);

            string sql = "";
           // int totalCount = 100;
            string children = "";
            int pagesize = limit;
            IList list = new ArrayList();

            //用户是否有查看所有task权限
            int groupid = LogUtility.GetGroupIDByUserID(userid);
            bool allview = false;
            if (groupid > 0)
            {
                sql = "select count(agv.ID) from Action_Group_View agv,ActionClass ac " +
                    "where agv.ActionGroupID = ac.ID and agv.ActionUrl = @au and agv.GroupID= @gid and ac.ActionType=@at";
                list.Add(new DictionaryEntry("@au", "all_taskinfo"));
                list.Add(new DictionaryEntry("@gid", groupid));
                list.Add(new DictionaryEntry("@at", 5));
                int iExist = DBAccess.QueryStatistic(sql, list);
                if (iExist > 0) //可查看所有任务
                {
                    sql = "delete from Task_Visible where UserID = @uid and UserType=3 and TaskID in (select ID from Task where taskType=@tty)";
                    list.Clear();
                    list.Add(new DictionaryEntry("@uid", userid));
                    list.Add(new DictionaryEntry("@tty", itasktype));
                    DBAccess.NoQuery(sql, list);


                    //查找不属于该用户的TaskID
                    sql = "select [ID] from Task where taskType = @tty";
                    list.Clear();
                    list.Add(new DictionaryEntry("@tty", itasktype));


                    DataSet dtvs = DBAccess.Query(sql, "Task", list);
                    if (dtvs != null)
                    {
                        if (dtvs.Tables.Count > 0)
                        {
                            DataTable dt = dtvs.Tables[0];
                            int nCount = dt.Rows.Count;
                            int i;
                            for (i = 0; i < nCount; i++)
                            {
                                DataRow row = dt.Rows[i];
                                int taskid = int.Parse(row["ID"].ToString());
                                
                                list.Clear();
                                sql = "IF NOT EXISTS(select ID from Task_Visible where TaskID=@tid and UserID=@uid) " +
                                        "insert into Task_Visible(TaskID,UserID,UserType) values(@tid,@uid,@ut)";
                                list.Add(new DictionaryEntry("@tid", taskid));
                                list.Add(new DictionaryEntry("@uid", userid));
                                list.Add(new DictionaryEntry("@ut", 3));
                                DBAccess.NoQuery(sql, list);
                                
                            }
                        }
                    }
                    allview = true;
                }
            }

            string condition = "";
            if (!allview)
            {
                condition = " and tv.isEnd=0";
                sql = "select COUNT(ID) from Task_View tv where tv.ID " +
                      "in(select distinct TaskID from Task_Visible where UserID=@uid) " +
                      "and tv.isEnd=0 and taskType=@tty";
            }
            else
            {
                sql = "select COUNT(distinct TaskID) from Task_Visible where UserID=@uid "+
                    "and TaskID in (select ID from Task_View where taskType=@tty)";
            }
            list.Clear();
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@tty", itasktype));
            int totalCount = DBAccess.QueryStatistic(sql, list);

            //分页
            sql =
            "select * from( " +
            "select ROW_NUMBER() OVER(Order by tv.ID desc) as RowNum," +
            "tv.* from Task_View tv where tv.taskType=@tty and tv.ID in(select distinct TaskID from Task_Visible where UserID=@uid) " + condition + ") as unions " +
            "where unions.RowNum between @start and @end";

           // IList list = new ArrayList();
            list.Clear();
            list.Add(new DictionaryEntry("@uid", userid));
            list.Add(new DictionaryEntry("@tty", itasktype));
            list.Add(new DictionaryEntry("@start", start + 1));
            list.Add(new DictionaryEntry("@end", start + limit));

            DataSet ds = DBAccess.Query(sql, "Task_view", list);

            string currentuserid = context.Request["userid"];
            int icurrentid = -1;
            if (currentuserid == null)
            {
                currentuserid = "";
            }
            else
            {
                if (!int.TryParse(currentuserid, out icurrentid))
                {
                    icurrentid = -1;
                }
            }
            list.Clear();
            currentuserid = currentuserid.Trim();
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
                        int ID = int.Parse(row["ID"].ToString());
                        string TaskName = row["TaskName"].ToString();
                        string dtime = ((DateTime)row["FirstSubmitTime"]).ToString("yyyy年MM月dd日");
                        string creator = row["createname"].ToString();
                        string createDep = row["CreateDName"].ToString();

                        string auditor = row["auditname"].ToString();
                        string auditdep = row["AuditDName"].ToString();
                        string state = row["statename"].ToString();
                        string statedes = row["stepDescription"].ToString();

                        //int j;
                 
                        children += "{\"taskid\":" + ID.ToString() + ",\"taskname\":\"" + TaskName
                            + "\"," + "\"taskcreatedate\":\"" + dtime + "\",\"creator\":\"" + creator + "\",\"creatordep\":\"" +
                           createDep + "\",\"auditor\": \"" + auditor + "\",auditordep:\"" + auditdep + "\",buttonName:\"" + buttonName + "\",state:\"" + state + "(" + statedes + ")\"},";
                        
                    }

                    if (children.Length > 0)
                    {
                        children = children.Substring(0, children.Length - 1);
                    }
                    ret = "{\"totalCount\":" + totalCount.ToString() + ",\"roots\":[" + children + "]}";
                }
            }
            context.Response.Write(ret);
        }

        protected override string getErrorMessage()
        {
            return "{\"totalCount\":0,\"roots\":[]}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }
}