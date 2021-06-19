using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// save_department_task 的摘要说明
    /// </summary>
    public class save_department_task : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string departmentid = context.Request["departmentid"].Trim();
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
                int nCount = DBAccess.QueryStatistic(sql, list);
                if (nCount > 0) //存在
                {
                    //2、解析departmentid,_
                    int id = -1;
                    if (!int.TryParse(departmentid, out id))
                    {
                        id = -1;
                    }
                    if (id > 0)
                    {
                        //建立task表与Department的关系
                        //tableid,taskid
                        sql = "select count(ID) from Department_Task where DepartmentID=@tableid and TaskID=@tid";
                        list.Clear();
                        list.Add(new DictionaryEntry("@tableid", id));
                        list.Add(new DictionaryEntry("@tid", taskid));
                        nCount = DBAccess.QueryStatistic(sql, list);
                        if (istate == 0) //从表中删除
                        {
                            sql = "delete from Department_Task where DepartmentID=@tableid and TaskID=@tid";
                        }
                        else
                        {
                            if (nCount > 0)//存在
                            {
                                sql = "update Department_Task set State=@istate where DepartmentID=@tableid and TaskID=@tid";
                                list.Add(new DictionaryEntry("@istate", istate));
                            }
                            else
                            {
                                sql = "insert into Department_Task(DepartmentID,TaskID,State) values(@tableid,@tid,@istate)";
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