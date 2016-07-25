using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BottleShop.Dac;
using System.Data;
namespace BottleShop.Controllers
{
    public class PayController : BaseController
    {
        //
        // GET: /Pay/

        public ActionResult Retutrn()
        {
            return View();
        }

        public ActionResult Pay()
        {
            //임시결재
            //new Dac_User().UserPay(AUser().USERID, "S", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "Y");
            return View();
        }

        public ActionResult CardPayResult()
        { 
             //임시결재
             new Dac_User().UserPay(AUser().USERID, "S", 20000, DateTime.Now, DateTime.Now.AddMonths(1), "Y");
            return View();
        }  
  
        public JsonResult PromoPayCheck(string poro_code = "")
        {
            string message = string.Empty;
            DataTable dt = new Dac_Promo().CheckPromo(poro_code).Tables[0];
            if(dt.Rows.Count > 0)
            {
                if(dt.Rows[0]["ISUSE"].ToString() == "Y")
                {
                    message = "프로모션 코드가 이미 사용되었습니다.";
                }
                else
                {
                    new Dac_Promo().UsePromo(poro_code, AUser().USERID);
                    new Dac_User().UserPay(AUser().USERID, "P", 0, DateTime.Now, DateTime.Now.AddMonths(1), "Y");
                    message = "Y";
                }
            }
            else
            {
                message = "프로모션 코드가 유효하지 않습니다.";
            }
            return Json(message);
        }

    }
}
