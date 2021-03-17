using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.function
{
    /// <summary>
    /// change_function_class 的摘要说明
    /// </summary>
    public class change_function_class : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //修改功能的所属类别
            string classid = context.Request["classid"];
            string functionid = context.Request["functionid"];
            string checks = context.Request["ischeck"];

            if (classid != null && functionid != null && checks != null)
            {
                string sql = "update Action set ActionGroupID = @classid where [ID] = @id";
                int iclassid = int.Parse(context.Request["classid"]);
                int ifunctionid = int.Parse(context.Request["functionid"]);
                IList list = new ArrayList();
                if (checks.Equals("true"))
                {
                    list.Add(new DictionaryEntry("@classid", iclassid));
                }
                else
                {
                    list.Add(new DictionaryEntry("@classid", -1));
                }


                list.Add(new DictionaryEntry("@id", ifunctionid));

                DBAccess.NoQuery(sql, list);
                context.Response.Write("{success:1,msg:'修改成功'}");
            }
            else
            {
                context.Response.StatusCode = 500;
                context.Response.Write("{success:0,msg:'参数错误.'}");
            }
        }

    }
}