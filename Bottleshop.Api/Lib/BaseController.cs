using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bottleshop.Api.Models;

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
                //CurrentPayInfo();
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

       
    }
}