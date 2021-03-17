using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.service.document
{
    /// <summary>
    /// get_documentview_byid 的摘要说明
    /// </summary>
    public class get_documentview_byid : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string documentid = context.Request["documentid"];
            int start = int.Parse(context.Request["start"]);
            int limit = int.Parse(context.Request["limit"]);
            string ret = getErrorMessage();
            int idocumentid = -1;
            if (!int.TryParse(documentid, out idocumentid))
            {
                idocumentid = -1;
            }
            //找页数
            string sql = "select PageCount,DocumentName,DocumentUrl from [Document] where [ID] = @did";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@did", idocumentid));

            DataSet ds = DBAccess.Query(sql, "Document", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        string documentname = DatabaseUtility.getStringValue(row, "DocumentName");
                        string path = DatabaseUtility.getStringValue(row, "DocumentUrl");
                        int pagecount = DatabaseUtility.getIntValue(row, "PageCount",0);
                        string children = "{\"src\":\"" + path + start.ToString() + ".jpg\"}";
                        ret = "{\"totalCount\":" + pagecount.ToString() + ",\"roots\":[" + children + "]}";
                    }
                }
            }
            context.Response.Write(ret);
        }
        protected override string getErrorMessage()
        {
            return "{\"totalCount\":0,\"roots\":[]}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
    }

}