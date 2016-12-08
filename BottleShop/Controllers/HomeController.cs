using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using BottleShop.Dac;

namespace BottleShop.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            List<PopupModel> listModel = new List<PopupModel>();
            DataSet ds = new Dac_Popup().GetPopUp();
            listModel = DataType.ConvertToList<PopupModel>(ds.Tables[0]);
            return View(listModel);
        }

        public ActionResult Popup()
        {
            List<PopupModel> listModel = new List<PopupModel>();
            DataSet ds = new Dac_Popup().GetPopUp();
            listModel = DataType.ConvertToList<PopupModel>(ds.Tables[0]);
            return View(listModel);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult coming()
        {
            return View();
        }

    }
}
