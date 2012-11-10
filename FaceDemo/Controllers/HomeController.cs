using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;

namespace FaceDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            if (Session["accessToken"] != null)
            {
                var accessToken = Session["accessToken"].ToString();

                //Exemplo de uso
                var client = new FacebookClient(accessToken);
                client.Post("me/feed", new { message = form["Mensagem"], link = form["Link"] });

                ViewBag.MensagemSucesso = "Enviado com sucesso";
                ViewBag.MostraForm = true;

                dynamic me = client.Get("me");
                ViewBag.Usuario = me.ToString();

            }

            return View();
        }


        public ActionResult Login()
        {
            dynamic parameters = new ExpandoObject();
            parameters.client_id = ConfigurationManager.AppSettings["FacebookAppId"];
            parameters.redirect_uri = "http://localhost:2780/home/LoginRetorno";
            parameters.response_type = "code";
            
            parameters.display = "popup";

            var extendedPermissions = "user_about_me,read_stream,publish_stream";

            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters.scope = extendedPermissions;

            var fb = new FacebookClient();
            var url = fb.GetLoginUrl(parameters);

            return Redirect(url.ToString());
        }

        public ActionResult LoginRetorno()
        {
            var _fb = new FacebookClient();
            FacebookOAuthResult oauthResult;
            //Pega o Code
            if (!_fb.TryParseOAuthCallbackUrl(Request.Url, out oauthResult))
            {
                //erro       
            }

            if (oauthResult.IsSuccess)
            {
                //Pega o Access Token "permanente"

                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("client_id", ConfigurationManager.AppSettings["FacebookAppId"]);
                parameters.Add("redirect_uri", "http://localhost:2780/home/LoginRetorno");
                parameters.Add("client_secret", ConfigurationManager.AppSettings["FacebookAppSecret"]);
                parameters.Add("code", oauthResult.Code);

                dynamic result = _fb.Get("/oauth/access_token", parameters);

                var accessToken = result.access_token;
                //TODO:Guardar no banco    
                Session["accessToken"] = accessToken;

                //Exemplo de uso
                var client = new FacebookClient(accessToken);
                dynamic me = client.Get("me");


                ViewBag.Usuario = me.ToString();
                ViewBag.MostraForm = true;
            }
            else
            {
                // user cancelled
            }
            return View("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
