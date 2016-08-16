using BottleShop.Dac;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BottleShop.Controllers
{
    public class NoticeController : Controller
    {
        //
        // GET: /Notice/

        public ActionResult Index(int page = 1)
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page * rows;

            DataSet ds = new Dac_Notice().NoticeList(sidx, eidx);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            List<NoticeModel> listModel = DataType.ConvertToList<NoticeModel>(ds.Tables[0]);
            return View(listModel);
        }

        public ActionResult View(int idx = 0)
        {
            DataSet ds = new Dac_Notice().ViewNotice(idx);
            List<NoticeModel> listModel = DataType.ConvertToList<NoticeModel>(ds.Tables[0]);
            return View(listModel[0]);
        }

    }
}
