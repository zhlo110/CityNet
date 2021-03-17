using CityNet.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    public class FunctiongroupNode : TreeNode
    {
        public override TreeNode createParent()
        {
            return null;
        }

        public override System.Collections.IList createChildren()
        {
            IList children = new ArrayList();
            //得到子节点
            int id = (int)getValue("ID");
            string sql = "select [ID],[ActionClassName],[Description],[ActionCreateTime] from ActionClass where ParentID = @pid";

            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@pid", id));
            DataSet ds = DBAccess.Query(sql, "ActionClass", list);

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
                        string actiongroupName = row["ActionClassName"].ToString();

                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["ActionCreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");

                        TreeNode node = new FunctiongroupNode();
                        node.setValue("ID", cid);
                        node.setValue("Name", actiongroupName);
                        node.setValue("Leaf", false);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);
                        node.Key = cid; //departmentid
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

        public override string leafjson(IList paramaters)
        {
            return "";
        }

        public override string nodejson(string childrenjson,IList paramaters)
        {
            return "";
        }

        public override TreeNode createNode(int ID)
        {
            TreeNode result = null;
            //当前节点的ID
            int id = ID;
            string sql = "select [ID],[ActionClassName],[Description],[ActionCreateTime] from ActionClass where [ID] = @id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", id));
            //查询department中匹配的字段
            DataSet ds = DBAccess.Query(sql, "ActionClass", list);
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
                        string functiongroupName = row["ActionClassName"].ToString();
                        string description = row["Description"].ToString();
                        DateTime dtime = (DateTime)row["ActionCreateTime"];
                        string createtime = dtime.ToString("yyyy年MM月dd日");

                        TreeNode node = new FunctiongroupNode();
                        node.setValue("ID", id);
                        node.setValue("Name", functiongroupName);
                        node.setValue("Leaf", false);
                        node.setValue("CreateTime", createtime);
                        node.setValue("Description", description);
                        node.Key = id; //departmentid
                        result = node;
                    }
                }
            }
            return result;
        }

        protected override System.Collections.Hashtable searchImp(string pattern, bool includeLeaves)
        {
            return null;
        }

        public override void deletebyids(string ids)
        {

            string sql = "exec('delete from ActionClass where [ID] in ('+@ids+')')";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@ids", ids));
            DBAccess.NoQuery(sql, list);

            sql = "exec('update [Action] set ActionGroupID = 0 where [ActionGroupID] in ('+@ids+')')";
            DBAccess.NoQuery(sql, list);
            //throw new NotImplementedException();
        }



        public override Hashtable searchleaves(int roleid, int type, bool ignorewilder, bool ignorewildclass)
        {
            return null;
        }
    }
}