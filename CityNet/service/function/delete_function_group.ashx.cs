using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityNet.Controllers;
using CityNet.Utility;
using CityNet.security;

namespace CityNet.service.function
{
    /// <summary>
    /// delete_function_group 的摘要说明
    /// </summary>
    public class delete_function_group : Security
    {
        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误，请重新登录.'}";
        }
        protected override int getErrorCode()
        {
            return 500;
        }
        protected override void fuctionimp(HttpContext context, IList parameters)
        {
            //删除功能组
            String function_id = context.Request["funid"];
            char[] splitc = new char[] { '_' };
            string[] vec2 = function_id.Split(splitc);
         //   string sql = "delete from ActionClass where [ID]= @id";
          //  IList list = new ArrayList();
         //   list.Add(new DictionaryEntry("@id", vec2[2]));

            string sid = vec2[2];
            int id = -1;
            if (int.TryParse(sid, out id))
            {
                TreeNode treeNode = new FunctiongroupNode();
                treeNode = treeNode.createNode(id);
                if (treeNode != null)
                {
                    treeNode.deleteNode(999);//删除treenode中的所有点，包括自己
                    returnInfo(context, "删除功能组成功");

                }
                else
                {
                    returnErrorInfo(context, "没有查询到该功能组！");
                }
            }
            else
            {
                returnErrorInfo(context, "功能组ID解析出错!");
            }


         //   DBAccess.NoQuery(sql, list);
        //    sql = "update [Action] set ActionGroupID = 0 where ActionGroupID=@id";
         //   DBAccess.NoQuery(sql, list);

       //     context.Response.Write("{success:1,msg:'删除节点成功.'}");
        }
    }
}