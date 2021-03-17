using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.user
{
    /// <summary>
    /// loginbyuserid 的摘要说明
    /// </summary>
    public class loginbyuserid : IHttpHandler
    {

        public string login(string username,string password,int sdepartment)
        {
            string res = "";
            IList list = new ArrayList();
            list.Add(username);
            list.Add(password);
            list.Add(sdepartment.ToString());
            bool success = LogUtility.Login(username, password);
            DataTable dt = Security.getUserInformation(list);
            if (dt != null)
            {
                DataRow row = dt.Rows[0];
                string name = row["RealName"].ToString();
                string department = row["DepartmentName"].ToString();
                string id = row["ID"].ToString();
                string groupName = row["RealGroupName"].ToString();
                string group = row["GroupName"].ToString();
                string departmentid = row["DepartmentID"].ToString();

                //登录次数加1
                string addsql = "UPDATE [User] set LogTime = LogTime+1 where ID=@id";
                list.Clear();
                list.Add(new DictionaryEntry("@id", id));
                DBAccess.NoQuery(addsql, list);

                string urlparam = "username=" + username + "&password=" + password + "&department=" + departmentid;
                string enc = EncryptHelper.AESEncrypt(urlparam);

                string url = "../Home/Index?params=" + enc;
                res = "{success:1 ,msg:'成功',id:" + id
                    + ",realname:'" + name + "',department:'" + department + "',groupname:'" + groupName
                    + "',username:'" + username + "',password:'" + password + "',managerurl:'" + url + "'}";
            }
            else
            {
                if (success)
                {
                    res = "{success:0,msg:'账号已经注册成功，但还未通过管理员审核。'}";
                }
                else
                {
                   res ="{success:0,msg:'用户名和密码错误！'}";
                }
            }
            return res;
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            String userid = context.Request["userid"].Trim();
            int iuserid = -1;
            if (!int.TryParse(userid, out iuserid))
            {
                iuserid = -1;
            }
            string res = "{success:0,msg:'系统没有找到用户，请联系管理员同步用户。'}";
            string sql = "select [UserName],[PassWord] from [User] where ID=@uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", iuserid));
            DataSet ds = DBAccess.Query(sql, "[User]", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;//查数据
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        //找用户名和密码
                        string username = DatabaseUtility.getStringValue(row, "UserName");
                        string password = DatabaseUtility.getStringValue(row, "PassWord");

                        sql = "select DepartmentID from User_Department_View where ID=@uid";
                        list.Clear();
                        list.Add(new DictionaryEntry("@uid", iuserid));
                        ds = DBAccess.Query(sql, "User_Department_View", list);
                        res = "{success:0,msg:'系统没有找到用户所在的部门，请联系管理员同步部门。'}";
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                dt = ds.Tables[0];
                                nCount = dt.Rows.Count;//查数据
                                if (nCount > 0)
                                {
                                    row = dt.Rows[0];
                                    int departmentid = DatabaseUtility.getIntValue(row, "DepartmentID", -1);
                                    if (departmentid > 0)
                                    {
                                        res = login(username, password, departmentid);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            context.Response.Write(res);

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}