using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// set_table_deletesign 的摘要说明
    /// </summary>
    public class set_table_deletesign : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string docid = context.Request["docid"];
            string tablesids  = context.Request["tableids"];
            int idocid = 0;
            if(!int.TryParse(docid,out idocid))
            {
                idocid = 0;
            }
            ArrayList ids = new ArrayList();
            if (tablesids != null)
            {
                char[] split = new char[1];
                split[0] = ',';

                string[] vec = tablesids.Split(split);
                int i;
                int nCount = vec.Length;
                string incondition = "";
                for (i = 0; i < nCount; i++)
                {
                    string sid = vec[i].Trim();
                    int value = -1;
                    if (int.TryParse(sid, out value))
                    {
                        ids.Add(value);
                        incondition += value.ToString() + ',';
                    }
                }
                if (ids.Count > 0)
                {
                    incondition = incondition.Substring(0, incondition.Length - 1);
                    string sql = "update DocumentTable set [deletestate] = 1 where DocumentID =@did and ID in (" + incondition + ")";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@did", idocid));
                    DBAccess.NoQuery(sql, list);
                    returnInfo(context, "删除成功");
                }
                else
                {
                    returnErrorInfo(context, "未找到要删除的数据");   
                }
            }
            else
            {
                returnErrorInfo(context,"参数错误");   
            }
            //throw new NotImplementedException();
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