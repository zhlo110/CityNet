using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.controller
{
    /// <summary>
    /// Document 的摘要说明
    /// </summary>
    public class Document : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string enuser_pass = context.Request["params"];
            string documentid = context.Request["docid"];
            IList vec = Security.validate(enuser_pass);//验证
            String json = "{success:0,url:'',params:'',message:'后台程序出错',documentid:0}";
            if (vec.Count >= 3 && documentid != null)
            {
                DataTable dt = Security.getUserInformation(vec);
                if (dt != null)
                {
                    DataRow row = dt.Rows[0];
                    string name = row["RealName"].ToString();
                    string department = row["DepartmentName"].ToString();
                    string groupName = row["RealGroupName"].ToString();

                    string url = "../Home/Manager?params=" + System.Web.HttpUtility.UrlEncode(enuser_pass);
                    string param = System.Web.HttpUtility.UrlEncode(enuser_pass);
                    string message = "欢迎您！" + department + "的" + name + "(" + groupName + ")。";

                    json = "{success:1,url:'" + url + "',params:'" + param + "',message:'" + message + "',documentid:" + documentid + "}";
                }
            }

            context.Response.Write(json);
            
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