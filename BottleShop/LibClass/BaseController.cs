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
    }
}
