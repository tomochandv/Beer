using BottleShop.Dac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BottleShop.Controllers
{
    public class ProductController : BaseController
    {
        //
        // GET: /Product/

        public ActionResult Index(int bc_idx = 2, string pr_name = "", int page = 1)
        {
            int rows = 10;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page * rows;
            DataSet ds = new Dac_Product().ProductSelectUser(bc_idx, pr_name, sidx, eidx);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            List<ProductModel> list = DataType.ConvertToList<ProductModel>(ds.Tables[0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.pr_name = pr_name;
            ViewBag.bc_idx = bc_idx;
            string bc_name = string.Empty;
            switch(bc_idx)
            {
                case 1:
                    bc_name = "Wine";
                break;
                case 2:
                    bc_name = "BEER";
                break;
                case 3:
                    bc_name = "Whisky";
                break;
                case 4:
                    bc_name = "Special Order";
                break;
            }
           
            ViewBag.bc_name = bc_name;
            return View(list);
        }

        public JsonResult AddCart(int pr_idx = 0)
        {
           int cout = new Dac_Cart().AddCart(pr_idx, AUser().USERID);
           return Json(cout);
        }

        public JsonResult SpOrder(string message = "")
        {
            int cout = new Dac_Cart().OrderSp(message, AUser().USERID);
            return Json(cout);
        }


    }
}
