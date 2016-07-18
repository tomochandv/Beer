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
        public JsonResult PwdFind(string id = "")
        {
            if(id != "")
            {
                List<UserModel> list = DataType.ConvertToList<UserModel>(new Dac_User().Login(id));
                if(list.Count > 0)
                {
                    string title = "The bottleshop 비밀번호 찾기";
                    string body = string.Format("문의하신 비밀번호는 {0} 입니다.", new Security().Description(list[0].PWD));

                    SendEmail(title, body, list[0].EMAIL);
                }
            }
            return Json("");
        }
        public ActionResult MyInfo()
        {
            List<PayInfoModel> list = DataType.ConvertToList<PayInfoModel>(new Dac_User().PayInfoUse(AUser().USERID));
            return View(list);
        }
        public JsonResult PwdChange(string pwd = "")
        {
            int row = 0;
            if (pwd != "")
            {
                row = new Dac_User().ChangePassword(AUser().USERID, new Security().Encription( pwd));
            }
            return Json(row);
        }

        public JsonResult DeleteAll()
        {
            int row = 0;
            row = new Dac_User().DeleteAllUSerInfo(AUser().USERID);
            if (row > 0)
            {
                FormsAuthentication.SignOut();
            }
            return Json(row);
        }

        public JsonResult DelteCart(int pr_idx)
        {
            int row = 0;
            row = new Dac_Cart().DeleteCart(pr_idx, AUser().USERID);
            return Json(row);
        }

        public RedirectResult Order()
        {
            string[] arrQty = Request["qty"].Split(',');
            string[] arrIdx = Request["pr_idx"].Split(',');
            string[] arrPrice = Request["pr_price"].Split(',');
            if (arrIdx.Length == arrPrice.Length && arrIdx.Length  == arrQty.Length)
            {
                int total_price = 0;
                for(int i=0; i < arrPrice.Length; i++)
                {
                    total_price += DataType.GetInt(arrPrice[i]) * DataType.GetInt(arrQty[i]);
                }
                int or_idx =  DataType.GetInt(new Dac_Cart().OrderInfo(AUser().USERID, total_price, DateTime.Now.AddDays(3)));
                if(or_idx > 0)
                {
                    for (int i = 0; i < arrIdx.Length; i++)
                    {
                        new Dac_Cart().OrderProduct(or_idx, DataType.GetInt(arrIdx[i]), DataType.GetInt(arrQty[i]), DataType.GetInt(arrPrice[i]));
                    }
                }
            }
            return Redirect(Url.Action("OrderHistory","User"));
        }
        public ActionResult OrderHistory()
        {
            DataSet ds = new Dac_Cart().OrderHistory(AUser().USERID);
            List<OrderInfoModel> listModel = DataType.ConvertToList<OrderInfoModel>(ds.Tables[0]);
            List<OrderProductModel> product = DataType.ConvertToList<OrderProductModel>(ds.Tables[1]);
            if(listModel != null)
            {
                foreach(var data in listModel)
                {
                    if(product != null)
                    {
                        List<OrderProductModel> ppModel = new List<OrderProductModel>();
                        foreach(var pp in product)
                        {
                            if(data.OR_IDX == pp.OR_IDX)
                            {
                                ppModel.Add(pp);
                            }
                        }
                        data.ProductList = ppModel;
                    }
                }
            }
            return View(listModel);
        }
    }
}

