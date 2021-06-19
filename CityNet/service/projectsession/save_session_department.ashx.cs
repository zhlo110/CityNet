using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// save_session_department 的摘要说明
    /// </summary>
    public class save_session_department : Security
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
            string  sessionid = context.Request["sessionid"].Trim();

            string state = context.Request["checked"];//当前状态
            int istate = -1;
            if (!int.TryParse(state, out istate))
            {
                istate = -1;
            }

            string sdepartmentid = context.Request["departmentid"];
            int idepartmentid = -1;
            if (!int.TryParse(sdepartmentid, out idepartmentid))
            {
                idepartmentid = -1;
            }

            string sisleaf = context.Request["isleaf"];

            string info = "插入错误";

            if (sisleaf.Equals("false"))//非叶子节点
            {
                if (istate >= 0 && istate <= 2)
                {
                    //1、判断department是否存在
                    string sql = "select count(ID) from Department where ID = @id";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@id", idepartmentid));
                    int nCount = DBAccess.QueryStatistic(sql, list);
                    if (nCount > 0) //存在
                    {
                        //2、解析sessionid
                        int isessionid = -1;
                        if (!int.TryParse(sessionid, out isessionid))
                        {
                            isessionid = -1;
                        }
                        if (isessionid > 0)
                        {
                            //建立task表与TKY_Project的关系
                            //tableid,taskid
                            sql = "select count(ID) from SessionDepartmentState where SessionID=@sid and DepartmentID=@did";
                            list.Clear();
                            list.Add(new DictionaryEntry("@sid", isessionid));
                            list.Add(new DictionaryEntry("@did", idepartmentid));
                            nCount = DBAccess.QueryStatistic(sql, list);
                            if (istate == 0) //从表中删除
                            {
                                sql = "delete from SessionDepartmentState where SessionID=@sid and DepartmentID=@did";
                            }
                            else
                            {
                                if (nCount > 0)//存在
                                {
                                    sql = "update SessionDepartmentState set State=@istate where SessionID=@sid and DepartmentID=@did";
                                    list.Add(new DictionaryEntry("@istate", istate));
                                }
                                else
                                {
                                    sql = "insert into SessionDepartmentState(SessionID,DepartmentID,State) values(@sid,@did,@istate)";
                                    list.Add(new DictionaryEntry("@istate", istate));
                                }
                            }
                            DBAccess.NoQuery(sql, list);
                            info = "插入成功";
                        }
                    }
                    returnInfo(context, info);
                }
                else
                {
                    returnErrorInfo(context, "checked参数传输错误");
                }
            }
            else//添加用户
            {
                if (istate >= 0 && istate <= 2)
                {
                    //1、判断department是否存在
                    string sql = "select count(ID) from [User] where ID = @id";
                    IList list = new ArrayList();

                    string userid = context.Request["userid"];
                    if (userid == null && userid == "")
                    {
                        userid = "";
                    }
                    int iuserid = -1;
                    if (!int.TryParse(userid,out iuserid))
                    {
                        iuserid = -1;
                    }

                    list.Add(new DictionaryEntry("@id", iuserid));
                    int nCount = DBAccess.QueryStatistic(sql, list);
                    if (nCount > 0) //存在
                    {
                        //2、解析sessionid
                        int isessionid = -1;
                        if (!int.TryParse(sessionid, out isessionid))
                        {
                            isessionid = -1;
                        }
                        if (isessionid > 0)
                        {
                            //建立task表与TKY_Project的关系
                            //tableid,taskid
                            sql = "select count(ID) from [Session_User] where SessionID=@sid and UserID=@uid";
                            list.Clear();
                            list.Add(new DictionaryEntry("@sid", isessionid));
                            list.Add(new DictionaryEntry("@uid", iuserid));
                            nCount = DBAccess.QueryStatistic(sql, list);
                            sql = "";
                            if (istate == 0) //从表中删除
                            {
                                sql = "delete from [Session_User] where SessionID=@sid and UserID=@uid";
                            }
                            else
                            {
                                if (nCount <= 0)//存在
                                {
                                    sql = "insert into [Session_User](SessionID,UserID) values(@sid,@uid)";
                                }
                            }
                            if (sql != "")
                            {
                                DBAccess.NoQuery(sql, list);
                                //由于对用户的工区进行了更改，该工区下的任务对该用户的可视关系也应做相应的修改
                                //调用存储过程修改
                                sql = " EXEC dbo.change_projcetsession_user_task @userid = @uid, @projectid = @sid, @removes = @rmv";
                                list.Clear();
                                list.Add(new DictionaryEntry("@sid", isessionid));
                                list.Add(new DictionaryEntry("@uid", iuserid));
                                list.Add(new DictionaryEntry("@rmv", istate));
                                DBAccess.NoQuery(sql, list);
                               
                                info = "插入成功";
                            }
                        }
                    }
                    returnInfo(context, info);
                }

            }
        }

    }
}