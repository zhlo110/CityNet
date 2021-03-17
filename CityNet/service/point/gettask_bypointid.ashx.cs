using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.point
{
    /// <summary>
    /// gettask_bypointid 的摘要说明
    /// </summary>
    public class gettask_bypointid : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string pointid = context.Request["pointid"];//多个
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userID = LogUtility.GetUserID(username, password);

            string[] vec = pointid.Split(new char[] { ',' });
            int i, nCount;
            nCount = vec.Length;
            IList pointlist = new ArrayList();
            for (i = 0; i < nCount; i++)
            {
                int ipointid = -1;
                if (int.TryParse(vec[i], out ipointid))
                {
                    pointlist.Add(ipointid);
                }
            }

            IList list = new ArrayList();
            string sql = "";
            if (pointlist.Count > 0)
            {
                string children = "";

                for (i = 0; i < pointlist.Count; i++)
                {
                    //查找每个点
                    sql = "select ID, PointName from Point where ID=@pid";
                    int ipointid = (int)pointlist[i];
                    list.Clear();
                    list.Add(new DictionaryEntry("@pid", ipointid));
                    DataSet ds = DBAccess.Query(sql, "Point", list);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            nCount = dt.Rows.Count;
                            int j;
                            for (j = 0; j < nCount; j++)
                            {
                                DataRow row = dt.Rows[j];
                                IList childrennode = new ArrayList();
                                int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                                string PointName = DatabaseUtility.getStringValue(row, "PointName");
                                IList pointchildren = new ArrayList();
                                getTask(pointchildren, ID,userID);
                                string schildren = converttonode(pointchildren);
                                children += "{text:'" + PointName + "',qtip:'" + PointName
                                    + "',leaf:false,expanded:true,cls:'folder',children:[" + schildren + "]},";

                            }
                        }
                    }
                }
                if (children.Length > 0)
                {
                    children = children.Substring(0, children.Length - 1);
                    children = "[" + children + "]";
                }

                context.Response.Write(children);
            }
            else
            {
                string ret = getErrorMessage();
                context.Response.Write(ret);
            }
        }

        private string converttonode(IList children)
        {
            int i;
            int nCount = children.Count;
            string ret = "";

            for (i = 0; i < nCount; i++)
            {
                ret += children[i].ToString() + ",";
            }
            if (ret.Length > 0)
            {
                ret = ret.Substring(0, ret.Length - 1);
            }
            return ret;
        }

        //通过pointID获取task,task必须为完成的，而且不能终止
        private void getTask(IList children,int pointid,int userid)
        {
            string insql = "select distinct TaskID from MeasurePoint where PointID = @pid";
            string insql2 = "select distinct TaskID from Task_Visible where UserID = @uid"; 
            string sql = "select ID,TaskName,createname,CreateDName,FirstSubmitTime from Task_View where ID in(" +
                insql + ") and ID in(" + insql2 + ") and priority=4 and isend=0";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", pointid));
            list.Add(new DictionaryEntry("@uid", userid));
            DataSet ds = DBAccess.Query(sql, "Task_View", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int j;
                    for (j = 0; j < nCount; j++)
                    {
                        DataRow row = dt.Rows[j];
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string TaskName = DatabaseUtility.getStringValue(row, "TaskName");
                        string createname = DatabaseUtility.getStringValue(row, "createname");
                        string CreateDName = DatabaseUtility.getStringValue(row, "CreateDName");
                        string FirstSubmitTime = DatabaseUtility.getDatetimeValue(row, "FirstSubmitTime");
                        string task = "{text:'" + TaskName + "',taskid:"+ID.ToString()+",creator:'"+
                            createname + "',datetime:'" + FirstSubmitTime + "',department:'" + CreateDName + "',leaf:true}";
                        children.Add(task);
                    }
                }
            }
        }

        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}