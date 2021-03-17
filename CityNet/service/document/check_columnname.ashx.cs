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
    /// check_columnname 的摘要说明
    /// </summary>
    public class check_columnname : Security
    {
        protected override string getErrorMessage()
        {
            return "0";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string schid = context.Request["schid"];
            int ischid = -1;
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            
            }
            string rowid = context.Request["rowid"];
            int irowid = -1;
            if (!int.TryParse(rowid, out irowid))
            {
                irowid = -1;
            }
            
            string colname = context.Request["colname"];
            string sql = "select count(ID) from TableRowScheme where TableSchemeID =@tsid and Name = @name and ID <> @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tsid", ischid));
            list.Add(new DictionaryEntry("@name", colname));
            list.Add(new DictionaryEntry("@id", irowid));
            int iExist = DBAccess.QueryStatistic(sql, list);
            if (iExist > 0)
            {
                context.Response.Write("1");
            }
            else
            {
                context.Response.Write("0");
            }

        }
    }
}