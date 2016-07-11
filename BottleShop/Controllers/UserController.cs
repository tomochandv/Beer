using BottleShop.Dac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BottleShop.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult Login()
        {
            return View();
        }
         [HttpPost]
        public JsonResult LoginProc(string id = "", string pwd = "")
        {
            bool result = false;
            List<UserModel> list = DataType.ConvertToList<UserModel>(new Dac_User().Login(id));
            if (list.Count == 1)
            {
                if(pwd == new Security().Description(list[0].PWD))
                { 
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string data = serializer.Serialize(list[0]);
                    FormsAuthenticationTicket newticket = new FormsAuthenticationTicket(1,
                                                                      "bottleshop",
                                                                      DateTime.Now,
                                                                      DateTime.Now.AddDays(1),
                                                                      false, // always persistent
                                                                      data,
                                                                      FormsAuthentication.FormsCookiePath);
                    HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(newticket));
                    cookie.Expires = newticket.Expiration;
                    Response.Cookies.Add(cookie);
                    FormsAuthentication.GetAuthCookie("bottleshop", true);
                    result = true;
                }
            }
            return Json(result);
        }

         public RedirectResult LogOut()
         {
             FormsAuthentication.SignOut();
             return Redirect(Url.Action("Index","Home"));
         }
         public ActionResult Join()
         {
             return View();
         }
    }
}
