using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    //处理报警
    public class UpdateAlarm
    {
        class TableColumn
        {
            public string Type;
            public string Name;
            public int No;
            public int AlarmSign;
        }
        //保存消警数据
        public static void savealarm()
        {
            savealarm("x");
            savealarm("y");
            savealarm("z");
            savealarm("parallelbais");
            savealarm("gravitybais");
        }
        public static void savealarm(string column)
        {
            string sql="insert into TempPointAlarm (ColumnName,MeasurePointID,ColumnValue) "+
                "select '" + column + "' as col,MeasurePointID, " + column
                + "_des from PointAlarm where " + column + "_des is not null and "+column
                + "_des <> '' and MeasurePointID is not null";
            DBAccess.NoQuery(sql, null);
        }
        //恢复消警报数据
        public static void restorealarm()
        {
            //从TempPointAlarm获取数据
            string sql = "select * from TempPointAlarm";
            DataSet ds = DBAccess.Query(sql, "TempPointAlarm");
            IList list = new ArrayList();
            
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
                        string columnname = DatabaseUtility.getStringValue(row, "ColumnName");
                        int mid = DatabaseUtility.getIntValue(row, "MeasurePointID",-1);
                        string value = DatabaseUtility.getStringValue(row, "ColumnValue");
                        if(mid > 0)
                        {
                            sql = "update PointAlarm set " +
                                columnname + "_des=@des," + columnname + "=0 where MeasurePointID=@mid";
                            list.Clear();
                            list.Add(new DictionaryEntry("@des", value));
                            list.Add(new DictionaryEntry("@mid", mid));
                            DBAccess.NoQuery(sql, list);
                        }

                    }
                }
            }

            sql = "delete from TempPointAlarm";
            DBAccess.NoQuery(sql, null);
        }

        //更新最新报警状态，前提条件是该任务已经审批完成
        // shemeid为方案名称
        public static void updateAlarm(int schemeid)
        {
            UpdateAlarm.savealarm();
            //1、在MeasurePoint表中找SchemeID的记录（太多）
            //查找条件 1、SchemeID=schemeid 2,TaskID必须是审批完成状态
            // 可能会增加表记录，也有可能删除表记录
            string insql = "select ID from MeasurePoint where SchemeID =@schid and TaskID " +
                "in (select ID from Task_View where priority=4)";

            //2 在上述条件成立的情况下，删除PointAlarm表
            string sql = "delete from PointAlarm where MeasurePointID in (" + insql + ")";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@schid", schemeid));
            DBAccess.NoQuery(sql, list);


            //3. 找方案
            //找方案
            sql = "select Name,No,ColumnRel,type,alarmsign from TableRowScheme where TableSchemeID = @shid order by No";
            list.Clear();
            list.Add(new DictionaryEntry("@shid", schemeid));
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
                        int alarmtype = DatabaseUtility.getIntValue(row, "alarmsign", -1); //是否为Alarm标志，如果是要判断报警
                        if (alarmtype > 0) //只存储有报警的列
                        {
                            TableColumn column = new TableColumn();
                            int No = DatabaseUtility.getIntValue(row, "No", -1);
                            string rel = DatabaseUtility.getStringValue(row, "ColumnRel");
                            string type = DatabaseUtility.getStringValue(row, "type");
                            string Name = DatabaseUtility.getStringValue(row, "Name");

                            column.No = No;
                            column.Type = type;
                            column.Name = Name;
                            column.AlarmSign = alarmtype;//是否为Alarm标志，如果是要判断报警
                            columns[rel] = column;
                        }
                    }
                }
            }

            //4 判断该方案是不是有投影信息
            sql = "select hasprojection from TableScheme where ID=@schid";
            list.Clear();
            list.Add(new DictionaryEntry("@schid", schemeid));
            int hasporject = DBAccess.QueryStatistic(sql, list);


            //5 重新建立表结构
            //5.1 找PointID
            sql = "select distinct PointID from MeasurePoint where SchemeID =@schid and TaskID " +
                "in (select ID from Task_View where priority=4)";
            list.Clear();
            list.Add(new DictionaryEntry("@schid", schemeid));
            ds = DBAccess.Query(sql, "MeasurePoint", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)//在该方案下的每一个点的ID
                    {
                        DataRow row = dt.Rows[i];
                        int pointid = DatabaseUtility.getIntValue(row, "PointID", -1);
                        if (pointid > 0)//点有效
                        {
                            //5.2重建点的报警关系
                            if (hasporject == 1)
                            {
                                setuppointalarmbyproject(columns, schemeid, pointid);
                            }
                            else
                            {
                                setuppointalarm(columns, schemeid, pointid,-1,"","");
                            }
                        }
                    }
                }
            }

            UpdateAlarm.restorealarm();
          
        }
        //查找报警ID
        public static int getAlermid(int scheid, double value, double oldvale)
        {
            int id = -1;

            double dvalue = value;
            double doldvalue = oldvale;
            double diff = Math.Abs(dvalue - doldvalue);

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
            return id;
        }

        public static void setuppointalarmbyproject(Hashtable columns, int schemeid, int pointid)
        {
            //根据L0 h分组
            string sql = "select L0,h from Point_User_View where PointID = @pid and SchemeID=@schid group by L0,h";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", pointid));
            list.Add(new DictionaryEntry("@schid", schemeid));

            DataSet ds = DBAccess.Query(sql, "Point_User_View", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)//查询L0 和h
                    {
                        DataRow row = dt.Rows[i];
                        string l0 = DatabaseUtility.getStringValue(row, "L0").Trim();
                        string h = DatabaseUtility.getStringValue(row, "h").Trim();
                        if(l0.Length > 0 && h.Length > 0) //投影分类
                        {
                            setuppointalarm(columns, schemeid, pointid,1,l0,h);
                        }

                    }
                }
            }

           // select L0,h from Point_User_View group by L0,h;
        }

        //columns 中只存有报警的列
        public static void setuppointalarm(Hashtable columns, int schemeid,int pointid,int hasproject,string l0,string h)
        {
            //将pointid所有的记录列出来，以ID排序，从小到大
            //查询条件 MeasurePoint 中的 PointID =pointid TaskID 已经审核完成，SchemeID=schemeid
            if (columns.Keys.Count > 0) //列有效
            {
                //找要查询的列
                string searchcolumn = "";
                foreach(string key in columns.Keys)
                {
                    searchcolumn += key + ",";
                }
                //只查询报警列 和ID
                string sql = "";
                if (hasproject > 0) // 有投影信息
                {
                    sql = "select " + searchcolumn + "ID from MeasurePoint where SchemeID =@schid and TaskID " +
                        "in (select ID from Task_View where priority=4) and PointID=@pid "+
                        "and L0=@l0 and h=@h order by ID asc";
                }
                else
                {
                    sql = "select " + searchcolumn + "ID from MeasurePoint where SchemeID =@schid and TaskID " +
                        "in (select ID from Task_View where priority=4) and PointID=@pid order by ID asc";
                }
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@schid", schemeid));
                list.Add(new DictionaryEntry("@pid", pointid));
                if (hasproject > 0) // 有投影信息
                {
                    list.Add(new DictionaryEntry("@l0", l0));
                    list.Add(new DictionaryEntry("@h", h));
                }

                DataSet ds = DBAccess.Query(sql, "MeasurePoint", list);
                Hashtable oldrecord = new Hashtable();//记录前面的值
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        int nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++)//查询每一条记录，建立该记录的报警表
                        {
                            DataRow row = dt.Rows[i];
                            int measureid = DatabaseUtility.getIntValue(row, "ID", -1);
                            if (measureid > 0)//点的测量ID有效
                            {
                                //当前的measureid
                                //1、查找当前值
                                Hashtable alarmtable = new Hashtable();
                                foreach (string key in columns.Keys)
                                {
                                    string svalue = DatabaseUtility.getStringValue(row, key);
                                    double dvalue=0;//当前值
                                    bool success = double.TryParse(svalue, out dvalue);
                                    if (success)
                                    {
                                        //2、查找前一个值
                                        if (oldrecord.ContainsKey(key))
                                        {
                                            IList valuelist = (IList)oldrecord[key];
                                            if (valuelist.Count > 0)
                                            {
                                                //dvalue 当前值
                                                double oldvalue = (double)valuelist[0];//前一个值
                                                //判断是否报警
                                                int alarmid = getAlermid(schemeid, dvalue, oldvalue);
                                                if (alarmid > 0) //有报警 该列
                                                {
                                                    alarmtable[key] = alarmid;
                                                }
                                            }
                                            valuelist.Insert(0, dvalue);//插入
                                        }
                                        else //如果没有，表示该条为第一条
                                        {
                                            IList valuelist = new ArrayList();
                                            valuelist.Insert(0, dvalue);
                                            oldrecord[key] = valuelist;
                                        }

                                    }
                                    
                                }

                                //插入报警数据
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
                                    list.Add(new DictionaryEntry("@mpid", measureid));
                                    sql = "insert into PointAlarm (" + arlarmstr + "MeasurePointID) values(" + arlarmval + "@mpid)";
                                    DBAccess.NoQuery(sql, list);
                                }
                              
                            }
                        }
                    }
                }


            }

        }

        //更新所有的方案
        public static void updateALlAlarm()
        {
            //1从 TableScheme中找到所有的方案
            string sql = "select ID from TableScheme";
            DataSet ds = DBAccess.Query(sql, "TableScheme", null);
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
                        int schemeid = DatabaseUtility.getIntValue(row, "ID", -1);
                        if (schemeid > 0)
                        {
                            updateAlarm(schemeid);
                        }
                    }
                }
            }


        }

    }
}