using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.user
{
    /// <summary>
    /// build_function_tree 的摘要说明
    /// </summary>
    public class build_function_tree : Security
    {
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //根据用户ID决定管理树左侧内容
            //Type 要为1
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int userid = LogUtility.GetUserID(username, password);
            IList paramaters = new ArrayList();
            paramaters.Add(true); //是否显示叶子节点
            paramaters.Add(false);//是否显示checkbox
            paramaters.Add(-1);
            string type = context.Request["funtype"];
            int itype = 1;
            if (!int.TryParse(type, out itype))
            {
                itype = 1;
            }
            
            if (userid > 0) //数据验证成功
            {
                //找groupID
                int groupID = LogUtility.GetGroupIDByUserID(userid);
                if (groupID > 0) //找GroupID
                {
                    TreeNode pNode = new FunctionClassNode();
                    IList tree = pNode.createTree(groupID, itype, true, true);

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
                else
                {
                    context.Response.Write(getErrorMessage());
                }
            }
            else
            {
                context.Response.Write(getErrorMessage());
            }
        }

        protected override string getErrorMessage()
        {
            return "[]";
        }
        protected override int getErrorCode()
        {
            return 200;
        }

      
    }
}