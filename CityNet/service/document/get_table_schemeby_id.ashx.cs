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
    /// get_table_schemeby_id 的摘要说明
    /// </summary>
    public class get_table_schemeby_id : Security
    {
        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string schid = context.Request["schid"];
            int ischid = -1;  //列个数
            if (!int.TryParse(schid, out ischid))
            {
                ischid = -1;
            }
            //找推荐方案,条件 方案有效，是推荐方案，列的个数与参数同
            string sql = "select trs.*,ts.hasprojection from TableRowScheme trs "
                +"left join TableScheme ts on ts.ID = trs.TableSchemeID where TableSchemeID = @tsid order by No asc";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@tsid", ischid));

            DataSet ds = DBAccess.Query(sql, "TableRowScheme", list);
            string str = "";
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = dt.Rows[i];
                        string Name = DatabaseUtility.getStringValue(row, "Name");
                        string Description = DatabaseUtility.getStringValue(row, "Description");
                        string Colrel = DatabaseUtility.getStringValue(row, "ColumnRel");
                        string Type = DatabaseUtility.getStringValue(row, "type");

                        int TabSchemeID = DatabaseUtility.getIntValue(row, "TableSchemeID", -1);
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        int No = DatabaseUtility.getIntValue(row, "No", -1);
                        int hasprojection = DatabaseUtility.getIntValue(row, "hasprojection", 0);

                        str += "{schid:" + TabSchemeID.ToString() + ",name:'" + Name
                            + "',description:'" + Description + "',type:'" + Type
                            + "',colrel:'" + Colrel + "',no:" + No.ToString() + ",rowid:" + ID.ToString() + ",hasprojection:" + hasprojection.ToString() + "},";
                    }
                }
            }

            if (str.Length > 0) //整理
            {
                str = str.Substring(0, str.Length - 1);
                str = "[" + str + "]";
            }
            else
            {
                str = "[]";
            }
            context.Response.Write(str);
        }

    }
}