using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityNet.Utility
{
    //铁科院配置文件
    public class ServerConfig
    {
        //基础的url
        public static string mServerUrl = "http://10.10.60.14:9300/servicesapi/";
        //全国7大区数据获取
        public static string mSevenArea = mServerUrl + "categoryItems/getCategoryItems";

        //根据片区获取该区项目信息
        public static string mProjectInfoUrl = mServerUrl + "categoryItems/getProjectInfoItemId/";

        //根据项目ID获取该区标段
        public static string mProjectSectionUrl = mServerUrl + "projectInfo/getProjectSectionByInfoId/";
        
        //根据标段的ID获取工区的信息
        public static string mProjectAreaUrl = mServerUrl + "projectSection/getProjectAreaList/";

        //根据标段的ID获取工点的信息
        public static string mProjecSiteUrl = mServerUrl + "projectSection/getProjectSiteList/";


        //获取所有部门数据
        public static string mDownloadDepartmentUrl = mServerUrl + "departments/getAllDeptByPage/";

        public static string mApplicationName = "f7f7857f14"; //应用凭证
        public static string mApplicationPassword = "8d3358935e1f7f7857f142de10e83744"; //应用秘钥

        //根据应用凭证和应用秘钥获取
        public static string mDownloadUserUrl = mServerUrl + "users/getAllUserInfosBySysMenu/" + mApplicationName + "/" + mApplicationPassword;

        public static string mDefaultPassWord = "Zjj20130711";


        //用户能看到的项目、工区和标段
        //用户能看到的项目
        public static string mUserVisibleProjectUrl = mServerUrl + "users/getProjectinfoByUserId/";
        //用户能看到的标段
        public static string mUserVisibleSectionUrl = mServerUrl + "users/getProjectSectionByUserId/";
        //用户能看到的工区
        public static string mUserVisibleProjectAreaUrl = mServerUrl + "users/getProjectAreaByUserId/";
        //根据用户ID应用凭证和应用秘钥获取角色信息
        public static string mGetRoleUrl = mServerUrl + "users/getShareRoleByUserId/{userID}/" + mApplicationName + "/" + mApplicationPassword;


        //获取用户
        //根据项目ID获取该项目下所有的用户
        public static string mGetUserByProject = mServerUrl + "projectInfo/getUserInfosByProjectInfoId/";
        //根据标段ID获取该标段下所有的用户
        public static string mGetUserBySection = mServerUrl + "projectSection/getUserInfoList/";
        //根据工区ID获取该工区下所有的用户
        public static string mGetUserByArea = mServerUrl + "projectarea/getUserInfoListById/";

        public class ProjectType
        {
            public static String ALLAREA = "AllArea"; //项目
            public static String PROJECT = "Project"; //项目
            public static String SECTION = "Section"; //标段
            public static String AREA = "Area"; //工区
        }

        public static string getuser_url(string type)
        {
            string url = "";
            type = type.Trim();
            if (type.Equals(ProjectType.PROJECT))//项目
            {
                url = mGetUserByProject;
            }
            else if (type.Equals(ProjectType.SECTION))//标段
            {
                url = mGetUserBySection;
            }
            else if (type.Equals(ProjectType.AREA))//工区
            {
                url = mGetUserByArea;
            }
            return url;
        }

    }
}