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
    /// getTables_By_docid 的摘要说明
    /// </summary>
    public class getTables_By_docid : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string docid = context.Request["docid"];
            string nodeid = context.Request["node"];
            if (nodeid != "project_id")
            {
                context.Response.Write("[]");
                return;
            }
            int idocid = -1;
            if (!int.TryParse(docid, out idocid))
            {
                idocid = -1;
            }

            string sql = "select [ID],[TableName] from DocumentTable where [DocumentID] = @did and deletestate = 0";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@did", idocid));
            DataSet ds = DBAccess.Query(sql, "DocumentTable", list);
            string children = "";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++ )
                    {
                        DataRow row = dt.Rows[i];
                        int ID = DatabaseUtility.getIntValue(row, "ID",-1);
                        string TableName = DatabaseUtility.getStringValue(row, "TableName");
                        TableName = TableName.Trim();
                        children +=
                        "{\"text\":'" + TableName + "',\"id\":'doc_table_id_" + ID.ToString() + "',\"qtip\":'" + TableName + "',\"leaf\":false ,\"checked\":false,\"documentid\":'"+
                        idocid.ToString()+"',\"tableid\":'" + ID.ToString() + "',\"cls\":'folder'},";
                    }
                }
            }
            if (children.Length > 0)
            {
                children = children.Substring(0, children.Length - 1);
            }
            children = "[" + children + "]";
            context.Response.Write(children);
        }
        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
    }
}