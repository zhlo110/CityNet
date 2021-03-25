using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{

    public class SessionNode : TreeNode
    {
        //批量删除点
        public override void deletebyids(string ids)
        {
            return;
        }
        //查找并创建父节点，如果父节点为空表示为根节点
        public override TreeNode createParent()
        {
            int parentid = -1;
            Object oparentid = this.getValue("ParentID");

            if (oparentid == null)
            {
                return null;
            }
            else
            {
                if (!int.TryParse(oparentid.ToString(), out parentid))
                {
                    parentid = -1;
                }
                string sql = "select * from Project_User_View where [ID] = @id";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@id", parentid));
                DataSet ds = DBAccess.Query(sql, "Project_User_View", list);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            SessionNode parent = new SessionNode();
                            parent.setValue("Leaf", false);
                            parent.setValue("ID", DatabaseUtility.getIntValue(row, "ID", -1));
                            parent.setValue("Name", DatabaseUtility.getStringValue(row, "Name"));
                            parent.setValue("Type", DatabaseUtility.getStringValue(row, "Type"));
                            parent.setValue("Description", DatabaseUtility.getStringValue(row, "Description"));
                            parent.setValue("Creator", DatabaseUtility.getStringValue(row, "UserName"));
                            parent.setValue("CreateTime", DatabaseUtility.getDatetimeValue(row, "CreateTime"));
                            parent.setValue("CreatorID", DatabaseUtility.getIntValue(row, "CreatorID", 0));
                            parent.setValue("ParentID", DatabaseUtility.getIntValue(row, "ParentID", 0));

                            parent.Key = "SessionTreeNode_" + DatabaseUtility.getIntValue(row, "ID", -1).ToString();
                            return parent;
                        }
                    }
                }
                return null;
            }
        }

        public override System.Collections.IList createChildren()
        {
            IList children = new ArrayList();
            //得到子节点
            int id = (int)getValue("ID");
            string sql = "select * from Project_User_View where [ParentID] = @pid";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", id));
            DataSet ds = DBAccess.Query(sql, "Project_User_View", list);

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
                        SessionNode node = new SessionNode();
                        node.setValue("Leaf", false);
                        node.setValue("ID", DatabaseUtility.getIntValue(row, "ID", -1));
                        node.setValue("Name", DatabaseUtility.getStringValue(row, "Name"));
                        node.setValue("Type", DatabaseUtility.getStringValue(row, "Type"));
                        node.setValue("Description", DatabaseUtility.getStringValue(row, "Description"));
                        node.setValue("Creator", DatabaseUtility.getStringValue(row, "UserName"));
                        node.setValue("CreateTime", DatabaseUtility.getDatetimeValue(row, "CreateTime"));
                        node.setValue("CreatorID", DatabaseUtility.getIntValue(row, "CreatorID", 0));
                        node.setValue("ParentID", DatabaseUtility.getIntValue(row, "ParentID", 0));
                        node.Key = "SessionTreeNode_" + DatabaseUtility.getIntValue(row, "ID", -1).ToString();
                        children.Add(node);
                    }
                }
            }
            return children;
        }

        public override System.Collections.IList createChildrenLeaves()
        {
            return null;
        }

        public override bool isLeaf()
        {
            return (bool)getValue("Leaf");
        }

        public override string leafjson(System.Collections.IList parameter)
        {
            return "";
        }

        public override string nodejson(string childrenjson, System.Collections.IList paramaters)
        {
            string Name = this.getValue("Name").ToString();
            string ID = this.getValue("ID").ToString();
            string isleaf = "false";
            string Type = this.getValue("Type").ToString();
            string Description = this.getValue("Description").ToString();
            string UserName = this.getValue("Creator").ToString();
            string CreateTime = this.getValue("CreateTime").ToString();
            string ParentID = this.getValue("ParentID").ToString();
            string CreatorID = this.getValue("CreatorID").ToString();
            bool expand = (bool)paramaters[1];
            bool checkbox = false;
            int taskid = -1;
            if (paramaters.Count >= 4)
            {
                checkbox = (bool)paramaters[2];
                taskid = (int)paramaters[3];
            }

  
            int iuserid = -1;
            if (!int.TryParse(CreatorID, out iuserid))
            {
                iuserid = -1;
            }
            string mCreater = UserName;
            string sexpand = "false";
            if (expand)
            {
                sexpand = "true";
            }
            string schildren = "";
            if (expand)
            {
                schildren = ",children:[" + childrenjson + "]";
            }
            string scheckbox = "";
            if (checkbox)
            {
                int state = getState(ID,taskid);
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

                scheckbox = ",indeterminate:" + indeterminate + ",checked:" + check;
            }
       

            string ret = "{text:'" + Name + "'" + ",parentid:" + ParentID
                    + ",sessiontype:'" + Type + "'," + "id:" + ID.ToString() + ",qtip:" + ID.ToString() + ",leaf: "
                    + isleaf + ",expanded:" + sexpand + ",cls:'folder',creator:'" + mCreater + "',createID:"
                    + CreatorID.ToString() + ",createtime:'" + CreateTime + "',description:'"
                    + Description + "'" + scheckbox + schildren + "}";

            return ret;
        }

        private int getState(string projectid, int taskid)
        {
            int state = -1;
            string sql = "select State from Session_Task where ProjectID = @prjid and TaskID=@taskid";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@prjid", projectid));
            list.Add(new DictionaryEntry("@taskid", taskid));
            DataSet ds = DBAccess.Query(sql, "Session_Task", list);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    int nCount = dt.Rows.Count;
                    if (nCount > 0)
                    {
                        DataRow row = dt.Rows[0];
                        state = DatabaseUtility.getIntValue(row, "State", -1);
                    }
                }
            }
            return state;
        }

        public override TreeNode createNode(int ID)
        {
            TreeNode result = new SessionNode();
            result.setValue("Leaf", false);
            result.setValue("ID", ID);
            result.setValue("ParentID", 0);
            result.Key = "SessionTreeNode_0";

            if (ID > 0)
            {
                string sql = "select * from Project_User_View where [ID] = @id";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@id", ID));
                DataSet ds = DBAccess.Query(sql, "Project_User_View", list);
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt != null)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            SessionNode node = new SessionNode();
                            node.setValue("Leaf", false);
                            node.setValue("ID", DatabaseUtility.getIntValue(row, "ID", -1));
                            node.setValue("Name", DatabaseUtility.getStringValue(row, "Name"));
                            node.setValue("Type", DatabaseUtility.getStringValue(row, "Type"));
                            node.setValue("Description", DatabaseUtility.getStringValue(row, "Description"));
                            node.setValue("Creator", DatabaseUtility.getStringValue(row, "UserName"));
                            node.setValue("CreateTime", DatabaseUtility.getDatetimeValue(row, "CreateTime"));
                            node.setValue("CreatorID", DatabaseUtility.getIntValue(row, "CreatorID", 0));
                            node.setValue("ParentID", DatabaseUtility.getIntValue(row, "ParentID", 0));

                            node.Key = "SessionTreeNode_" + DatabaseUtility.getIntValue(row, "ID", -1).ToString();
                            result = node;
                        }
                    }
                }
            }
            return result;

        }

        public override System.Collections.Hashtable searchleaves(int roleid, int type, bool ignorewilder, bool ignorewildclass)
        {
            return null;
        }

        protected override System.Collections.Hashtable searchImp(string pattern, bool includeLeaves)
        {
            Hashtable result = new Hashtable();
            Hashtable leavestable = new Hashtable();
            string sql = "select * from Project_User_View where [Name] like '%'+@seachtext+'%'";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@seachtext", pattern));
            //查询department中匹配的字段
            DataSet ds = DBAccess.Query(sql, "Project_User_View", list);
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
                        SessionNode node = new SessionNode();
                        node.setValue("Leaf", false);
                        node.setValue("ID", DatabaseUtility.getIntValue(row, "ID", -1));
                        node.setValue("Name", DatabaseUtility.getStringValue(row, "Name"));
                        node.setValue("Type", DatabaseUtility.getStringValue(row, "Type"));
                        node.setValue("Description", DatabaseUtility.getStringValue(row, "Description"));
                        node.setValue("Creator", DatabaseUtility.getStringValue(row, "UserName"));
                        node.setValue("CreateTime", DatabaseUtility.getDatetimeValue(row, "CreateTime"));
                        node.setValue("CreatorID", DatabaseUtility.getIntValue(row, "CreatorID", 0));
                        node.setValue("ParentID", DatabaseUtility.getIntValue(row, "ParentID", 0));
                        node.Key = "SessionTreeNode_" + DatabaseUtility.getIntValue(row, "ID", -1).ToString();

                        result.Add(node.Key, node);
                    }
                }
            }
            return result;
        }
    }
}