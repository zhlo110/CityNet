using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class DepartmentNode : TreeNode
    {
        public DepartmentNode()
        {
        }
        public override bool isLeaf()
        {
           return (bool)getValue("Leaf");
        }
        //获取单个节点的JSON,叶子节点
        public override string leafjson(IList paramaters)
        {
            bool ischeck = false;
            if (paramaters.Count >= 2)
            {
                ischeck = (bool)paramaters[1];
            }
            string scheck = "";
            if(ischeck)
            {
                scheck = "\"checked\":false,";
            }

            string retjson = "{\"text\":'" + getValue("Name").ToString()
                        + "',\"createtime\":\"" + getValue("CreateTime").ToString()
                        + "\",\"description\":\"" + getValue("Description").ToString()
                        + "\","+scheck+"\"projectName\":'" + getValue("Name").ToString()
                        + "',\"id\":\"" + this.Key.ToString() 
                        + "\",\"qtip\":" + getValue("ID").ToString() + ",\"leaf\": true,\"provider\":''}";
            if (paramaters.Count >= 4)
            {
                bool checkstate = (bool)paramaters[2];
                int sessionid = (int)paramaters[3];
                if (checkstate)
                {
                    //查询state
                    int departmentid = (int)getValue("ID");
                    int state = getUserState(sessionid, departmentid);

                    string check = "false";
                    string indeterminate = "false";
                    if (state == 0)
                    {
                        check = "false";
                        indeterminate = "false";
                    }
                    else if (state == 1)
                    {
                        check = "true";
                        indeterminate = "false";
                    }
                    string scheckbox = ",indeterminate:" + indeterminate + ",checked:" + check;
                    retjson = "{text:'" + getValue("Name").ToString() + "',"
                           + "id:'" + Key.ToString()
                           + "',leaf:true,qtip:" + getValue("ID").ToString() + scheckbox + "}";
                }
            }
            return retjson;

        }

        private int getUserState(int sessionid, int userid)
        {
            //
            string sql = "select count(ID) from [Session_User] where SessionID = @sid and UserID = @uid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@sid", sessionid));
            list.Add(new DictionaryEntry("@uid", userid));
            int Count = DBAccess.QueryStatistic(sql,list);
            if (Count > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private int getState(int sessionid,int departmentid)
        {
            string sql = "select State from SessionDepartmentState where SessionID = @sid and DepartmentID = @did";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@sid", sessionid));
            list.Add(new DictionaryEntry("@did", departmentid));
            return DBAccess.QueryStatistic(sql, list);
        }

        //获取节点的JSON,非叶子节点，childrenjson为叶子节点
        public override string nodejson(string childrenjson, IList paramaters)
        {
            string retjson = "{\"text\":'" + getValue("Name").ToString()
                           + "',\"createtime\":\"" + getValue("CreateTime").ToString()
                           + "\",\"description\":\"" + getValue("Description").ToString()
                           + "\",\"projectName\":'" + getValue("Name").ToString() + "'," + "\"id\":"
                           + Key.ToString()
                           + ",\"qtip\":1,\"leaf\": false,\"expanded\":true,\"provider\":'',children:[" + childrenjson + "]}";
            if (paramaters.Count >= 4)
            {
                bool checkstate = (bool)paramaters[2];
                int sessionid = (int)paramaters[3];
                if (checkstate)
                {
                    //查询state
                    int departmentid = (int)getValue("ID");
                    int state = getState(sessionid, departmentid);

                    string check = "false";
                    string indeterminate = "false";
                    if (state == 1)
                    {
                        check = "true";
                        indeterminate = "true";
                    }
                    else if (state == 2)
                    {
                        check = "true";
                        indeterminate = "false";
                    }
                    
                    string scheckbox = ",indeterminate:" + indeterminate + ",checked:" + check;
                    

                    retjson = "{text:'" + getValue("Name").ToString()+"'," 
                           + "id:"+ Key.ToString()
                           + ",leaf: false,expanded:false"+scheckbox+"}";
                }
            }
            return retjson;
        }

        public override void deletebyids(string ids)
        {
            string sql = "exec('delete from Department where [ID] in ('+@ids+')')";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@ids", ids));
            DBAccess.NoQuery(sql, list);

            sql = "exec('delete from User_Department where [DepartmentID] in ('+@ids+')')";
            DBAccess.NoQuery(sql, list);
        }

        protected override Hashtable searchImp(string pattern, bool includeLeaves)
        {
            Hashtable result = new Hashtable();
            Hashtable leavestable = new Hashtable();
            string sql = "select [ID],[DepartmentName],[CreateTime],[Description] from Department where [DepartmentName] like '%'+@seachtext+'%'";
            IList list = new ArrayList();
  
            list.Add(new DictionaryEntry("@seachtext", pattern));
            //查询department中匹配的字段
            DataSet ds = DBAccess.Query(sql, "Department", list);
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
                        int id = int.Parse(row["ID"].ToString());
                        string departmentName = row["DepartmentName"].ToString();
                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["CreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");
                        TreeNode node = new DepartmentNode();
                        node.setValue("ID", id);
                        node.setValue("Name", departmentName);
                        node.setValue("Leaf",false);
                        node.setValue("DepartmentID", id);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);
                  //      node.setValue("DeparmentID","");
                        node.Key = id;//department

                        if (includeLeaves) //如果包含人员，把这个节点的人员添加进来
                        {
                            IList childrenNode = node.createChildrenLeaves();
                            int j, jCount;
                            jCount = childrenNode.Count;
                            for (j = 0; j < jCount; j++ )
                            {
                                TreeNode subnode = childrenNode[j] as TreeNode;
                                leavestable.Add(subnode.Key, subnode);
                                node.Children.Add(subnode);
                                subnode.Parent = node;
                            }
                        }

                        result.Add(node.Key,node);
                    }
                }
            }
            //包含部门人员
            if (includeLeaves)
            {
                //查找部门人员和部门
                sql = "select u.[ID],u.[RealName],u.[CreateTime],u.[Description],ud.DepartmentID,d.DepartmentName,d.CreateTime as dCreateTime,d.Description as dDescription " +
                    "from [User] u,User_Department ud,[Department] d " +
                    "where ud.DepartmentID = d.ID and u.ID = ud.UserID and u.[RealName] like '%'+@seachtext+'%'";
           //     IList list = new ArrayList();
           //     list.Add(new DictionaryEntry("@seachtext", pattern));
                ds = DBAccess.Query(sql, "User", list);
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
                             int id = int.Parse(row["ID"].ToString());
                             string RealName = row["RealName"].ToString();
                             int departmentid = int.Parse(row["DepartmentID"].ToString());
                             string departmentName = row["DepartmentName"].ToString();
                             string description = row["Description"].ToString();
                             DateTime dtime = (DateTime)row["CreateTime"];
                             string createtime = dtime.ToString("yyyy年MM月dd日");
                             dtime = (DateTime)row["dCreateTime"];
                             string dcreatetime = dtime.ToString("yyyy年MM月dd日");
                             string ddescription = row["dDescription"].ToString();


                             TreeNode node = new DepartmentNode();
                             node.setValue("ID", id);
                             node.setValue("Name", RealName);
                             node.setValue("Leaf", true);
                             node.setValue("DepartmentID", departmentid);
                             node.setValue("CreateTime", createtime);
                             node.setValue("Description", description);
                             node.Key = "User_ID_" + id.ToString() + "DepartID_" + departmentid.ToString(); //userid

                             TreeNode parentnode = null;
                             if (result.ContainsKey(departmentid))
                             {
                                 parentnode = result[departmentid] as TreeNode;
                             }
                             else
                             {
                                 parentnode = new DepartmentNode();
                                 parentnode.setValue("ID", departmentid);
                                 parentnode.setValue("Name", departmentName);
                                 parentnode.setValue("Leaf", false);
                                 parentnode.setValue("DepartmentID", id);
                                 parentnode.setValue("CreateTime", dcreatetime);
                                 parentnode.setValue("Description", ddescription);

                                 parentnode.Key = departmentid;//不管
                                 result.Add(parentnode.Key, parentnode);
                             }
                             if (!leavestable.ContainsKey(node.Key))
                             {
                                 parentnode.Children.Add(node);
                             }
                         }
                     }
                 }
            }
            return result;
        }
        public override  IList createChildrenLeaves()
        {
            IList children = new ArrayList();
            //得到子节点
            int id = (int)getValue("ID");
            string sql = "select u.[ID],u.[RealName],u.[CreateTime],u.[Description],ud.DepartmentID,d.DepartmentName " +
                    "from [User] u,User_Department ud,[Department] d " +
                    "where ud.DepartmentID = d.ID and u.ID = ud.UserID and ud.DepartmentID = @pid";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", id));
            DataSet ds = DBAccess.Query(sql, "User", list);

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
                        int cid = int.Parse(row["ID"].ToString());
                        string username = row["RealName"].ToString();
                        string departmentName = row["DepartmentName"].ToString();
                        int departmentid = int.Parse(row["DepartmentID"].ToString());
                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["CreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");

                        TreeNode node = new DepartmentNode();
                        node.setValue("ID", cid);
                        node.setValue("Name", username);
                        node.setValue("Leaf", true);
                        node.setValue("DepartmentID", departmentid);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);
                        node.Key = "User_ID_" + cid.ToString() + "DepartID_" + departmentid.ToString();//userid
                        children.Add(node);
                    }
                }
            }
            return children;
        }

        public override IList createChildren()
        {
            IList children = new ArrayList();
            //得到子节点
            int id = (int)getValue("ID");
            string sql = "select [ID],[DepartmentName],[Description],[CreateTime] from Department where ParentID = @pid";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", id));
            DataSet ds = DBAccess.Query(sql, "Department", list);

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
                        int cid = int.Parse(row["ID"].ToString());
                        string departmentName = row["DepartmentName"].ToString();

                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["CreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");

                        TreeNode node = new DepartmentNode();
                        node.setValue("ID", cid);
                        node.setValue("Name", departmentName);
                        node.setValue("Leaf", false);
                        node.setValue("DepartmentID", cid);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);
                        node.Key = cid; //departmentid
                        children.Add(node);
                    }
                }
            }
            return children;
        }

        public override TreeNode createNode(int ID)
        {
            TreeNode result = null;
            //当前节点的ID
            int id = ID;
            string sql = "select [ID],[DepartmentName],[Description],[CreateTime] from Department where [ID] = @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", id));
            //查询department中匹配的字段
            DataSet ds = DBAccess.Query(sql, "Department", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        id = int.Parse(row["ID"].ToString());
                        string departmentName = row["DepartmentName"].ToString();
                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["CreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");

                        TreeNode node = new DepartmentNode();
                        node.setValue("ID", id);
                        node.setValue("Name", departmentName);
                        node.setValue("Leaf", false);
                        node.setValue("DepartmentID", id);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);
                        node.Key = id; //departmentid
                        result = node;
                    }
                }
            }
            return result;
        }

        public override TreeNode createParent()
        {
            TreeNode result = null;
            //当前节点的ID
            int id = (int)getValue("ID");
            string sql = "select [ID],[DepartmentName],[Description],[CreateTime] from Department where [ID] in " +
                         "(select ParentID from Department where ID = @uid)";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@uid", id));
            //查询department中匹配的字段
            DataSet ds = DBAccess.Query(sql, "Department", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        int pid = int.Parse(row["ID"].ToString());
                        string pdepartmentName = row["DepartmentName"].ToString();
                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["CreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");

                        TreeNode node = new DepartmentNode();
                        node.setValue("ID", pid);
                        node.setValue("Name", pdepartmentName);
                        node.setValue("Leaf", false);
                        node.setValue("DepartmentID", pid);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);

                        //      node.setValue("DeparmentID","");
                        node.Key = pid; //departmentid
                        result = node;
                    }
                }
            }

            return result;
        }



        public override Hashtable searchleaves(int roleid, int type, bool ignorewilder, bool ignorewildclass)
        {
            return null;
        }
    }
}