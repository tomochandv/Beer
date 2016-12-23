using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using BottleShop.Api.Lib;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Bottleshop.Api.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Joinus()
        {
            return View();
        }

        public JsonResult MemberSave(string id = "", string pwd = "", string name = "", 
            string tell = "", string email = "", string birth = "")
        {
            bool result = false;
            try
            {
                if (id != "" && pwd != "" && name != "" && tell != "" && email != "")
                {
                    Member member = new Member();
                    member.Uid = id;
                    member.Pwd = new Security().Encription(pwd);
                    member.Name = name;
                    member.Tell = tell;
                    member.Email = email;
                    member.Birth = birth;
                    member.CreateDate = DateTime.Now;
                    member.MemberType = "U";

                    MongodbHelper.InsertOneModel<Member>(member, "Member");

                    LoginCookie loginCookie = new LoginCookie();
                    loginCookie.Uid = Server.UrlEncode(id);
                    loginCookie.Name = Server.UrlEncode(name);
                    var json = JsonConvert.SerializeObject(loginCookie);
                    var userCookie = new HttpCookie("bottleshop", json);
                    userCookie.Expires.AddHours(5);
                    HttpContext.Response.Cookies.Add(userCookie);
                }
                result = true;
            }
            catch(Exception ex)
            {
                new Log().Error(ex);
            }
            return Json(result);
        }

        public JsonResult Validate(string type = "", string value = "")
        {
            int count = 0;
            if (type == "id")
            {
                var document = MongodbHelper.Find<Member>(new BsonDocument(), "Member").Where(x => x.Uid == value);
                count = document.ToList().Count;
            }
            else if (type == "birth")
            {
                try
                {
                    DateTime date = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    if (ProcGetAge(date.ToShortDateString(), true) < 20)
                    {
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }
                }
                catch (Exception ex)
                {
                    new Log().Error(ex);
                    count = 404;
                }
            }
            else
            {
                var document = MongodbHelper.Find<Member>(new BsonDocument(), "Member").Where(x => x.Email == value);
                count = document.ToList().Count;
            }
            return Json(count);
        }

        public JsonResult LoginProc(string id = "", string pwd = "")
        {
            bool result = false;
            var list = MongodbHelper.Find<Member>(new BsonDocument(), "Member").Where(x => (x.Uid == id && new Security().Description(x.Pwd) == pwd)).ToList();
            if (list.Count == 1)
            {
                LoginCookie loginCookie = new LoginCookie();
                loginCookie.Uid = Server.UrlEncode(list[0].Uid);
                loginCookie.Name = Server.UrlEncode(list[0].Name);
                var json = JsonConvert.SerializeObject(loginCookie);
                var userCookie = new HttpCookie("bottleshop", json);
                userCookie.Expires.AddHours(5);
                HttpContext.Response.Cookies.Add(userCookie);
                result = true;
            }
            return Json(result);
        }

        public JsonResult PwdChange(string pwd = "")
        {
            int row = 0;
            if (pwd != "")
            {
                var filter = Builders<Member>.Filter.Eq("Uid", AUser().Uid);
                var update = Builders<Member>.Update.Set("Pwd", new Security().Encription(pwd));
                MongodbHelper.Update<Member>(filter, update, "Member");
                row = 1;
            }
            return Json(row);
        }

        public JsonResult PwdFind(string id = "")
        {
            if (id != "")
            {
                var filter = Builders<Member>.Filter.Eq("Uid", id);
                Member info = MongodbHelper.FindOne<Member>(filter, "Member");
                if (info != null)
                {
                    string title = "The bottleshop 비밀번호 찾기";
                    string body = string.Format("문의하신 비밀번호는 {0} 입니다.", new Security().Description(info.Pwd));

                   ProjectUtill.SendEmail(title, body, info.Email);
                }
            }
            return Json("");
        }

        public JsonResult UserOut()
        {
            int row = 0;
            var filter = Builders<Member>.Filter.Eq("Uid", AUser().Uid);
            var filter1 = Builders<MemberPayInfo>.Filter.Eq("Uid", AUser().Uid);
            var update = Builders<MemberPayInfo>.Update.Set("BillingUse", false);
            MongodbHelper.Update<MemberPayInfo>(filter1, update, "MemberPayInfo");
            MongodbHelper.Delete<Member>(filter, "Member");
            return Json(row);
        }

        public RedirectResult LogOut()
        {
            if (Request.Cookies["bottleshop"] != null)
            {
                var user = new HttpCookie("bottleshop")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    Value = null
                };
                Response.Cookies.Add(user);
            }
            //FormsAuthentication.SignOut();
            return Redirect(Url.Action("Index", "Home"));
        }

        private int ProcGetAge(string BirthDay, bool KoreanAgeType = true) // BirthDay Format - YYYY-MM-DD
        {
            int Age = 0;
            int BirthYear = Convert.ToInt32(BirthDay.Substring(2, 2));
            int NowYear = DateTime.Now.Year;
            if (BirthDay.Substring(0, 2) == "19")
            {
                Age = (NowYear - (1900 + BirthYear));
            }
            else
            {
                Age = (NowYear - (2000 + BirthYear));
            }
            if (KoreanAgeType)
            {
                int BirthMonth = Convert.ToInt32(BirthDay.Substring(5, 2));
                int nowMonth = DateTime.Now.Month;

                if (BirthMonth == nowMonth)
                {
                    int BirthDays = Convert.ToInt32(BirthDay.Substring(8, 2));
                    int nowDay = DateTime.Now.Day;

                    if (BirthDays <= nowDay)
                        Age = Age + 1;
                }
                else if (BirthMonth < nowMonth)
                    Age = Age + 1;
            }
            return Age;
        }

    }
}
