using System;
using System.Collections.Generic;
using System.IO;
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

        #region 상품관리
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
                filter = filter & Builders<Product>.Filter.Eq("CategoryId", DataType.Int(bc_idx));
            }
            if (pr_name != "")
            {
                filter = filter & Builders<Product>.Filter.Regex("ProductName", new BsonRegularExpression(pr_name, "i"));
            }
            if (isSale != "")
            {
                filter = filter & Builders<Product>.Filter.Eq("IsSale", isSale == "Y" ? true : false);
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

        public ActionResult ProductAdd()
        {
            return View();
        }

        public JsonResult ProductInsert(int cat = 0, string name = "", string country = "", string gubun = "", int qty = 0, int liter = 0, string inNm = "",
           int price = 0, int sellPrice = 0, int sellQty = 0, int price_nomem = 0, int inqty = 0)
        {
            int coutn = 0;
            try
            {
                Product model = new Models.Product();
                model.CategoryId = cat;
                model.InDate = DateTime.Now;
                model.Inqty = inqty;
                model.IsSale = false;
                model.ProductCompany = inNm;
                model.ProductCountry = country;
                model.ProductGubun = gubun;
                model.ProductLiter = liter;
                model.ProductName = name;
                model.ProductPurchsePrice = price;
                model.ProductQty = qty;
                model.ProductSaleMemberPrice = sellPrice;
                model.ProductSaleNormalPrice = price_nomem;
                model.ProductSaleQty = sellQty;
                MongodbHelper.InsertOneModel<Product>(model, "Product");
                coutn = 1;
            }
            catch (Exception ex)
            {
                coutn = 0;
            }
            return Json(coutn);
        }
        
        #endregion

        #region Notice
        public ActionResult Notice(int page = 1)
        {
            int rows = 20;
            ViewBag.page = page;
            int sidx = ((page - 1) * rows);
            var filter = FilterDefinition<Notice>.Empty;
            var sort = Builders<Notice>.Sort.Ascending("Indate");
            MongoPagingResult<Notice> result = MongodbHelper.FindPaging<Notice>(filter, sort, "Notice", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            return View(result);
        }

        public ActionResult NoticeWrite(string idx = "")
        {
            ViewBag.idx = idx;
            Notice collection = new Notice();
            if (idx != "")
            {
                var filter = Builders<Notice>.Filter.Eq("Id", ObjectId.Parse(idx));
                collection = MongodbHelper.FindOne<Notice>(filter, "Notice");
            }
            return View(collection);
        }
        [ValidateInput(false)]
        public JsonResult NoticeSave(string idx = "", string title = "", string content = "")
        {
            int count = 0;
            try
            {
                if (idx != "")
                {
                    var filter = Builders<Notice>.Filter.Eq("Id", ObjectId.Parse(idx));
                    var update = Builders<Notice>.Update.Set("Title", title).Set("Contents", content);
                    MongodbHelper.Update<Notice>(filter, update, "Notice");
                }
                else
                {
                    Notice notice = new Notice();
                    notice.Contents = content;
                    notice.Title = title;
                    notice.Indate = DateTime.Now;
                    MongodbHelper.InsertOneModel<Notice>(notice, "Notice");
                }
                count++;
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }

            return Json(count);
        }
        public JsonResult NoticeDelete(string idx = "")
        {
            int count = 0;
            try
            {
                if (idx != "")
                {
                    var filter = Builders<Notice>.Filter.Eq("Id", ObjectId.Parse(idx));
                    MongodbHelper.Delete<Notice>(filter, "Notice");
                }
                count++;
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }

            return Json(count);
        } 
        #endregion

        #region Popup
        public ActionResult PopUp()
        {
            var filter = FilterDefinition<Popup>.Empty;
            Popup popup = new Popup();
            popup = MongodbHelper.FindOne<Popup>(filter, "Popup");
            return View(popup);
        }
        [ValidateInput(false)]
        public JsonResult PopupSave(string title = "", string content = "", string useyn = "")
        {
            int count = 0;
            try
            {
                var filter = FilterDefinition<Popup>.Empty;
                Popup popup = new Popup();
                popup.Contents = content;
                popup.Title = title;
                popup.UseYn = useyn == "Y" ? true : false;
                MongodbHelper.ReplaceOne<Popup>(filter, popup, "Popup");
                count++;
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
            }

            return Json(count);
        }

        #region ckeditor images
        public void uploadnow(HttpPostedFileWrapper upload)
        {
            if (upload != null)
            {
                string ImageName = upload.FileName;
                string name = ImageName;
                string[] arrImageName = ImageName.Split('.');
                if (arrImageName.Length > 0)
                {
                    string extension = arrImageName[arrImageName.Length - 1];
                    name = string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHmmss"), extension);
                }
                string path = System.IO.Path.Combine(Server.MapPath("~/Upload/Images"), name);
                upload.SaveAs(path);
            }
        }
        public ActionResult uploadPartial()
        {
            var appData = Server.MapPath("~/Upload/Images");
            var images = Directory.GetFiles(appData).Select(x => new ImageView
            {
                Url = Url.Content("/Upload/Images/" + Path.GetFileName(x))
            });
            return View(images);
        }

        #endregion 
        #endregion

        #region Member
        public ActionResult Member(string name = "", string id = "", int page = 1)
        {
            int rows = 20;
            int sidx = ((page - 1) * rows);
            var filter = FilterDefinition<Member>.Empty;
            if (name != "")
            {
                filter = filter & Builders<Member>.Filter.Regex("Name", new BsonRegularExpression(name, "i"));
            }
            if (id != "")
            {
                filter = filter & Builders<Member>.Filter.Regex("Uid", new BsonRegularExpression(id, "i"));
            }
            var sort = Builders<Member>.Sort.Ascending("Indate");
            MongoPagingResult<Member> result = MongodbHelper.FindPaging<Member>(filter, sort, "Member", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.name = name;
            ViewBag.userids = id;
            ViewBag.page = page;
            return View(result);
        }

        public ActionResult ViewPay(string userid = "")
        {
            var filter = Builders<MemberPayInfo>.Filter.Eq("Uid", userid);
            var collection = MongodbHelper.FindOne<MemberPayInfo>(filter, "MemberPayInfo");
            return View(collection);
        }
        
        #endregion

        #region Promotion
        public ActionResult Promotion(int page = 1, string poro_code = "", string use = "", string useid = "", string usedate = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows);
            var filter = FilterDefinition<Promotion>.Empty;
            if (poro_code != "")
            {
                filter = filter & Builders<Promotion>.Filter.Regex("PromotionCode", new BsonRegularExpression(poro_code, "i"));
            }
            if (use != "")
            {
                filter = filter & Builders<Promotion>.Filter.Eq("Use", use == "Y" ? true : false);
            }
            if (useid != "")
            {
                filter = filter & Builders<Promotion>.Filter.Regex("Uid", new BsonRegularExpression(useid, "i"));
            }
            if (usedate != "")
            {
                filter = filter & Builders<Promotion>.Filter.Eq("UseDate", DataType.Datetime(usedate));
            }
            var sort = Builders<Promotion>.Sort.Ascending("PromotionCode");
            MongoPagingResult<Promotion> result = MongodbHelper.FindPaging<Promotion>(filter, sort, "Promotion", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.poro_code = poro_code;
            ViewBag.use = use;
            ViewBag.useid = useid;
            ViewBag.usedate = usedate;
            ViewBag.page = page;

            return View(result);
        }

        public ActionResult CreatePromotion()
        {
            return View();
        }

        public JsonResult AddPromotion(int count = 0)
        {
            int rows = 0;
            if (count > 0)
            {
                List<Promotion> list = new List<Promotion>();
                for (int i = 1; i <= count; i++)
                {
                    string code = string.Format("BOTTLE{0}", Guid.NewGuid().ToString("N"));
                    Promotion promotion = new Promotion();
                    promotion.PromotionCode = code;
                    promotion.Send = false;
                    promotion.Uid = "";
                    promotion.Use = false;
                    promotion.UseDate = DateTime.Now;
                    list.Add(promotion);
                    rows++;
                }
                MongodbHelper.InsertManyModel<Promotion>(list, "Promotion");
            }
            return Json(rows);
        }

        public JsonResult SendPromotion(string poro_code = "")
        {
            var filter = Builders<Promotion>.Filter.Eq("PromotionCode", poro_code);
            var update = Builders<Promotion>.Update.Set("Send", true);
            long result = MongodbHelper.Update<Promotion>(filter, update, "Promotion");
            return Json(result);
        } 
        #endregion

        public ActionResult Order(int page = 1, string name = "", string id = "", string orderid = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows);
            var filter = FilterDefinition<Order>.Empty;
            if (name != "")
            {
                filter = filter & Builders<Order>.Filter.Regex("PromotionCode", new BsonRegularExpression(name, "i"));
            }
            if (orderid != "")
            {
                filter = filter & Builders<Order>.Filter.Regex("Uid", new BsonRegularExpression(orderid, "i"));
            }
            if (id != "")
            {
                filter = filter & Builders<Order>.Filter.Regex("Uid", new BsonRegularExpression(id, "i"));
            }
            var sort = Builders<Order>.Sort.Descending("OrderDate");
            MongoPagingResult<Order> result = MongodbHelper.FindPaging<Order>(filter, sort, "Order", sidx, rows);

            double dd = double.Parse(result.Count.ToString()) / double.Parse(rows.ToString());
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.page = page;
            ViewBag.name = name;
            ViewBag.id = id;
            ViewBag.orderid = orderid;

            var filter1 = FilterDefinition<Member>.Empty;
            ViewBag.Members = MongodbHelper.Find<Member>(filter1, "Member");

            return View(result);
        }

        public JsonResult OrderStatusChange(string id = "", string status = "0")
        {
            long result = 0;
            if (id != "")
            {
                var filter = Builders<Order>.Filter.Eq("Id", ObjectId.Parse(id));
                var update = Builders<Order>.Update.Set("OrderStatus", DataType.Int(status));
                result= MongodbHelper.Update<Order>(filter, update, "Order");
            }
            return Json(result);
        }

        public JsonResult OrderStatusAllChange(string id = "", string status = "2")
        {
            long result = 0;
            if (id != "")
            {
                string[] arror_idx = id.Split(',');
                for (int i = 0; i < arror_idx.Length; i++)
                {
                    var filter = Builders<Order>.Filter.Eq("Id", ObjectId.Parse(arror_idx[i]));
                    var update = Builders<Order>.Update.Set("OrderStatus", DataType.Int(status));
                    result += MongodbHelper.Update<Order>(filter, update, "Order");
                }
            }
            return Json(result);
        }

        public JsonResult OrderList()
        {
            List<string> OrderId = new List<string>();
            List<TodayOrder> today = new List<TodayOrder>();
            List<TodayOrder> result = new List<TodayOrder>();
            var filter = Builders<Order>.Filter.Eq("OrderStatus", 0);
            var sort = Builders<Order>.Sort.Descending("OrderDate");
            var collection = MongodbHelper.FindSort<Order>(filter, sort, "Order");

            if(collection.Count > 0)
            {
               foreach(var data in collection)
               {
                   OrderId.Add(data.Id.ToString());
                   foreach(var data1 in data.Product)
                   {
                       today.Add(new TodayOrder(data1.Product, data1.Qty, null));
                   }
               }

               var groups = today.GroupBy(d => d.Product.Id)
                               .Select(
                                 g => new
                                 {
                                     Key = g.Key,
                                     Count = g.Sum(s => s.Qty)
                                 });
               foreach (var data in groups)
               {
                   var filter1 = Builders<Product>.Filter.Eq("Id", data.Key);
                   var collection1 = MongodbHelper.FindOne<Product>(filter1, "Product");
                   result.Add(new TodayOrder(collection1, data.Count, OrderId));
               }
                
            }

            return Json(result);
        }
    }
}
