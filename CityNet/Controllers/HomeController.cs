using CityNet.security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CityNet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            String enuser_pass = Request["params"];
            IList vec = Security.validate(enuser_pass);//验证
            ViewBag.Message = "后台程序出错.";
            ViewBag.Url = "";
            ViewBag.Message = "";
            if (vec.Count >= 3)
            {
                DataTable dt = Security.getUserInformation(vec);
                if (dt != null)
                {
                    DataRow row = dt.Rows[0];
                    string name = row["RealName"].ToString();
                    string department = row["DepartmentName"].ToString();
                    string groupName = row["RealGroupName"].ToString();
                    ViewBag.Url = "../Home/Manager?params=" + System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Params = System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Message = "欢迎您！" + department + "的" + name + "(" + groupName + ")。";
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        public ActionResult Document()
        {
            String enuser_pass = Request["params"];
            String documentid = Request["docid"];
            IList vec = Security.validate(enuser_pass);//验证
            ViewBag.Message = "后台程序出错.";
            ViewBag.Url = "";
            ViewBag.Message = "";
            ViewBag.DocumentID = documentid;
            if (vec.Count >= 3 && documentid != null)
            {
                DataTable dt = Security.getUserInformation(vec);
                if (dt != null)
                {
                    DataRow row = dt.Rows[0];
                    string name = row["RealName"].ToString();
                    string department = row["DepartmentName"].ToString();
                    string groupName = row["RealGroupName"].ToString();
                    ViewBag.Url = "../Home/Manager?params=" + System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Params = System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Message = "欢迎您！" + department + "的" + name + "(" + groupName + ")。";
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        //点详细列表
        public ActionResult PointDetail()
        {
            String enuser_pass = Request["params"];
            String pointid = Request["pointid"];
            IList vec = Security.validate(enuser_pass);//验证
            ViewBag.Message = "后台程序出错.";
            ViewBag.Url = "";
            ViewBag.Message = "";
            ViewBag.PointID = pointid;
            if (vec.Count >= 3 && pointid != null)
            {
                DataTable dt = Security.getUserInformation(vec);
                if (dt != null)
                {
                    DataRow row = dt.Rows[0];
                    string name = row["RealName"].ToString();
                    string department = row["DepartmentName"].ToString();
                    string groupName = row["RealGroupName"].ToString();
                    ViewBag.Url = "../Home/Manager?params=" + System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Params = System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Message = "欢迎您！" + department + "的" + name + "(" + groupName + ")。";
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";
            return View();
        }

        public ActionResult Manager()
        {
            String enuser_pass = Request["params"];
            IList vec = Security.validate(enuser_pass);//验证
            // LogUtility

            ViewBag.Message = "后台程序出错.";
            ViewBag.Params = "";
            if (vec.Count >= 3)
            {
                DataTable dt = Security.getUserInformation(vec);
                if (dt != null)
                {
                    DataRow row = dt.Rows[0];
                    string name = row["RealName"].ToString();
                    string department = row["DepartmentName"].ToString();
                    string groupName = row["RealGroupName"].ToString();
                    ViewBag.Params = System.Web.HttpUtility.UrlEncode(enuser_pass);
                    ViewBag.Message = "欢迎您！" + department + "的" + name + "(" + groupName + ")。";
                    return View();
                }
            }
            return RedirectToAction("Error");
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
