using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BottleShop.Dac;
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

    }
}
