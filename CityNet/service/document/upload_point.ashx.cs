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
    /// upload_point 的摘要说明
    /// </summary>
    public class upload_point : Security
    {
        class TableColumn
        {
            public string Type;
            public string Name;
            public int No;
            public int AlarmSign;
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {

            string tableid = context.Request["tableid"];
            string documentid = context.Request["documentid"];
            string schemeid = context.Request["schemeid"];
            string L0 = context.Request["L0"].Trim();
            string h = context.Request["h"].Trim();
            string description = context.Request["description"].Trim();
            string option = context.Request["option"];

            string username = (string)parameters[0];
            string password = (string)parameters[1];
            string departmentid = (string)parameters[2];
            int userid = LogUtility.GetUserID(username,password);

            
            int itableid = -1;
            if (!int.TryParse(tableid, out itableid))
            {
                itableid = -1;
            }

            int idocumentid = -1;
            if (!int.TryParse(documentid, out idocumentid))
            {
                idocumentid = -1;
            }

            int ischemeid = -1;
            if (!int.TryParse(schemeid, out ischemeid))
            {
                ischemeid = -1;
            }

            int ioption = 0;
            if (!int.TryParse(option, out ioption))
            {
                ioption = 0;
            }
            //操作流程 点击Document,选择Scheme
            //从document中找taskID
            int itaskid = -1;
            string sql = "";
            sql = "select TaskID from Document where ID=@docid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@docid", idocumentid));
            itaskid = DBAccess.QueryStatistic(sql, list);

            //找方案
            sql = "select Name,No,ColumnRel,type,alarmsign from TableRowScheme where TableSchemeID = @shid order by No";
            list.Clear();
            list.Add(new DictionaryEntry("@shid", ischemeid));

            DataSet ds = DBAccess.Query(sql, "TableRowScheme", list);
            Hashtable columns = new Hashtable();//把方案放入表中，查询键为rel
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
                        TableColumn column = new TableColumn();
                        int No = DatabaseUtility.getIntValue(row, "No", -1);
                        string rel = DatabaseUtility.getStringValue(row, "ColumnRel");
                        string type = DatabaseUtility.getStringValue(row, "type");
                        string Name = DatabaseUtility.getStringValue(row, "Name");
                        int alarmtype = DatabaseUtility.getIntValue(row, "alarmsign", -1); //是否为Alarm标志，如果是要判断报警
                        column.No = No;
                        column.Type = type;
                        column.Name = Name;
                        column.AlarmSign = alarmtype;//是否为Alarm标志，如果是要判断报警
                        columns[rel] = column;
                    }
                }
            }
            //取原始数据
            sql = "select [ID],[Row],qtip from DocumentDataRow where TableID =@tid order by [ID]";
            list.Clear();
            list.Add(new DictionaryEntry("@tid", itableid));
            ds = DBAccess.Query(sql, "DocumentDataRow", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable rowtable = ds.Tables[0];
                    int nCount = rowtable.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        DataRow row = rowtable.Rows[i];
                        int ID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string srow = DatabaseUtility.getStringValue(row, "Row");
                        //入库
                        //1解析数据
                        ArrayList parsedata = parserow(srow);
                        string msg = "";
                        Hashtable dataformat = new Hashtable();
                        bool success = invalidate(parsedata, columns, out msg, dataformat); //数据类型验证
                        if (!success)//行数据验证失败
                        {
                            addmessage(msg,ID);
                        }
                        else //行验证成功
                        {
                            //寻找pointName
                            if (columns.ContainsKey("pointname"))
                            {
                                TableColumn tc = (TableColumn)columns["pointname"];//获取点名方案
                                if (parsedata.Count > tc.No - 1 && tc.No - 1 >= 0) //判断数组是否越界
                                {
                                    string pointname = (string)parsedata[tc.No - 1];

                                    //schemeid，pointID, userid, h,L0
                                    //入库时查看是否有重名的点
                                    sql = "select count(ID) from Point where PointName=@pname and DepartmentID=@did";
                                    list.Clear();
                                    list.Add(new DictionaryEntry("@pname", pointname));
                                    list.Add(new DictionaryEntry("@did", departmentid));
                                  //  list.Add(new DictionaryEntry("@uid", userid));
                                   // list.Add(new DictionaryEntry("@l0", L0));
                                  //  list.Add(new DictionaryEntry("@h", h));
                                  //  list.Add(new DictionaryEntry("@schid", ischemeid));
                                    int pnum = DBAccess.QueryStatistic(sql, list);
                                    int pointID = -1;
                                    if (pnum > 0) //有重名的点
                                    {
                                         if (ioption == 1) //舍弃选项
                                          {
                                              addmessage(pointname + "点已经存在，如果为复测，请在入库时选择\"标识为复测\"选项", ID);
                                              continue;
                                          }
                                          else //标识为复测选项
                                          {
                                            //找ID
                                              sql = "select top 1 ID from Point where PointName = @pname and DepartmentID=@did";
                                              list.Clear();
                                              list.Add(new DictionaryEntry("@pname", pointname));
                                              list.Add(new DictionaryEntry("@did", departmentid));
                                              pointID = DBAccess.QueryStatistic(sql, list);
                                      
                                              sql = "update Point set Description = @des,DepartmentID=@did where ID=@pid";
                                              list.Clear();
                                              list.Add(new DictionaryEntry("@des", description));
                                              list.Add(new DictionaryEntry("@did", departmentid));
                                              list.Add(new DictionaryEntry("@pid", pointID));
                                              DBAccess.NoQuery(sql, list);
                                        }
                                    }
                                    else //没有重名点
                                    {
                                        sql = "insert into Point(PointName,DepartmentID,FirstMeasureTime,Description) values(@pn,@did,@fmt,@des)";
                                        list.Clear();
                                        list.Add(new DictionaryEntry("@pn", pointname));
                                        list.Add(new DictionaryEntry("@did", departmentid));
                                        list.Add(new DictionaryEntry("@fmt", DateTime.Now));
                                        list.Add(new DictionaryEntry("@des", ""));
                                     //   list.Add(new DictionaryEntry("@sid", ischemeid));
                                        DBAccess.NoQuery(sql, list);

                                        sql = "select Max(ID) from Point";
                                        pointID = DBAccess.QueryStatistic(sql,null);
                                        //插入点
                                    }

                                    //判断是否有重复的数据
                                    //schemeid，pointID, userid, h,L0
                                    
                                    //插入数据
                                    string colstr = "";
                                    string colval = "";
                                    foreach(string key in columns.Keys)
                                    {
                                        if (key.Equals("pointname")) continue;//pointname不在MeasurePoint表中
                                        colstr += "," + key;
                                        colval += ",@"+key;
                                    }

                                    sql = "insert into MeasurePoint(PointID,pointdescription,UpLoadUserID,destoryed,L0,h,TaskID,SchemeID" +
                                        colstr + ") values(@pid,@pntdes,@uid,@destoryed,@l0,@h,@tid,@sid" + colval + ")";
                                    list.Clear();
                                    list.Add(new DictionaryEntry("@pid", pointID));
                                    list.Add(new DictionaryEntry("@pntdes", description));
                                    list.Add(new DictionaryEntry("@uid", userid));
                                    list.Add(new DictionaryEntry("@destoryed", 0));
                                    list.Add(new DictionaryEntry("@l0", L0));
                                    list.Add(new DictionaryEntry("@h", h));
                                    list.Add(new DictionaryEntry("@tid", itaskid));
                                    list.Add(new DictionaryEntry("@sid", ischemeid));
                                    foreach (string key in columns.Keys)
                                    {
                                        if (key.Equals("pointname")) continue;
                                        string sign = "@" + key;
                                        TableColumn tcindex = (TableColumn)columns[key];
                                        string value = (string)parsedata[tcindex.No - 1];

                                        if (tcindex.Type.Equals("datetime"))//datetime单独处理
                                        {
                                            string format = (string)dataformat[key];
                                            DateTime date;
                                            DateTime.TryParseExact(value,
                                                   format,
                                                   System.Globalization.CultureInfo.InvariantCulture,
                                                   System.Globalization.DateTimeStyles.None,
                                                   out date);
                                            list.Add(new DictionaryEntry(sign, date));
                                        }
                                        else
                                        {
                                            list.Add(new DictionaryEntry(sign, value));
                                        }
                                    }
                                    DBAccess.NoQuery(sql, list); //插入数据
                                    sql = "select Max(ID) from MeasurePoint";
                                    int measurepointid = DBAccess.QueryStatistic(sql, null);

                                    //插入报警数据PointAlarm表
                                    //1、计算并查找
                                    /*
                                    Hashtable alarmtable = new Hashtable();
                                    foreach (string key in columns.Keys)
                                    {
                                        if (key.Equals("pointname")) continue;
                                        TableColumn tcindex = (TableColumn)columns[key];
                                        string value = (string)parsedata[tcindex.No - 1];
                                        if (tcindex.AlarmSign == 1)//该列为判断报警lie
                                        {
                                            //   key; //列名
                                            //   value;//为值
                                            //   相比较的列，PointID相同，ID最大，key列不能为空
                                            //   TaskID 必须为已经审批完成的数据
                                            //   从Point_View查询该列的SchemeID和 key列的值
                                            sql = "select top 1 ID,TaskID,SchemeID," + key
                                                + " from Point_View where PointID=@pid and " + key + " is not null "+
                                                "and TaskID in (select ID from Task_View where priority=4) order by ID desc";
                                            list.Clear();
                                            list.Add(new DictionaryEntry("@pid", pointID));

                                            DataSet dataset = DBAccess.Query(sql, "Point_View", list);
                                            if (dataset != null)
                                            {
                                                if (dataset.Tables.Count > 0)
                                                {
                                                    DataTable datatable = dataset.Tables[0];
                                                    int rowcounts = datatable.Rows.Count;
                                                    if (rowcounts > 0)
                                                    {
                                                        DataRow datarow = datatable.Rows[0];
                                                        int shemeid = DatabaseUtility.getIntValue(datarow, "SchemeID", -1);
                                                        string oldervalue = DatabaseUtility.getStringValue(datarow, key);//比较值
                                                        //value 当前值
                                                        //找报警规则 shemeid
                                                        int alarmid = getAlermid(shemeid,value,oldervalue);
                                                        if (alarmid > 0) //存在报警
                                                        {
                                                            alarmtable[key] = alarmid;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //2、插入
                                    list.Clear();
                                    string arlarmstr = "";
                                    string arlarmval = "";
       
                                    foreach (string key in alarmtable.Keys)
                                    {
                                        arlarmstr += key + ",";
                                        arlarmval += "@" + key + ",";
                                        list.Add(new DictionaryEntry('@' + key, alarmtable[key]));
                                    }
                                    if (arlarmstr.Length > 0)
                                    {
                                        list.Add(new DictionaryEntry("@mpid", measurepointid));
                                        sql = "insert into PointAlarm (" + arlarmstr + "MeasurePointID) values(" + arlarmval + "@mpid)";
                                        DBAccess.NoQuery(sql, list); 
                                    }*/

                                    //删除原来行
                                  //  ID

                                    sql = "delete from DocumentDataRow where ID=@id";
                                    list.Clear();
                                    list.Add(new DictionaryEntry("@id", ID));
                                    DBAccess.NoQuery(sql, list); 

                                }
                                else
                                {
                                    addmessage(tc.Name + "数据越界错误", ID);
                                }
                            }
                            else
                            {
                                addmessage("方案中没有点名，请联系管理员修改方案", ID);
                            }
                        }
                    }

                    //查看表里还有没有数据
                    sql = "select count(ID) from DocumentDataRow where [TableID] = @tid";
                    list.Clear();
                    list.Add(new DictionaryEntry("@tid", tableid));
                    int rowcount = DBAccess.QueryStatistic(sql, list);
                    if (rowcount <= 0)//删除表
                    {
                        sql = "delete from DocumentTable where [ID] = @tid";
                        list.Clear();
                        list.Add(new DictionaryEntry("@tid", tableid));
                        DBAccess.NoQuery(sql,list);
                    }

                }
            }
            returnInfo(context,"执行了数据录入的命令，插入不成功的数据将保留到表中");
        }

        private int getAlermid(int scheid,string value,string oldvale)
        {
            int id = -1;

            double dvalue = 0;
            double doldvalue = 0;
            bool success1 = false;
            bool success2 = false;
            success1 = double.TryParse(value, out dvalue);
            success2 = double.TryParse(oldvale, out doldvalue);
            if (success1&&success2)
            {
                double diff = Math.Abs(dvalue-doldvalue);

                //查找规则并计算
                string sql = "select top 1 ID,AlarmLevel from AlarmScheme where SchemeID=@sid" +
                    " and minLevel <= @diff order by AlarmLevel desc;";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@sid", scheid));
                list.Add(new DictionaryEntry("@diff", diff));

                DataSet dataset = DBAccess.Query(sql, "AlarmScheme", list);
                if (dataset != null)
                {
                    if (dataset.Tables.Count > 0)
                    {
                        DataTable datatable = dataset.Tables[0];
                        int rowcounts = datatable.Rows.Count;
                        if (rowcounts > 0)
                        {
                            DataRow datarow = datatable.Rows[0];
                            id = DatabaseUtility.getIntValue(datarow, "ID", -1);
                        }
                    }
                }
            }
            return id;
        }

        private void  addmessage(string msg,int ID)
        {
            string sql = "update DocumentDataRow set qtip = @qtip where ID=@id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@qtip", msg));
            list.Add(new DictionaryEntry("@id", ID));
            DBAccess.NoQuery(sql, list);
        }
        //验证数据的有效性
        private bool invalidate(ArrayList data,Hashtable scheme,out string msg,Hashtable dateformat)
        {
            msg = "入库成功";
            bool success = true;
            foreach (string key in scheme.Keys)
            {
                //key
                //方案
                TableColumn tc = (TableColumn)scheme[key];
                if (data.Count > tc.No - 1 && tc.No - 1  >= 0)
                {
                    string value = (string)data[tc.No - 1];
                    if (tc.Type.Equals("int"))
                    {
                        int ivalue = -1;
                        if (!int.TryParse(value, out ivalue))
                        {
                            success = false;
                            msg = tc.Name + "应为数字，请检查数据的正确性";
                            break;
                        }
                    }
                    else if (tc.Type.Equals("decimal"))
                    {
                        double dvalue = 0.0;
                        if (!double.TryParse(value, out dvalue))
                        {
                            success = false;
                            msg = tc.Name + "应为数字，请检查数据的正确性";
                            break;
                        }
                    }
                    else if (tc.Type.Equals("datetime"))
                    {
                        string format1 = "yyyy-MM-dd";
                        
                        string format2 = "yyyy年MM月dd日";
                        string format3 = "yyyy-MM-dd HH:mm:ss";
                        string format4 = "yyyy年MM月dd日 HH时mm分ss秒";
                        DateTime date;

                        if (DateTime.TryParseExact(value, 
                                   format1, 
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   System.Globalization.DateTimeStyles.None, 
                                   out date))
                        {
                            dateformat[key] = format1;
                            continue;
                        }
                        else if (DateTime.TryParseExact(value,
                                   format2,
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   System.Globalization.DateTimeStyles.None,
                                   out date))
                        {
                            dateformat[key] = format2;
                            continue;
                        }
                        else if (DateTime.TryParseExact(value,
                                   format3,
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   System.Globalization.DateTimeStyles.None,
                                   out date))
                        {
                            dateformat[key] = format3;
                            continue;
                        }
                        else if (DateTime.TryParseExact(value,
                                   format4,
                                   System.Globalization.CultureInfo.InvariantCulture,
                                   System.Globalization.DateTimeStyles.None,
                                   out date))
                        {
                            dateformat[key] = format4;
                            continue;
                        }
                        else
                        {
                            success = false;
                            msg = tc.Name + "应为日期，入库数据仅支持yyyy-MM-dd,yyyy年MM月dd日,yyyy-MM-dd HH:mm:ss,yyyy年MM月dd日 HH时mm分ss秒四种格式";
                            break;
                        }
                      //  DateTime.TryParseExact(
                    }
                }
                else
                {
                    success = false;
                    msg = tc.Name + "数据越界错误";
                    break;
                }
            }

            return success;
        }

        private string trim(string row)
        {
            if (row.IndexOf('[') == 0)
            {
                row = row.Substring(1, row.Length - 1);
            }
            if (row.Length > 0 && row.LastIndexOf(']') == row.Length - 1)
            {
                row = row.Substring(0, row.Length - 1);
            }
            return row;
        }

        private ArrayList parserow(string row)
        {
            //[CP011]#[-855412.5666]#[5422350.4181]#[3242212.8429]#[与CPⅠQ54共桩]
            string[] split = new string[1];
            split[0] = "]#";
            ArrayList list = new ArrayList();
            string[] vec = row.Split(split, StringSplitOptions.None);
            int columns = vec.Length;
            int k = 0;

            for(k = 0; k < columns; k++)
            {
                string value = "";
                value = vec[k];
                value = trim(value);
                value = HttpUitls.String2Json(value);
                list.Add(value);
            }
            return list;
        }
    }
}