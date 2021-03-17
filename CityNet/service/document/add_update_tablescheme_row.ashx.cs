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
    /// add_update_tablescheme_row 的摘要说明
    /// </summary>
    public class add_update_tablescheme_row : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string rowid = context.Request["rowid"];
            string rowname = context.Request["rowname"];
            string colrel = context.Request["colrel"];
            string description = context.Request["description"];
            string schid = context.Request["schid"];
            string type = context.Request["type"];
            string no = context.Request["no"];
            string alarmcheck = context.Request["alarmcheck"];

            if (rowid != null && rowname != null && colrel != null && schid != null 
                && no != null && type!=null)
            {
                int irowid = -1;
                if (!int.TryParse(rowid, out irowid))
                {
                    irowid = -1;
                }
                int ischid = -1;
                if (!int.TryParse(schid, out ischid))
                {
                    ischid = -1;
                }
                int ino = -1;
                if (!int.TryParse(no, out ino))
                {
                    ino = -1;
                }

                int ialarmcheck = 0;
                if (!int.TryParse(alarmcheck, out ialarmcheck))
                {
                    ialarmcheck = 0;
                }

                if (irowid == -1)//添加
                {
                    string sql = "insert into [TableRowScheme] (Name,ColumnRel,Description,TableSchemeID,No,type,alarmsign)" +
                        " values (@sn,@cr,@des,@sid,@no,@ty,@alarm)";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@sn", rowname));
                    list.Add(new DictionaryEntry("@cr", colrel));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@sid", ischid));
                    list.Add(new DictionaryEntry("@no", ino));
                    list.Add(new DictionaryEntry("@ty", type));
                    list.Add(new DictionaryEntry("@alarm", ialarmcheck));

                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'插入列成功'}");

                }
                else
                {
                    // (Name,Description,creatorID,Priority,createTime)" +
                    //    " values (@sn,@des,@createid,@pri,@ct)
                    string sql = "update [TableRowScheme] set Name=@sn,ColumnRel = @cr,Description=@des,No=@no,type=@ty where ID=@id";
                    IList list = new ArrayList();
                    list.Add(new DictionaryEntry("@sn", rowname));
                    list.Add(new DictionaryEntry("@cr", colrel));
                    list.Add(new DictionaryEntry("@des", description));
                    list.Add(new DictionaryEntry("@no", ino));
                    list.Add(new DictionaryEntry("@ty", type));
                    list.Add(new DictionaryEntry("@id", irowid));
                    DBAccess.NoQuery(sql, list);
                    context.Response.Write("{success:1,msg:'更新列成功'}");
                }
            }
            else
            {
                context.Response.Write("{success:0,msg:'表单格式错误'}");
                context.Response.StatusCode = getErrorCode();
            }
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
    }
}