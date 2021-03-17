using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.tkyserver
{
    /// <summary>
    /// save_task_project 的摘要说明
    /// </summary>
    public class save_task_project : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string projectid = context.Request["projectid"].Trim();
            string projectname = context.Request["projectname"].Trim();
            string state = context.Request["checked"];
            int istate = -1;
            if (!int.TryParse(state, out istate))
            {
                istate = -1;
            }
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            string info = "插入错误";

            if (istate >= 0 && istate <= 2)
            {
                //1、判断taskid是否存在

                string sql = "select count(ID) from Task where ID = @id";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@id", itaskid));
                int nCount = DBAccess.QueryStatistic(sql,list);
                if(nCount > 0) //存在
                {
                    //2、解析projectid,_
                    char[] split = new char[] {'_'};
                    string[] vec = projectid.Split(split);
                    if(vec.Length == 2)
                    {
                        string type = vec[0].Trim();
                        string sid = vec[1].Trim();
                        int id = -1;
                        if (!int.TryParse(sid, out id))
                        {
                            id = -1;
                        }
                        if (id > 0 && projectname != null && type != null && type != "" && projectname != "")
                        {
                            //判断该ID是否存在，type
                            //不存在，插入，存在 不动
                            sql = "select count(ID) from TKY_Project where TKY_ID = @tid and Type = @ty";
                            list.Clear();
                            list.Add(new DictionaryEntry("@tid", id));
                            list.Add(new DictionaryEntry("@ty", type));
                            nCount = DBAccess.QueryStatistic(sql, list);
                            int tableid = -1;
                            if (nCount > 0)//找ID,存在
                            {
                                sql = "select ID from TKY_Project where TKY_ID = @tid and Type = @ty";
                                tableid = DBAccess.QueryStatistic(sql, list);
                            }
                            else //插入，并找ID
                            {
                                sql = "insert into TKY_Project(Name,TKY_ID,Type) values(@name,@tid,@ty)";
                                list.Clear();
                                list.Add(new DictionaryEntry("@name", projectname));
                                list.Add(new DictionaryEntry("@tid", id));
                                list.Add(new DictionaryEntry("@ty", type));
                                DBAccess.NoQuery(sql, list);

                                sql = "select ID from TKY_Project where Name=@name and TKY_ID=@tid and Type=@ty";
                                tableid = DBAccess.QueryStatistic(sql, list);
                            }

                            if (tableid > 0)
                            {
                                //建立task表与TKY_Project的关系
                                //tableid,taskid
                                sql = "select count(ID) from TKY_Task where ProjectID=@tableid and TaskID=@tid";
                                list.Clear();
                                list.Add(new DictionaryEntry("@tableid", tableid));
                                list.Add(new DictionaryEntry("@tid", taskid));
                                nCount = DBAccess.QueryStatistic(sql, list);
                                if (istate == 0) //从表中删除
                                {
                                    sql = "delete from TKY_Task where ProjectID=@tableid and TaskID=@tid";
                                }
                                else
                                {
                                    if (nCount > 0)//存在
                                    {
                                        sql = "update TKY_Task set State=@istate where ProjectID=@tableid and TaskID=@tid";
                                        list.Add(new DictionaryEntry("@istate", istate));
                                    }
                                    else
                                    {
                                        sql = "insert into TKY_Task(ProjectID,TaskID,State) values(@tableid,@tid,@istate)";
                                        list.Add(new DictionaryEntry("@istate", istate));
                                    }
                                }
                                DBAccess.NoQuery(sql, list);
                                info = "插入成功";

                            }

                        }
                    }
                }
                returnInfo(context, info);  
            }
            else
            {
                returnErrorInfo(context, "checked参数传输错误");
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