using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

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
        //schemeid 为表格方案ID 不是报警方案ID
        //更新报警方案，当修改报警规则的时候将更改该方案
        public static void updateAlarm(int schemeid)
        {
           
            // 1 先删除AlarmPoint中的内容（与之相关的）

            //查找条件 1、SchemeID=schemeid 2,TaskID必须是审批完成状态,3未消警状态
            string insql = "select ID from Alarm_Point_View where SchemeID =@schid and TaskID " +
                "in (select ID from Task_View where priority=4) and Eluminated=0";

            string sql = "delete from AlarmPoint where ID in (" + insql + ")";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@schid", schemeid));
            DBAccess.NoQuery(sql, list);

            //2、重建该方案的报警
            //2.1 表里表格方案中的所有报警方案
            sql = "select ID,unit,Rules,Type from AlarmScheme where SchemeID = @schid";
            DataSet ds = DBAccess.Query(sql, "AlarmScheme", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    int i;
                    for (i = 0; i < nCount; i++)
                    {
                        /*
                         { "type": "Value", "name": "值变化" },
                         { "type": "Single", "name": "单次变化" },
                         { "type": "Speed", "name": "速率变化" },
                         { "type": "Accumulate", "name": "累计变化" }
                         */
                        /*
                         { "feq": "everyyear", "name": "每年" },
                         { "feq": "everymonth", "name": "每月" },
                         { "feq": "everyday", "name": "每日" },
                         { "feq": "everyhour", "name": "每小时" },
                         { "feq": "everyminute", "name": "每分钟" },
                         { "feq": "everysecond", "name": "每秒钟" }
                         
                         */
                        DataRow row = dt.Rows[i];
                        int AlarmID = DatabaseUtility.getIntValue(row, "ID", -1);
                        string unit = DatabaseUtility.getStringValue(row, "unit");
                        string rule = DatabaseUtility.getStringValue(row, "Rules");
                        string type = DatabaseUtility.getStringValue(row, "Type");//1 Value 2 Speed 3 

                        setupalarmbyrule(schemeid, AlarmID, unit, rule, type);

                    }
                }
            }
        }

        protected static void setupalarmbyrule(int schemeid,int alarmID,string unit,string rule, string type)
        {
            string realtype = "";
            char[] split = new char[] { '_' };
            string[] vec = type.Split(split);
            realtype = vec[0].Trim();


            switch (realtype)
            {
                case "Value"://值变化
                    ValueChange(schemeid,alarmID, unit, rule);
                    break;
                case "Single"://单次变化
                    SingleChange(schemeid, alarmID, unit, rule);
                    break;
                case "Speed"://速率变化
                    SpeedChange(schemeid, alarmID, unit, rule);
                    break;
                case "Accumulate"://累计变化
                    AccumulateChange(schemeid, alarmID, unit, rule);
                    break;
            }
        }
        public static double getUnitFactor(string unit)
        {
            double factor = 1.0; //默认因子为1
            unit = unit.Trim();
            //长度单位，数据库存储以米为单位
            switch (unit)
            {
                case "公里":
                    factor = 0.001;
                    break;
                case "千米":
                    factor = 0.001;
                    break;
                case "米":
                    factor = 1.0;
                    break;
                case "分米":
                    factor = 10.0;
                    break;
                case "厘米":
                    factor = 100.0;
                    break;
                case "毫米":
                    factor = 1000.0;
                    break;

            }
            //其他单位先不管

            return factor;
        }
        protected static void insertAlarmPoint(int alarmid,int mesurepntid)
        {
            if (alarmid > 0 && mesurepntid > 0) //判断有效性
            {
                //已经消除的警报，不能再次插入
                string sql = "select max(Eluminated) from AlarmPoint where  MeasurePointID = @mid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@mid", mesurepntid));
                int Eluminated = DBAccess.QueryStatistic(sql, list);
                if (Eluminated == 1) return;//已经消警了

                //没有消警
                sql = "select count(ID) from AlarmPoint where MeasurePointID = @mid and AlarmSchemeID= @aid";
                list.Clear();
                list.Add(new DictionaryEntry("@mid", mesurepntid));
                list.Add(new DictionaryEntry("@aid", alarmid));
                int ncount = DBAccess.QueryStatistic(sql, list);
                if (ncount <= 0) //之前没有报警，现在报警了
                {
                    //插入最高等级报警（如黄色报警和红色报警，直插入红色报警，黄色报警舍弃）
                    sql = "declare @cur_shemelevel int " + //待插入的alarmsheme
                          "declare @insert_shemelevel int " + //已经插入的alarmsheme
                          "select @cur_shemelevel=Max(AlarmLevel) from AlarmScheme where ID = @aid " +
                          "select @insert_shemelevel = MAX(AlarmLevel) from AlarmPoint AP " +
                          "left join AlarmScheme ALS on AP.AlarmSchemeID = ALS.ID where MeasurePointID=@mid " +
                          "IF(@insert_shemelevel is null) " + //没有数据
                          "begin insert into AlarmPoint(MeasurePointID,AlarmSchemeID,Eluminated,Desciption) values(@mid,@aid,0,'') end " +
                          "else if (@cur_shemelevel>@insert_shemelevel) " + //有数据,且待插入的数据比数据库内的数据级别高
                          "begin update AlarmPoint set AlarmSchemeID=@aid where MeasurePointID=@mid end";
                  //  sql = "insert into AlarmPoint(MeasurePointID,AlarmSchemeID,Eluminated,Desciption) values(@mid,@aid,0,'')";
                    DBAccess.NoQuery(sql, list);
                }
            }
        }

        //值变化
        protected static void ValueChange(int schemeid,int alarmID, string unit, string rule)
        {
            string sql = "select ID from MeasurePoint where SchemeID =@schid and TaskID " +
              "in (select ID from Task_View where priority=4)";
            //得到单位
            double factor = getUnitFactor(unit);
            //解析rule,从rule里面寻找条件
            string conditionsql = factor.ToString("f4");
            conditionsql = conditionsql + "*";
            
            //解析json对象
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(rule);
            rule = json["rule"].ToString();
            rule = rule.Replace("@", conditionsql);
            sql += " and " + rule;
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@schid", schemeid));
          
            //判断sql语句是否有错
            string msg = "";
            DataSet ds = DBAccess.QueryCheckSQL(sql, "MeasurePoint",list, out msg);
            msg = msg.Trim();
            if (msg.Length == 0) //没有语法错误
            {
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
                            int measureid = DatabaseUtility.getIntValue(row, "ID", -1);
                            insertAlarmPoint(alarmID, measureid); //插入报警表
                        }
                    }
                }
            }
            else//有语法错误
            {
                sql = "update AlarmScheme set ErrorMsg = ErrorMsg+@msg where ID=@alarmid";
                list.Clear();
                msg += "<br/>";
                list.Add(new DictionaryEntry("@msg", msg));
                list.Add(new DictionaryEntry("@alarmid", alarmID));
                DBAccess.NoQuery(sql, list);

            }
        }
        //单次变化
        protected static void SingleChange(int schemeid, int alarmID, string unit, string rule)
        {
          //  string sql = "select * from MeasurePoint where SchemeID =@schid and TaskID " +
           //   "in (select ID from Task_View where priority=4)";
            //得到单位
            double factor = getUnitFactor(unit);

            //解析json对象
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(rule);
            string rules = json["rule"].ToString();

            //解析rule,从rule里面寻找条件
            string conditionsql = factor.ToString("f4");
            conditionsql = conditionsql + "*T.diff_";
            string rulecondition = rules.Replace("@", conditionsql); //添加条件


            //提取参数
            string queryvalue = "a.ID,";
            string tablevalue = "ID,";
            HashSet<string> paramSet = new HashSet<string>();
            foreach (Match match in Regex.Matches(rules,@"(?<!\w)@\w+"))
            {
                string value = match.Value;
                value = value.Substring(1, value.Length - 1);
                if (!paramSet.Contains(value))
                {
                    queryvalue += "abs(a." + value + "-b." + value + ") as diff_" + value + ",";
                    tablevalue += value + ",";
                    paramSet.Add(value);
                }
            }
            queryvalue = queryvalue.Substring(0, queryvalue.Length - 1);
            queryvalue += " ";
            tablevalue = tablevalue.Substring(0, tablevalue.Length - 1);
            tablevalue += " ";
            /*
            string sql = "select T.* from (select " + queryvalue + " from " +
                         "(select ROW_NUMBER() OVER(Order by MeasureTime asc) as RowNum ," + tablevalue + " from MeasurePoint " +
                         "where SchemeID = @schid and TaskID in (select ID from Task_View where priority=4)) a," +
                         "(select ROW_NUMBER() OVER(Order by MeasureTime asc) as RowNum ," + tablevalue + " from MeasurePoint " +
                         "where SchemeID = @schid and TaskID in (select ID from Task_View where priority=4)) b " +
                         "where a.RowNum = b.RowNum+1) T where " + rulecondition; */

            string sql = 
            "select * from("+
            "select " +  queryvalue +" from" +
            "(select ROW_NUMBER() OVER(partition by PointID Order by  MeasureTime asc) as RowNum,MeasureTime,PointID," + tablevalue +
            " from MeasurePoint " + 
            "where SchemeID = @schid and TaskID in (select ID from Task_View where priority=4)) a,"+
            "(select ROW_NUMBER() OVER(partition by PointID Order by  MeasureTime asc) as RowNum,MeasureTime,PointID," + tablevalue +
            " from MeasurePoint " + 
            "where SchemeID =@schid and TaskID in (select ID from Task_View where priority=4)) b where "+
            "a.RowNum = b.RowNum+1  and a.PointID = b.PointID) T where "+ rulecondition;



            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@schid", schemeid));

            //判断sql语句是否有错
            string msg = "";
            DataSet ds = DBAccess.QueryCheckSQL(sql, "MeasurePoint", list, out msg);
            msg = msg.Trim();
            if (msg.Length == 0) //没有语法错误
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        int nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++) //从1开始
                        {
                            DataRow row = dt.Rows[i];//本行
                            int measureid = DatabaseUtility.getIntValue(row, "ID", -1);
                            insertAlarmPoint(alarmID, measureid); //插入报警表
                        }
                    }
                }
            }
            else//有语法错误
            {
                sql = "update AlarmScheme set ErrorMsg = ErrorMsg+@msg where ID=@alarmid";
                list.Clear();
                msg += "<br/>";
                list.Add(new DictionaryEntry("@msg", msg));
                list.Add(new DictionaryEntry("@alarmid", alarmID));
                DBAccess.NoQuery(sql, list);

            }

        }

        //根据速度算
        protected static void SpeedChange(int schemeid, int alarmID, string unit, string rule)
        {
            //  string sql = "select * from MeasurePoint where SchemeID =@schid and TaskID " +
            //   "in (select ID from Task_View where priority=4)";
            //得到单位
            double factor = getUnitFactor(unit);

            //解析json对象
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(rule);
            string rules = json["rule"].ToString();
            string frequency = json["frequency"].ToString().Trim(); //频率

            //解析rule,从rule里面寻找条件
            string conditionsql = factor.ToString("f4");
            conditionsql = conditionsql + "*T.diff_";
            string rulecondition = rules.Replace("@", conditionsql); //添加条件
            //{type:'Single',frequency:'everyday',rule:'@x>=1 and @y >= 1'}

            //提取参数
            string queryvalue = "a.ID,";
            string tablevalue = "ID,";
            HashSet<string> paramSet = new HashSet<string>();
            foreach (Match match in Regex.Matches(rules, @"(?<!\w)@\w+"))
            {
                string value = match.Value;
                value = value.Substring(1, value.Length - 1);
                if (!paramSet.Contains(value))
                {
                    queryvalue += "abs(a." + value + "-b." + value + ")/dbo.DateTimeDiff('" +
                        frequency + "',b.MeasureTime,a.MeasureTime) as diff_" + value + ",";
                    tablevalue += value + ",";
                    paramSet.Add(value);
                }
            }
            queryvalue = queryvalue.Substring(0, queryvalue.Length - 1);
            queryvalue += " ";
            tablevalue = tablevalue.Substring(0, tablevalue.Length - 1);
            tablevalue += " ";
            /*
            string sql = "select T.* from (select " + queryvalue + " from " +
                         "(select ROW_NUMBER() OVER(Order by MeasureTime asc) as RowNum,MeasureTime," + tablevalue + " from MeasurePoint " +
                         "where SchemeID = @schid and TaskID in (select ID from Task_View where priority=4)) a," +
                         "(select ROW_NUMBER() OVER(Order by MeasureTime asc) as RowNum,MeasureTime," + tablevalue + " from MeasurePoint " +
                         "where SchemeID = @schid and TaskID in (select ID from Task_View where priority=4)) b " +
                         "where a.RowNum = b.RowNum+1) T where " + rulecondition;*/


            string sql =
            "select * from(" +
            "select " + queryvalue + " from" +
            "(select ROW_NUMBER() OVER(partition by PointID Order by  MeasureTime asc) as RowNum,MeasureTime,PointID," + tablevalue +
            " from MeasurePoint " +
            "where SchemeID = @schid and TaskID in (select ID from Task_View where priority=4)) a," +
            "(select ROW_NUMBER() OVER(partition by PointID Order by  MeasureTime asc) as RowNum,MeasureTime,PointID," + tablevalue +
            " from MeasurePoint " +
            "where SchemeID =@schid and TaskID in (select ID from Task_View where priority=4)) b where " +
            "a.RowNum = b.RowNum+1  and a.PointID = b.PointID) T where " + rulecondition;



            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@schid", schemeid));

            //判断sql语句是否有错
            string msg = "";
            DataSet ds = DBAccess.QueryCheckSQL(sql, "MeasurePoint", list, out msg);
            msg = msg.Trim();
            if (msg.Length == 0) //没有语法错误
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        int nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++) //从1开始
                        {
                            DataRow row = dt.Rows[i];//本行
                            int measureid = DatabaseUtility.getIntValue(row, "ID", -1);
                            insertAlarmPoint(alarmID, measureid); //插入报警表
                        }
                    }
                }
            }
            else//有语法错误
            {
                sql = "update AlarmScheme set ErrorMsg = ErrorMsg+@msg where ID=@alarmid";
                list.Clear();
                msg += "<br/>";
                list.Add(new DictionaryEntry("@msg", msg));
                list.Add(new DictionaryEntry("@alarmid", alarmID));
                DBAccess.NoQuery(sql, list);

            }

        }

        //累计变化
        protected static void AccumulateChange(int schemeid, int alarmID, string unit, string rule)
        {
            //  string sql = "select * from MeasurePoint where SchemeID =@schid and TaskID " +
            //   "in (select ID from Task_View where priority=4)";
            //得到单位
            double factor = getUnitFactor(unit);

            //解析json对象
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(rule);
            string rules = json["rule"].ToString();
            string frequency = json["frequency"].ToString().Trim(); //频率

            //解析rule,从rule里面寻找条件
            string conditionsql = factor.ToString("f4");
            conditionsql = conditionsql + "*T.diff_";
            string rulecondition = rules.Replace("@", conditionsql); //添加条件
            //{type:'Single',frequency:'everyday',rule:'@x>=1 and @y >= 1'}

            //提取参数
            string queryvalue = "baseA.ID,";
            string tablevalue = "ID,";
            HashSet<string> paramSet = new HashSet<string>();
            foreach (Match match in Regex.Matches(rules, @"(?<!\w)@\w+"))
            {
                string value = match.Value;
                value = value.Substring(1, value.Length - 1);
                if (!paramSet.Contains(value))
                {
                    queryvalue += "abs(a1." + value + "-baseA." + value + ") as diff_" + value + ",";
                    tablevalue += value + ",";
                    paramSet.Add(value);
                }
            }
            queryvalue = queryvalue.Substring(0, queryvalue.Length - 1);
            queryvalue += " ";
            tablevalue = tablevalue.Substring(0, tablevalue.Length - 1);
            tablevalue += " ";

            string sql =
                "select * from" +
                "(select baseA.MeasureTime,baseA.PointID," +
                queryvalue +
                "from " +
                "(select MeasureTime,PointID," +
                tablevalue +
                "from MeasurePoint where SchemeID =@schid and TaskID in (select ID from Task_View where priority=4)) " +
                "baseA left join " +
                "(select * from(" +
                "select PointID,MeasureTime," +
                "ROW_NUMBER() over (partition by PointID order by MeasureTime) as classNo," +
                tablevalue +
                " from MeasurePoint where SchemeID = @schid " +
                "and TaskID in (select ID from Task_View where priority=4)) a where a.classNo=1) a1 " +
                "on baseA.PointID = a1.PointID) T where " + rulecondition;

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@schid", schemeid));

            //判断sql语句是否有错
            string msg = "";
            DataSet ds = DBAccess.QueryCheckSQL(sql, "MeasurePoint", list, out msg);
            msg = msg.Trim();
            if (msg.Length == 0) //没有语法错误
            {
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        int nCount = dt.Rows.Count;
                        int i;
                        for (i = 0; i < nCount; i++) //从1开始
                        {
                            DataRow row = dt.Rows[i];//本行
                            int measureid = DatabaseUtility.getIntValue(row, "ID", -1);
                            insertAlarmPoint(alarmID, measureid); //插入报警表
                        }
                    }
                }
            }
            else//有语法错误
            {
                sql = "update AlarmScheme set ErrorMsg = ErrorMsg+@msg where ID=@alarmid";
                list.Clear();
                msg += "<br/>";
                list.Add(new DictionaryEntry("@msg", msg));
                list.Add(new DictionaryEntry("@alarmid", alarmID));
                DBAccess.NoQuery(sql, list);

            }

        }
        


        //更新最新报警状态，前提条件是该任务已经审批完成
        // shemeid为方案名称
        public static void updateAlarm_bak(int schemeid)
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