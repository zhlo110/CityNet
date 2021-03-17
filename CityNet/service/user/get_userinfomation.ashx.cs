using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.user
{
    /// <summary>
    /// get_userinfomation 的摘要说明
    /// </summary>
    public class get_userinfomation : Security
    {
        
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }

        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            DataTable dt = Security.getUserInformation(parameters);
            if (dt != null)
            {
                string username = parameters[0].ToString();
                string password = parameters[1].ToString();
                string departmentID = parameters[2].ToString();
                DataRow row = dt.Rows[0];
                string name = row["RealName"].ToString();
                string department = row["DepartmentName"].ToString();
                string id = row["ID"].ToString();
                string groupName = row["RealGroupName"].ToString();
                string group = row["GroupName"].ToString();
                string times = row["LogTime"].ToString();
                string phonenumber = row["PhoneNumber"].ToString();
                //string departmentid = row["DepartmentID"].ToString();
                string sex = row["Sex"].ToString();
                string occupation = "";
                if (row["Position"] != DBNull.Value)
                {
                    occupation = row["Position"].ToString();
                }
                string officenumber = "";
                if (row["OfficeNumber"] != DBNull.Value)
                {
                    officenumber = row["OfficeNumber"].ToString();
                }
                string email = "";
                if (row["Email"] != DBNull.Value)
                {
                    email = row["Email"].ToString();
                }
                string idcard = "";
                if (row["IDCard"] != DBNull.Value)
                {
                    idcard = row["IDCard"].ToString();
                }

                string birthday = "";
                if (row["Brithday"] != DBNull.Value)
                {
                    DateTime datetime = (DateTime)row["Brithday"];
                    birthday = datetime.ToString("yyyy年MM月dd日");
                }

                string other = "";
                if (row["Description"] != DBNull.Value)
                {
                    other = row["Description"].ToString();
                }


             //   string urlparam = "username=" + username + "&password=" + password;
              //  string enc = EncryptHelper.AESEncrypt(urlparam);
                string url = "../Home/Index?params=" + System.Web.HttpUtility.UrlEncode(parameters[3].ToString());
                string res = "{success:1 ,msg:'成功',id:" + id
                    + ",realname:'" + name + "',department:'" + department + "',groupname:'" + groupName
                    + "',username:'" + username + "',password:'"
                    + password + "',managerurl:'" + url + "',logtimes:" + times
                    + ",phone:'" + phonenumber + "',departmentID:" + departmentID
                    + ",occupation:'" + occupation + "',officephone:'" + officenumber
                    + "',idcard:'" + idcard + "',birthday:'" + birthday + "',sex:"
                    + sex + ",email:'" + email + "',other:'" + other + "'}";

                context.Response.Write(res);
            }
            else
            {
                context.Response.Write(getErrorMessage());
                context.Response.StatusCode = getErrorCode();
            }
        }
    }
}