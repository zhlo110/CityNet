using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class FunctionClassNode : TreeNode
    {
        public override void deletebyids(string ids)
        {
            return;
        }

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
                string sql = "select * from ActionClass where [ID] = @id";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@id", parentid));
                 DataSet ds = DBAccess.Query(sql, "Action", list);
                 if (ds != null)
                 {
                     DataTable dt = ds.Tables[0];
                     if (dt != null)
                     {
                         if (dt.Rows.Count > 0)
                         {
                             DataRow row = dt.Rows[0];
                             FunctionClassNode parent = new FunctionClassNode();
                             parent.setValue("Leaf", false);
                             parent.setValue("ID",row["ID"]);
                             parent.setValue("Name", row["ActionClassName"]);
                             parent.setValue("ParentID", row["ParentID"]);
                             parent.setValue("Description", row["Description"]);
                             parent.setValue("Priority", row["Priority"]);

                             parent.setValue("CreateTime", row["ActionCreateTime"]);
                             parent.setValue("ActionUrl", "");
                             parent.setValue("CreateID", row["CreateID"]);
                             parent.setValue("ActionType", row["ActionType"]);
                             parent.Key = "FunctionTreeNode_Folder_" + row["ID"].ToString();
                             return parent;
                         }
                     }
                 }
                 return null;
            }
        }

        public override System.Collections.IList createChildren()
        {
            return null;
        }

        public override System.Collections.IList createChildrenLeaves()
        {
            return null;
        }

        public override bool isLeaf()
        {
            return (bool)getValue("Leaf");
        }
        //paramaters[0] 是否显示叶子节点
        //paramaters[1] 是否显示checkbox
        public override string leafjson(IList paramaters)
        {
            bool showcheckbox = (bool)paramaters[1];
            string mActionName = this.getValue("Name").ToString();
            string mID = this.getValue("ID").ToString();
            string isleaf = "true";
            string mUrl = this.getValue("ActionUrl").ToString();
            string mCreateID = this.getValue("CreateID").ToString();
            string mDescription = this.getValue("Description").ToString();
            string priority = getValue("Priority").ToString();
            string actiontype = getValue("ActionType").ToString();
            DateTime dt = (DateTime)getValue("CreateTime");
            string mCreateDate = dt.ToString("yyyy年MM月dd日");
            int iuserid = -1;
            if (!int.TryParse(mCreateID, out iuserid))
            {
                iuserid = -1;
            }
            string mCreater = LogUtility.getRealName(iuserid);
            string groupid = paramaters[2].ToString();
            string schecked = "";
            //   if (groupid.Trim().Equals())
            if (rolehasfunction(groupid, mID))
            {
                schecked = ",\"checked\":true";
            }
            else
            {
                schecked = ",\"checked\":false";
            }
            if (!showcheckbox)
                schecked = "";
            string ret = "{\"text\":'" + mActionName + "',\"actiontype\":" + actiontype.ToString() + ",\"priority\":"+priority+",\"projectName\":'" + mActionName + "'," + "\"id\":'" + Key.ToString() + "',\"qtip\":" + mID.ToString() + ",\"leaf\": "
                        + isleaf + ",\"url\":'" + mUrl + "',\"provider\":'',\"creater\":'" + mCreater + "',\"createID\":'"
                        + mCreateID.ToString() + "',\"createtime\":'" + mCreateDate + "',\"description\":'"
                        + mDescription + "'" + schecked + "}";

            return ret;
           // return null;
        }

        public bool rolehasfunction(string groupid,string functionid)
        {
            int igroupid = -1;
            int ifunctionid = -1;
            bool suc1 = int.TryParse(groupid,out igroupid);
            bool suc2 = int.TryParse(functionid, out ifunctionid);
            if(suc1&&suc2)
            {
                string sql = "select count([ID]) from Action_Group_View where ID = @id and GroupID =@gid";
                IList list = new ArrayList();
                list.Add(new DictionaryEntry("@id", ifunctionid));
                list.Add(new DictionaryEntry("@gid", igroupid));
                if (DBAccess.QueryStatistic(sql, list) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override string nodejson(string childrenjson,IList paramaters)
        {
            bool showcheckbox = (bool)paramaters[1];
            string mActionName = this.getValue("Name").ToString();
            string mID = this.getValue("ID").ToString();
            string isleaf = "false";
            string mUrl = this.getValue("ActionUrl").ToString();
            string mCreateID = this.getValue("CreateID").ToString();
            string mDescription = this.getValue("Description").ToString();
            DateTime dt = (DateTime)getValue("CreateTime");
            string actiontype = getValue("ActionType").ToString();
            string mCreateDate = dt.ToString("yyyy年MM月dd日");
            string priority = getValue("Priority").ToString();
            int iuserid = -1;
            if (!int.TryParse(mCreateID, out iuserid))
            {
                iuserid = -1;
            }
            string mCreater = LogUtility.getRealName(iuserid);

            string groupid = paramaters[2].ToString();
            string schecked = "";
            string ret = "{\"text\":'" + mActionName + "',\"actiontype\":" + actiontype + ",\"priority\":" + priority 
                    + ",\"projectName\":'" + mActionName + "'," + "\"id\":'" + Key.ToString() + "',\"qtip\":" + mID.ToString() + ",\"leaf\": "
                    + isleaf + ",\"expanded\":'true',\"cls\":'folder',\"creater\":'" + mCreater + "',\"createID\":'"
                    + mCreateID.ToString() + "',\"createtime\":'" + mCreateDate + "',\"description\":'"
                    + mDescription + "'," + schecked + "\"children\":[" + childrenjson + "]}";

            return ret;
        }

        public override TreeNode createNode(int ID)
        {
            return null;
        }

        protected override System.Collections.Hashtable searchImp(string pattern, bool includeLeaves)
        {
            return null;
          //  throw new NotImplementedException();
        }


        public override Hashtable searchleaves(int roleid, int type, bool ignorewilder, bool ignorewildclass)
        {
            Hashtable retlist = new Hashtable();
            string sql = "";
            string rolecondition = "";
            string typecondition = "";
            IList list = new ArrayList();
            
            if (roleid > 0)
            {
                rolecondition = " [GroupID]=@gid ";
                list.Add(new DictionaryEntry("@gid", roleid));
            }
            else
            {
                rolecondition = " 1=1 ";
            }
            if (type > 0)
            {
                typecondition = " [ActionType]=@type ";
                list.Add(new DictionaryEntry("@type", type));
            }
            else
            {
                typecondition = " 1=1 ";
            }

            sql = "select DISTINCT [ID],[ActionName],[ActionUrl],[ActionGroupID],[ActionCreateTime],[Priority],[acpriority]," +
                  "[Description],[CreateID] from Action_Group_View where" + rolecondition + "and" + typecondition;

            if (ignorewilder)
            {
                sql += " and ActionGroupID is not NULL and ActionGroupID in (select [ID] from ActionClass)";
            }
            sql += " order by [acpriority]";
            DataSet ds = DBAccess.Query(sql, "Action", list);
            if (ds != null)
            {
                DataTable dt = ds.Tables[0];
                if (dt != null)
                {
                    int i;
                    int nCount = dt.Rows.Count;
                    for (i = 0; i < nCount; i++ )
                    {
                        DataRow row = dt.Rows[i];
                        //功能节点，都是叶子节点
                        FunctionClassNode leaf = new FunctionClassNode();
                        leaf.setValue("ID",row["ID"]);
                        leaf.setValue("Name", row["ActionName"]);
                        leaf.setValue("Priority", row["Priority"]);

                        leaf.setValue("CreateTime", row["ActionCreateTime"]);
                        leaf.setValue("ActionUrl", row["ActionUrl"]);
                        leaf.setValue("Description", row["Description"]);
                        leaf.setValue("CreateID", row["CreateID"]);
                        leaf.setValue("ParentID", row["ActionGroupID"]);
                        leaf.setValue("ActionType", "-1");
                        string sID = row["ID"].ToString();
                        leaf.setValue("Leaf", true);
                        leaf.Key = "FunctionTreeNode_Leaf_" + row["ID"].ToString();

                        TreeNode parent = leaf.createParent();
                        if (parent == null)
                        {
                            leaf.setValue("ParentID", -1);
                            leaf.Parent = null;
                            retlist.Add(leaf.Key,leaf);
                        }
                        else
                        {
                            if (!retlist.ContainsKey(parent.Key))
                            {
                                retlist.Add(parent.Key, parent);
                            }
                            else
                            {
                                parent = (TreeNode)retlist[parent.Key];
                            }
                            leaf.Parent = parent;
                            parent.Children.Add(leaf);
                        }
                    }
                }
            }

            if (!ignorewildclass)
            {
                sql = "select * from ActionClass where [ID] not in (select distinct [ActionGroupID] from Action_Group_View) order by [Priority]";
                ds = DBAccess.Query(sql, "ActionClass", list);
                if (ds != null)
                {
                     DataTable dt = ds.Tables[0];
                     if (dt != null)
                     {
                         int i;
                         int nCount = dt.Rows.Count;
                         for (i = 0; i < nCount; i++)
                         {
                             DataRow row = dt.Rows[i];
                             FunctionClassNode wildnode = new FunctionClassNode();
                             wildnode.setValue("Leaf", false);
                             wildnode.setValue("ID", row["ID"]);
                             wildnode.setValue("Name", row["ActionClassName"]);
                             wildnode.setValue("ParentID", row["ParentID"]);
                             wildnode.setValue("Description", row["Description"]);
                             wildnode.setValue("Priority", row["Priority"]);

                             wildnode.setValue("CreateTime", row["ActionCreateTime"]);
                             wildnode.setValue("ActionUrl", "");
                             wildnode.setValue("CreateID", row["CreateID"]);
                             wildnode.setValue("ActionType", row["ActionType"]);
                             wildnode.Key = "FunctionTreeNode_Folder_" + row["ID"].ToString();
                             retlist.Add(wildnode.Key,wildnode);
                         }
                     }
                 }
            }

            return retlist;
        }
    }
}