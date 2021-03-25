using CityNet.Controllers;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.projectsession
{
    /// <summary>
    /// delete_sesssion_byid 的摘要说明
    /// </summary>
    public class delete_sesssion_byid : Security
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
            //判断功能组中有没有功能

            
            String sessionid = context.Request["sessionid"];
            int isessionid = -1;
            if (!int.TryParse(sessionid, out isessionid))
            {
                isessionid = -1;
            }
            TreeNode pNode = new SessionNode();
            pNode = pNode.createNode(isessionid);

            //找所有的子节点
            Hashtable allchildren = new Hashtable();
            pNode.TraceChildren(allchildren,999);

            deletesinglenode(isessionid);
            foreach (string key in allchildren.Keys)
            {
                pNode = allchildren[key] as TreeNode;
                int ID =  (int)pNode.getValue("ID");
                deletesinglenode(ID);
            }
            returnInfo(context,"删除成功");
        }

        private void deletesinglenode(int id)
        {
            //删除ProjectSession表
            string sql = "delete from [Project_Session] where ID=@id";
            IList list = new ArrayList();
            list.Add(new DictionaryEntry("@id", id));
            DBAccess.NoQuery(sql, list);
            //删除session_Task表
            sql = "delete from [Session_Task] where ProjectID=@id";
            DBAccess.NoQuery(sql, list);
        }
    }
}