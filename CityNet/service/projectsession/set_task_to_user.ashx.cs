using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// set_task_to_user 的摘要说明
    /// </summary>
    public class set_task_to_user : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            //查找taskid是否存在
            string sql = "select count(ID) from Task where ID = @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", itaskid));
            int nCount = DBAccess.QueryStatistic(sql, list);

            if(nCount > 0)
            {
                //删除所有与taskid有关的记录,UserType=0表示授权用户，其他的用户不要删除（创建者或审核者）
                sql = "delete from Task_Visible where TaskID=@taskid and UserType=0";
                list.Clear();
                list.Add(new DictionaryEntry("@taskid", itaskid));
                DBAccess.NoQuery(sql, list);

                //重建
                sql = "select distinct ProjectID from Session_Task where TaskID=@taskid and State = 2";//全选的Session
                sql = "select ID,Type from Project_Session where ID in (" + sql + ")";
                list.Clear();
                list.Add(new DictionaryEntry("@taskid", itaskid));
                DataSet ds = DBAccess.Query(sql, "Project_Session", list);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++)
                        {
                            DataRow row = dt.Rows[i];
                            int SessionID = DatabaseUtility.getIntValue(row, "ID", -1);
                            string type = DatabaseUtility.getStringValue(row, "Type").Trim();
                            if (SessionID > 0) //将sessionID
                            {
                                settaskvisible(SessionID, itaskid);
                            }
                        }
                    }
                }


            }
        }


        private void settaskvisible(int sessionid,int taskid)
        {
            //从[Session_User]中取Userid
            string sql = "select distinct UserID from [Session_User] where SessionID = @sid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@sid", sessionid));
            DataSet ds = DBAccess.Query(sql, "Session_User", list);
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
                        int Userid = DatabaseUtility.getIntValue(row, "UserID", -1);
                        if (Userid > 0) //得到userid
                        {
                            //查找userid是否存在
                            sql = "select count(ID) from [User] where ID=@uid";
                            list.Clear();
                            list.Add(new DictionaryEntry("@uid", Userid));
                            int userexist = DBAccess.QueryStatistic(sql, list);

                            //查找表里是否有数据
                            sql = "select count(ID) from Task_Visible where TaskID=@tid and UserID=@uid";
                            list.Clear();
                            list.Add(new DictionaryEntry("@tid", taskid));
                            list.Add(new DictionaryEntry("@uid", Userid));
                            int length = DBAccess.QueryStatistic(sql, list);

                            if (length == 0 && userexist > 0)//没有，插入
                            {
                                sql = "insert into Task_Visible(TaskID,UserID,UserType) values(@tid,@uid,0)";
                                DBAccess.NoQuery(sql, list);
                            }

                        }
                    }
                }
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