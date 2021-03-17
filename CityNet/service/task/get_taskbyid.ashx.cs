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
    /// get_taskbyid 的摘要说明
    /// </summary>
    public class get_taskbyid : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            //throw new NotImplementedException();
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            string sql = "";
       //     string template =$"my name is {name}";
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            string jsonstr = "{}";

            if (itaskid == -1) //新建
            {
                string createid = context.Request["userid"];
                string createname = context.Request["realname"];
                string createdepid = context.Request["departmentid"];
                string createdepname = context.Request["department"];
                string date = DateTime.Now.ToString("yyyy年MM月dd日");
                //stateid,和statename



                jsonstr = "{taskid:-1,taskname:'',createid:" + createid.ToString()
                    + ",auditid:0,stateid:0,submittime:'" + date + "',step:0,stepdes:'',createname:'" + createname 
                    + "',auditname:'',statename:'提交',priority:0,"
                                + "editable:0,createdepid:" + createdepid + ",createdepname:'" + createdepname
                                +"',auditdepid:0,auditdepname:''}";
            }
            else //修改
            {
                sql = "select * from Task_View where ID=@taskid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@taskid", itaskid));
                DataSet ds = DBAccess.Query(sql, "Task_View", list);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        int nCount = dt.Rows.Count;
                        if (nCount > 0)
                        {
                            DataRow row = dt.Rows[0];
                            string taskname = DatabaseUtility.getStringValue(row, "TaskName");
                            int createid = DatabaseUtility.getIntValue(row, "CreateID",-1);
                            int auditid = DatabaseUtility.getIntValue(row, "AuditID",-1);
                            int stateid = DatabaseUtility.getIntValue(row, "StateID",-1);
                            string submittime = DatabaseUtility.getDatetimeValue(row, "FirstSubmitTime");
                            int step = DatabaseUtility.getIntValue(row, "step", 0);
                            string stepdes = DatabaseUtility.getStringValue(row, "stepDescription");
                            string createname = DatabaseUtility.getStringValue(row, "createname");
                            string auditname = DatabaseUtility.getStringValue(row, "auditname");
                            string statename = DatabaseUtility.getStringValue(row, "statename");
                            int priority = DatabaseUtility.getIntValue(row, "priority", -1);
                            int editable = DatabaseUtility.getIntValue(row, "editable", 0);
                            int createdepid = DatabaseUtility.getIntValue(row, "CreateDID", -1);
                            string createdepname = DatabaseUtility.getStringValue(row, "CreateDName");
                            int auditdepid = DatabaseUtility.getIntValue(row, "AuditDID", -1);
                            string auditdepname = DatabaseUtility.getStringValue(row, "AuditDName");

                            jsonstr = "{taskid:" + itaskid.ToString() + ",taskname:'" + taskname
                                + "',createid:" + createid.ToString() + ",auditid:" + auditid.ToString()
                                + ",stateid:" + stateid.ToString() + ",submittime:'" + submittime
                                + "',step:" + step.ToString() + ",stepdes:'" + stepdes + "',createname:'" + createname
                                + "',auditname:'" + auditname + "',statename:'" + statename + "',priority:" + priority.ToString() + ","
                                + "editable:" + editable.ToString() + ",createdepid:" + createdepid.ToString()
                                + ",createdepname:'" + createdepname + "',auditdepid:" + auditdepid.ToString() + ",auditdepname:'" + auditdepname + "'}";
                        }
                    }
                }
            }
            context.Response.Write(jsonstr);
        }

       

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
    }
}