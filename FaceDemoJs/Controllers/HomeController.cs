using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;

namespace FaceDemoJs.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            if (Session["AccessToken"] != null)
            {
                var accessToken = Session["AccessToken"].ToString();
                var client = new FacebookClient(accessToken);
                dynamic result = client.Get("me/friends");
                ViewBag.Usuario = result.ToString();
            }

            return View();
        }

        public JsonResult Login()
        {
            var accessToken =Request["accessToken"];
            Session["AccessToken"] = accessToken;

            return Json(true);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
