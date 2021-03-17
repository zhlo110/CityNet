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
    /// get_document_detail 的摘要说明
    /// </summary>
    public class get_document_detail : Security
    {
        protected override string getErrorMessage()
        {
            return "";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string docid = context.Request["docid"];
            int idocid = -1;
            if(!int.TryParse(docid,out idocid))
            {
                idocid = -1;
            }
            string ret = "";
            string sql = "select ID,TableHtml from DocumentTable where ID=@docid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@docid", idocid));
            DataSet ds = DBAccess.Query(sql, "DocumentTable", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string Tablehtml = DatabaseUtility.getStringValue(row, "TableHtml");
                        ret = Tablehtml;
                    }
                }
            }
            context.Response.Write(ret);
        }
    }
}