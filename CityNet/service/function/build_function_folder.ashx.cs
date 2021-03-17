using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.function
{
    /// <summary>
    /// build_function_folder 的摘要说明
    /// </summary>
    public class build_function_folder : Security
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
            //创建功能树，不包含叶子节点

            TreeNode pNode = new FunctionClassNode();
            IList tree = pNode.createTree(-1, -1, true,false);
            IList paramaters = new ArrayList();
            paramaters.Add(false); //是否显示叶子节点
            paramaters.Add(false);//是否显示checkbox
            paramaters.Add(-1);//是否显示checkbox
            String writeret = "";
            int i;
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
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //创建功能树，不包含叶子节点
            string node = context.Request["node"];
            if (node == null || node == "new_item_id")
            {
                context.Response.Write(getErrorMessage());
            }
            else
            {
                IList leaflist = new ArrayList();
                IList tree = FunctionTreeUtility.createFullFucTree(leaflist, true, 0, false);
                int nCount = tree.Count;
                String writeret = "";
                int i;

                for (i = 0; i < nCount; i++)
                {
                    FunctionTreeNode root = (FunctionTreeNode)tree[i];
                    writeret += root.toJson(false, true, false, false) + ",";
                }
                if (writeret.Length > 0)
                {
                    writeret = writeret.Substring(0, writeret.Length - 1);
                }
                // writeret = "[{\"text\":'项目根目录',\"id\":'root',\"qtip\":'root',\"leaf\": false,\"expanded\":'true',\"cls\":'folder',\"children\":[" + writeret + "]}]";
                writeret = "[" + writeret + "]";
                context.Response.Write(writeret);
            }
        }*/
    }
}