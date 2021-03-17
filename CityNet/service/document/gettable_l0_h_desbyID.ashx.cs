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
    /// gettable_l0_h_desbyID 的摘要说明
    /// </summary>
    public class gettable_l0_h_desbyID : Security
    {

        protected override string getErrorMessage()
        {
            return "{tableid:-1,description:'',l0:'',h:''}";
        }

        protected override int getErrorCode()
        {
            return 200;
        }
        private string removeallret(string val)
        {
            val = val.Replace("\\r","");
            val = val.Replace("\\n","");
            val = val.Replace("\\t", "");
            return val;
        }

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string tableid = context.Request["tableid"].Trim();
            int itableid = -1;
            if (!int.TryParse(tableid, out itableid))
            {
                itableid = -1;
            }
            string ret = getErrorMessage();
            string sql = "select ID,Description,L0,h from DocumentTable where ID=@id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", itableid));
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
                        string L0 = HttpUitls.String2Json(removeallret(DatabaseUtility.getStringValue(row, "L0")).Trim());
                        string h = HttpUitls.String2Json(removeallret(DatabaseUtility.getStringValue(row, "h")).Trim());
                        string description = HttpUitls.String2Json(removeallret(DatabaseUtility.getStringValue(row, "Description")).Trim());
                        ret = "{tableid:" + ID.ToString() + ",description:'" + description + "',l0:'"+L0+"',h:'"+h+"'}";
                    }
                }
            }
            context.Response.Write(ret);
        }
    }
}