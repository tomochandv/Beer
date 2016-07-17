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
    public class UserController : BaseController
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

        public ActionResult Joinus()
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

         [HttpPost]
         public JsonResult JoinProc(string id = "", string pwd = "", string name = "", string tell = "", string email = "", string addr = "", string sms = "Y", string isemail = "Y")
         {
             int count = 0;
             if (id != "" && pwd != "" && name != "" && tell != "" && email != "")
             {
                 count = new Dac_User().UserJon(id, new Security().Encription(pwd), name, tell, email, addr, sms, isemail);
                 if(count > 0)
                 {
                     List<UserModel> list = DataType.ConvertToList<UserModel>(new Dac_User().Login(id));
                     if (list.Count == 1)
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
                     }
                 }
             }
             return Json(count);
         }
         [HttpPost]
         public JsonResult Validate(string type = "", string value = "")
         {
             int count = 0;
             if (type == "id")
             {
                 count = DataType.GetInt(new Dac_User().CheckEmailAndId(value, "", "I"));
             }
             else
             {
                 count = DataType.GetInt(new Dac_User().CheckEmailAndId("", value, "E"));
             }
             return Json(count, JsonRequestBehavior.AllowGet);
         }

        public ActionResult Cart()
         {
             DataTable dt = new Dac_Cart().SelectCart(AUser().USERID).Tables[0];
             return View(dt);
         }
    }
}
