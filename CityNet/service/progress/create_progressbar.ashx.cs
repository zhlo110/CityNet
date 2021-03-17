using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.progress
{
    /// <summary>
    /// create_progressbar 的摘要说明
    /// </summary>
    public class create_progressbar : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string username = parameters[0].ToString();
            //string password = parameters[1].ToString();
            string radomNum = DateTime.Now.Ticks.ToString();
            string uniqueID = username + "_" + radomNum;
          //  string sql = "delete from ProgressBar where UniqueID =@uid";
          //  
          //  list.Add(new DictionaryEntry("@uid", uniqueID));
          //  DBAccess.NoQuery(sql, list);

            ProgressBarUtil.delete(uniqueID);

            string text = "";
            if (context.Request["showtext"] == null)
            {
                text = "";
            }
            else
            {
                text = context.Request["showtext"];
            }
            string totlenum = context.Request["total"];
            int itotal = -1;
            if(!int.TryParse(totlenum,out itotal))
            {
                itotal = -1;
            }
            string description = "";
            if (context.Request["description"] == null)
            {
                description = "";
            }
            else
            {
                description = context.Request["description"];
            }
            ProgressBarUtil.insert(uniqueID, text, itotal, 0, description);


            context.Response.Write("{success:1,msg:'新建成功',uniqueid:'" + uniqueID
                + "',showtext:'" + text + "',total:" + totlenum
                + ",description:'" + description + "',current:0,over:false}");
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}