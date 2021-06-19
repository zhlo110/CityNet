using CityNet.Controllers;
using CityNet.Models;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.security
{
    public abstract class Security : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            String enuser_pass = context.Request["params"];
            IList list = Security.validate(enuser_pass);
            if (list.Count > 0)
            {
                fuctionimp(context, list);
            }
            else
            {
                returnErrorInfo(context);
            }
        }

        public void returnErrorInfo(HttpContext context)
        {
            context.Response.Write(getErrorMessage());
            context.Response.StatusCode = getErrorCode();
        }

        public void returnInfo(HttpContext context, string msg)
        {
            context.Response.Write("{success:1,msg:'" + msg + "'}");
        }

        public void returnErrorInfo(HttpContext context,string msg)
        {
            context.Response.Write("{success:0,msg:'"+msg+"'}");
            context.Response.StatusCode = getErrorCode();
        }

        public static DataTable getUserInformation(IList list)
        {
            string username = list[0].ToString();
            string password = list[1].ToString();
            string sdepartment = list[2].ToString();
            String sql =
                        "select u.*,d.DepartmentName,g.GroupName,g.RealGroupName,ud.DepartmentID from " +
                        "[User] u,[Department] d, [User_Group] ug,[Group] g, [User_Department] ud " +
                        "where ug.UserID = u.ID and ug.GroupID = g.ID " +
                        "and ud.UserID = u.ID and ud.DepartmentID = d.ID " +
                        "and u.UserName = @un and u.PassWord = @pw and d.ID = @did";

            int idepartment = -1;
            if (!int.TryParse(sdepartment, out idepartment))
            {
                idepartment = -1;
            }
            IList paralist = new ArrayList();
            paralist.Add(new DictionaryEntry("@un", username));
            paralist.Add(new DictionaryEntry("@pw", password));
            paralist.Add(new DictionaryEntry("@did", sdepartment));
            DataSet dataset = DBAccess.Query(sql, "User", paralist);
            DataTable table = null;
            if (dataset != null)
            {
                int nCount = dataset.Tables.Count;
                if (nCount > 0)
                {
                     DataTable dt = dataset.Tables[0];
                     nCount = dt.Rows.Count;
                     if (nCount > 0)
                     {
                         table = dt;
                     }
                }
            }
            return table;
        }

        public static IList validate(string enuser_pass)
        {
           // String enuser_pass = Request["params"];
            IList list = new ArrayList();
            string deparams = EncryptHelper.AESDecrypt(enuser_pass);
            deparams = deparams.Replace("username=", "");
            deparams = deparams.Replace("&password=", ",");
            deparams = deparams.Replace("&department=", ",");
            string[] split = new string[] { "," };
            // deparams.sp
            string[] vec = deparams.Split(split, StringSplitOptions.RemoveEmptyEntries);
            // LogUtility

            if (vec.Length == 3)
            {
                string username = vec[0];
                string password = vec[1];
                string sdepartment = vec[2];
                Boolean successlog = LogUtility.Login(username, password);
                if (successlog)
                {
                    list.Add(username);
                    list.Add(password);
                    list.Add(sdepartment);
                    list.Add(enuser_pass);
                   // fuctionimp(context, list);
                }
            }
            return list;
        }

        //针对进度条的函数
        ProgressInfo mProgressInfo = null;
        double mStep = 0.0;
        public void initProgressBar(string taskid, int totalNum)
        {
            if (taskid != null && totalNum > 0)
            {
                mProgressInfo = ProgressBarUtil.getProgressInfo(taskid);
                mStep = (double)mProgressInfo.Total / (double)totalNum;
            }
        }

        public void updateProgress(string taskid,int i)
        {
            if (mProgressInfo != null)
            {
                int progress = (int)(i * mStep);
                ProgressBarUtil.updateProgress(taskid, progress);
            }
        }

        public void updateProgressText(string taskid, string text)
        {
            if (mProgressInfo != null)
            {
                ProgressBarUtil.updateProgressText(taskid, text);
            }
        }

        public void deleteProgress(string taskid)
        {
            if (taskid != null)
            {
                ProgressBarUtil.delete(taskid);
            }
        }

        public int stringtoint(string value,int defaultvalue)
        {
            if (value == null)
                value = "";
            int val = defaultvalue;
            if (!int.TryParse(value.Trim(), out val))
            {
                val = defaultvalue;
            }
            return val;
        }

        public double stringtodouble(string value, double defaultvalue)
        {
            double val = defaultvalue;
            if (!double.TryParse(value.Trim(), out val))
            {
                val = defaultvalue;
            }
            return val;
        }

        protected abstract void fuctionimp(HttpContext context, IList parameters);
        protected abstract string getErrorMessage();
        protected abstract int getErrorCode();

    }
}