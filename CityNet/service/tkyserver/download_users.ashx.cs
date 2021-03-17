using Newtonsoft.Json;
using CityNet.Controllers;
using CityNet.Models;
using CityNet.security;
using CityNet.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.service.tkyserver
{
    /// <summary>
    /// download_users 的摘要说明
    /// </summary>
    public class download_users : Security
    {
        protected override void fuctionimp(HttpContext context, System.Collections.IList parameters)
        {
            string url = ServerConfig.mDownloadUserUrl;
            string taskid = context.Request["taskid"];
            string json = HttpUitls.Get(url);
            string username = parameters[0].ToString();
            string password = parameters[1].ToString();
            int createuserid = LogUtility.GetUserID(username, password);

            if (json != null && !json.Trim().Equals("[]"))
            {
                IList<UserClass> userList = new List<UserClass>();
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;

                userList = JsonConvert.DeserializeObject<IList<UserClass>>(json, jsetting);
                int nCount = userList.Count;

                int i;
                string usersql = "";
                usersql = "select count([ID]) from [User] where [ID]=@id";
                string departmentsql = "select count([ID]) from Department where [ID]=@did";
                //

                string insert_user_sql = "insert [User]([ID],[PassWord],[UserName],[RealName],[PhoneNumber],[tkyUserType],[Position],[CreateTime],[LogTime],[State],[Sex],[AuditID],[Description]) " +
                    "values(@id,@pw,@un,@rn,@pn,@tut,@pos,@ctime,@logtime,@state,@sex,@adid,@des)";
                string update_user_sql = "update [User] set "
                                          + "[PassWord]=@pw,"
                                          + "[UserName]=@un,"
                                          + "[RealName]=@rn,"
                                          + "[PhoneNumber]=@pn,"
                                          + "[tkyUserType]=@tut,"
                                          + "[Position]=@pos,"
                                          + "[CreateTime]=@ctime,"
                                          + "[LogTime]=@logtime,"
                                          + "[State]=@state,"
                                          + "[Sex]=@sex,"
                                          + "[AuditID]=@adid,"
                                          + "[Description]=@des where [ID] = @id";
                string has_rel_sql = "select count([ID]) from User_Department where [UserID] = @uid and [DepartmentID] = @did";
                string insert_rel_sql = "insert User_Department([UserID],[DepartmentID]) values(@uid,@did)";
                IList list = new ArrayList();


                initProgressBar(taskid, nCount);
                for (i = 0; i < nCount; i++)
                {
                    updateProgress(taskid, i);
                    UserClass uc = userList[i];
                    uc.trim();
                    list.Clear();
                    list.Add(new DictionaryEntry("@id", uc.ID));
                    string sql = "";
                    if (DBAccess.QueryStatistic(usersql, list) > 0) //已经有了User
                    {
                        sql = update_user_sql; //更新
                    }
                    else //没有User
                    {
                        sql = insert_user_sql; //插入
                    }
                    list.Clear();
      
                    list.Add(new DictionaryEntry("@id", uc.ID));
                    list.Add(new DictionaryEntry("@pw", ServerConfig.mDefaultPassWord));
                    list.Add(new DictionaryEntry("@un", uc.Account));
                    list.Add(new DictionaryEntry("@rn", uc.Name));
                    list.Add(new DictionaryEntry("@pn", uc.Mobile));
                    list.Add(new DictionaryEntry("@tut", uc.UserType));
                    list.Add(new DictionaryEntry("@pos", uc.ProName));
                    list.Add(new DictionaryEntry("@ctime", DateTime.Now));

                    list.Add(new DictionaryEntry("@logtime", 0));
                    list.Add(new DictionaryEntry("@state", 0));

                    list.Add(new DictionaryEntry("@sex", 1));
                    list.Add(new DictionaryEntry("@adid",0));

                    list.Add(new DictionaryEntry("@des", ""));
                    

                    DBAccess.NoQuery(sql, list); //插入或更新了
                    
                    list.Clear();
                    int did = uc.DepartmentId;
                    list.Add(new DictionaryEntry("@did", did));
                    if (DBAccess.QueryStatistic(departmentsql, list) > 0) //有部门
                    {
                        list.Clear();
                        //部门和用户是否建立了关系
                        list.Add(new DictionaryEntry("@uid", uc.ID));
                        list.Add(new DictionaryEntry("@did", did));
                        if (DBAccess.QueryStatistic(has_rel_sql, list) <= 0) //未建立关系
                        {
                            DBAccess.NoQuery(insert_rel_sql,list); //建立关系
                        }
                    }
                    //下载角色，并建立用户和角色的关系
                    DownloadRole(uc.ID, createuserid);

                   // list.Add(new DictionaryEntry("@name", dc.Name));
                    //
                }
            }
            deleteProgress(taskid);
            returnInfo(context, "更新成功！");
            //throw new NotImplementedException();
        }

        private void DownloadRole(int userid,int createid)
        {
            string url = ServerConfig.mGetRoleUrl;
            url = url.Replace("{userID}", userid.ToString());
            string json = HttpUitls.Get(url);
            string rolecount = "select count(ID) from [Group] where tkyID = @tid";
            string insertrole = "insert [Group](GroupName,Description,CreateID,RealGroupName,tkyID) values(@gn,@des,@cid,@rgn,@tid)";
            string updaterole = "update [Group] set GroupName=@gn,Description=@des,CreateID=@cid,RealGroupName=@rgn where tkyID=@tid";
            string has_usr_group_rel = "select count(UG.ID) from User_Group UG,[Group] G where UG.GroupID = G.ID and G.tkyID=@tid and UG.UserID = @uid";
            string insert_rel = "insert User_Group (UserID,GroupID) values(@uid,(select G.ID from [Group] G where G.tkyID=@tid))";
            IList list = new ArrayList();
            if (json != null && !json.Trim().Equals("[]"))
            {
                IList<ShareRole> RoleList = new List<ShareRole>();
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.NullValueHandling = NullValueHandling.Ignore;

                RoleList = JsonConvert.DeserializeObject<IList<ShareRole>>(json, jsetting);
                int nCount = RoleList.Count;
                int i;

                for (i = 0; i < nCount; i++ )
                {
                    ShareRole role = RoleList[i];
                    role.trim();
                    //删除角色
                    list.Clear();
                    list.Add(new DictionaryEntry("@tid", role.ID));
                    string sql = "";
                    if (DBAccess.QueryStatistic(rolecount, list) > 0)
                    {
                        sql = updaterole;
                    }
                    else
                    {
                        sql = insertrole;
                    }

                    //添加角色
                    list.Clear();
                    list.Add(new DictionaryEntry("@gn", role.Name));
                    list.Add(new DictionaryEntry("@des", role.Content));
                    list.Add(new DictionaryEntry("@cid", createid));
                    list.Add(new DictionaryEntry("@rgn", role.Name));
                    list.Add(new DictionaryEntry("@tid", role.ID));
                    DBAccess.NoQuery(sql, list);

                    //判断是否存在
                    list.Clear();
                    list.Add(new DictionaryEntry("@tid", role.ID));
                    list.Add(new DictionaryEntry("@uid", userid));
                    if (DBAccess.QueryStatistic(has_usr_group_rel, list) <= 0) //不存在关联
                    {
                        //插入关联
                        DBAccess.NoQuery(insert_rel, list);
                    }
                }
            }
        }

        protected override string getErrorMessage()
        {
            return "{success:0,msg:'用户名和密码错误！'}";
        }

        protected override int getErrorCode()
        {
            return 500;
        }
    }
}