using CityNet.Controllers;
using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.task
{
    /// <summary>
    /// can_eidtable 的摘要说明
    /// </summary>
    public class can_eidtable : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string taskid = context.Request["taskid"];
            int itaskid = -1;
            if (!int.TryParse(taskid, out itaskid))
            {
                itaskid = -1;
            }
            int editable = 0;
            string sql = "select [editable],isEnd from Task_View where ID=@taskid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@taskid", itaskid));

            DataSet ds = DBAccess.Query(sql,"Task_View",list);

            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        int.TryParse(row["editable"].ToString(),out editable);
                        int isEnd = 0;
                        int.TryParse(row["isEnd"].ToString(), out isEnd);
                        if (isEnd > 0)
                        {
                            editable = 0;
                        }
                    }
                }
            }
            context.Response.Write("{success:1,editable:" + editable.ToString() + ",msg:'查询成功'}");
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