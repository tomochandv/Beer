using BottleShop.Dac;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BottleShop
{
    public class BaseController : Controller
    {
        protected override void OnAuthorization(AuthorizationContext context)
        {
            if (context.HttpContext.User != null && context.HttpContext.User.Identity.IsAuthenticated == true)
            {
                FormsIdentity id = (FormsIdentity)context.HttpContext.User.Identity;
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    AUser();
                    CurrentPayInfo();
                }
                else
                {
                    context.Result = new RedirectResult("/User/Login");
                }
            }
          
        }

        /// <summary>
        /// 사용자 정보 알아오기
        /// </summary>
        /// <returns></returns>
        public UserModel AUser()
        {
            UserModel info = new UserModel();
            FormsIdentity id = (FormsIdentity)User.Identity;
            if (id.IsAuthenticated)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                info = serializer.Deserialize<UserModel>(id.Ticket.UserData);
                ViewBag.NAME = info.NAME;
                ViewBag.USERID = info.USERID;
            }
            return info;
        }

        public PayInfoModel CurrentPayInfo()
        {
            PayInfoModel model = new PayInfoModel();
            UserModel uinfo = AUser();
            List<PayInfoModel> list = DataType.ConvertToList<PayInfoModel>(new Dac_User().PayInfoUse(uinfo.USERID));
            if(list.Count > 0)
            {
                model = list[0];
                if(model.SDATE <= DateTime.Now && DateTime.Now <= model.EDATE)
                {
                    ViewBag.pay = "Y";
                }
                else
                {
                    ViewBag.pay = "N";
                }
            }
            else
            {
                ViewBag.pay = "N";
            }
            return model;
        }
    }
}
