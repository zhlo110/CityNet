using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.function
{
    /// <summary>
    /// build_all_function_tree 的摘要说明
    /// </summary>
    public class build_all_function_tree : Security
    {
        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            string node = context.Request["node"];
            if (node == null || node == "new_item_id")
            {
                context.Response.Write(getErrorMessage());
            }
            else
            {
                int i;
                string roleid = context.Request["roleid"];
                int iroleid = 0;
                if (roleid != null)
                {
                    int.TryParse(roleid, out iroleid);
                }

                TreeNode pNode = new FunctionClassNode();
                IList tree = pNode.createTree(-1, -1, true,true);
                IList paramaters = new ArrayList();
                paramaters.Add(true); //是否显示叶子节点
                paramaters.Add(true);//是否显示checkbox
                paramaters.Add(roleid);
                String writeret = "";
                int nCount = tree.Count;
                for (i = 0; i < nCount; i++)
                {
                    TreeNode root = (TreeNode)tree[i];
                    string nodejson = root.toJson(paramaters);
                    if (nodejson.Length > 0)
                    {
                        writeret += nodejson + ",";
                    }
                }
                if (writeret.Length > 0)
                {
                    writeret = writeret.Substring(0, writeret.Length - 1);
                }
                writeret = "[" + writeret + "]";
                context.Response.Write(writeret);
            }


            /*
            string node = context.Request["node"];
            if (node == null || node == "new_item_id")
            {
                context.Response.Write(getErrorMessage());
            }
            else
            {
                int i;
                string roleid = context.Request["roleid"];
                int iroleid = 0;
                if (roleid != null)
                {
                    int.TryParse(roleid, out iroleid);
                }

                IList leaflist = new ArrayList();
                IList tree = FunctionTreeUtility.createFullFucTree(leaflist, true, iroleid, false);
                int nCount = tree.Count;
                String writeret = "";

                for (i = 0; i < nCount; i++)
                {
                    FunctionTreeNode root = (FunctionTreeNode)tree[i];
                    writeret += root.toJson(false, false, false, true) + ",";
                }

                IList wildlist = FunctionTreeUtility.createWildFuncLeaves(iroleid);
                nCount = wildlist.Count;
                for (i = 0; i < nCount; i++)
                {
                    FunctionTreeNode root = (FunctionTreeNode)wildlist[i];
                    writeret += root.toJson(false, false, false, true) + ",";
                }

                if (writeret.Length > 0)
                {
                    writeret = writeret.Substring(0, writeret.Length - 1);
                }
                // writeret = "[{\"text\":'项目根目录',\"id\":'root',\"qtip\":'root',\"leaf\": false,\"expanded\":'true',\"cls\":'folder',\"children\":[" + writeret + "]}]";
                writeret = "[" + writeret + "]";
                context.Response.Write(writeret);
            }*/
        }
       
    }
}