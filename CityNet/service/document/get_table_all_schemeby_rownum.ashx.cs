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
    /// get_table_all_schemeby_rownum 的摘要说明
    /// </summary>
    public class get_table_all_schemeby_rownum : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string columns = context.Request["columns"];
            int icolumns = 0;  //列个数
            if (!int.TryParse(columns, out icolumns))
            {
                icolumns = 0;
            }
            int taskid = this.stringtoint(context.Request["taskid"],-1);
            int schemeid = this.stringtoint(context.Request["schemeid"], -1);
            string sql = "";

            //找推荐方案,条件 方案有效，列的个数与参数同
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@rn", icolumns));
            if (taskid > 0 && schemeid > 0)
            {
                sql = "select Name,ID from TableScheme_View where valid = 1 and RowNum = @rn and ID in(select TableSchemeID from Task_TableScheme where TaskID=@tid)";
                list.Add(new DictionaryEntry("@tid", taskid));
                
            }
            else
            {
                sql = "select Name,ID from TableScheme_View where valid = 1 and RowNum = @rn and ID not in(select TableSchemeID from Task_TableScheme)";
            }
           
            DataSet ds = DBAccess.Query(sql, "TableScheme_View", list);
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
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        if (schemeid > 0 && ID == schemeid)
                        {
                            str = "{name:'" + Name + "',schid:" + ID.ToString() + "}," + str;
                        }
                        else
                        {
                            str += "{name:'" + Name + "',schid:" + ID.ToString() + "},";
                        }
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