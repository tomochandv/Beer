using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bottleshop.Api.Models;
using MongoDB.Driver;

namespace Bottleshop.Api.Lib
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext context)
        {
            var userInCookie = Request.Cookies["bottleshop"];
            if (userInCookie != null)
            {
                AUser();
                GetPayInfo();
            }
            else
            {
                //context.Result = new RedirectResult("/User/Login");
            }
        }

        /// <summary>
        /// 사용자 정보 알아오기
        /// </summary>
        /// <returns></returns>
        public LoginCookie AUser()
        {
            LoginCookie info = new LoginCookie();
            var userInCookie = Request.Cookies["bottleshop"];

            if (userInCookie != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                info = serializer.Deserialize<LoginCookie>(userInCookie.Value);
                ViewBag.NAME = HttpContext.Server.UrlDecode(info.Name);
                ViewBag.USERID = info.Uid;
            }
            return info;
        }

        public bool GetPayInfo()
        {
            bool result = false;
            var filter = Builders<MemberPayInfo>.Filter.Eq("Uid", AUser().Uid);
            var model = MongodbHelper.FindOne<MemberPayInfo>(filter, "MemberPayInfo");
            if(model != null)
            {
                if(model.billList != null && model.billList.Count > 0)
                {
                    var data = model.billList.Where(x => x.Use == true && x.StartDate <= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59")
                        && x.EndDate >= DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00")).ToList();
                    if(data.Count > 0)
                    {
                        result = true;
                        ViewBag.pay = "Y";
                    }
                }
            }

            return result;
        }

       
    }
}