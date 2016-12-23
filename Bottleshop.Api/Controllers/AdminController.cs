using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bottleshop.Api.Lib;
using Bottleshop.Api.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Bottleshop.Api.Controllers
{
    public class AdminController : AdminBaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Product(int bc_idx = 0, string pr_name = "", int page = 1, string isSale = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows);
            ViewBag.page = page;
            ViewBag.bc_idx = bc_idx;
            ViewBag.pr_name = pr_name;
            ViewBag.isSale = isSale;
            ViewBag.bc_name = ProjectUtill.GetCategoryName(bc_idx);

            var filter = FilterDefinition<Product>.Empty;
            if (bc_idx != 0)
            {
                filter = filter & Builders<Product>.Filter.Eq("bc_idx", DataType.Int(bc_idx));
            }
            if (pr_name != "")
            {
                filter = filter & Builders<Product>.Filter.Eq("ProductName", pr_name);
            }
            if (isSale != "")
            {
                filter = filter & Builders<Product>.Filter.Eq("IsSale", isSale == "Y" ? true: false);
            }

            var sort = Builders<Product>.Sort.Ascending("ProductName");
            MongoPagingResult<Product> result = MongodbHelper.FindPaging<Product>(filter, sort, "Product", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            return View(result);
        }

        public JsonResult Update(string name = "", string value = "", string price = "", string qty = "", string cat = "", string price1 = "", string inqty = "")
        {
            int result = 0;
            if (name != "")
            {
                string[] arrName = name.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrValue = value.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrprice = price.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrqty = qty.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrCat = cat.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrprice1 = price1.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrInqty = inqty.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                for (int i = 0; i < arrName.Length; i++)
                {
                    var filter = Builders<Product>.Filter.Eq("Id", ObjectId.Parse(arrName[i]));
                    var update = Builders<Product>.Update.Set("IsSale", arrValue[i] == "Y" ? true : false).Set("ProductSaleMemberPrice", DataType.Float(arrprice[i]))
                        .Set("ProductSaleNormalPrice", DataType.Float(arrprice1[i])).Set("ProductSaleQty", DataType.Int(arrqty[i]))
                        .Set("CategoryId", DataType.Int(arrCat[i])).Set("Inqty", DataType.Int(arrInqty[i]));
                    MongodbHelper.Update<Product>(filter, update, "Product");
                    result++;
                }
            }
            return Json(result);
        }

    }
}
