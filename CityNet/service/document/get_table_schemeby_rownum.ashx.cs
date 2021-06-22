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
    /// get_table_schemeby_rownum 的摘要说明
    /// </summary>
    public class get_table_schemeby_rownum : Security
    {

        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string columns = context.Request["columns"];
            int icolumns = 0;  //列个数
            if (!int.TryParse(columns, out icolumns))
            {
                icolumns = 0;
            }
            int schemeid = this.stringtoint(context.Request["schemeid"], -1);
 
            //找推荐方案,条件 方案有效，是推荐方案，列的个数与参数同
            string sql = "select count(ID) from TableScheme_View where valid = 1 and RowNum = @rn and Priority=1";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@rn", icolumns));
            string condition = "";
            if (schemeid > 0)
            {
                condition = " and [ID] = @schid ";
                list.Add(new DictionaryEntry("@schid", schemeid));
                sql += condition;
            }
            int Priority = DBAccess.QueryStatistic(sql, list);
            if (Priority > 0) //有推荐方案
            {
                sql = "select trs.*,tsv.hasprojection from TableRowScheme trs left join TableScheme_View tsv on trs.TableSchemeID = tsv.ID where trs.TableSchemeID=" +
                      "(select top 1 [ID] from TableScheme_View where valid = 1 and RowNum = @rn and Priority=1" + condition + ") order by trs.No";
            }
            else//没有推荐方案，找最新方案
            {
                sql = "select trs.*,tsv.hasprojection from TableRowScheme trs left join TableScheme_View tsv on trs.TableSchemeID = tsv.ID where trs.TableSchemeID=" +
                     "(select top 1 [ID] from TableScheme_View where valid = 1 and RowNum = @rn" + condition + " order by ID desc) order by trs.No ";
            }
            DataSet ds = DBAccess.Query(sql, "TableRowScheme", list);
            string str = "";
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
                            + "',colrel:'" + Colrel + "',no:" + No.ToString() 
                            + ",rowid:" + ID.ToString() + ",hasprojection:" + hasprojection.ToString() + "},";

                    }
                }
            }
            
            if (str.Length <= 0) //未找到
            {
                int i;
                for (i = 0; i < icolumns; i++)
                {
                    str += "{schid:-1,name:'未找到方案',description:'',type:'',colrel:'',no:"+(i+1).ToString()+",rowid:-1},";
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