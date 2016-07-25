using BottleShop.Dac;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BottleShop.Controllers
{
    public class AdminController : AdminBaseController
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList(string name = "", string id = "", int page = 1)
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page * rows;
            DataSet ds = new Dac_User().SelectUSerList(sidx, eidx, name, id);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            List<UserModel> list = DataType.ConvertToList<UserModel>(ds.Tables[0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.name = name;
            ViewBag.userids = id;

            return View(list);
        }
        public ActionResult ProductList(int bc_idx = 0, string pr_name = "", int page = 1, string isSale = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page*rows;
            DataSet ds = new Dac_Product().ProductSelect(bc_idx, pr_name, sidx, eidx, isSale);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            List<ProductModel> list = DataType.ConvertToList<ProductModel>(ds.Tables[0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.pr_name = pr_name;
            ViewBag.bc_idx = bc_idx;
            ViewBag.isSale = isSale;
            return View(list);
        }
        public JsonResult SaleUpdate(string name = "" , string value = "", string price = "", string qty = "")
        {
            int result = 0;
            if(name != "")
            {
                List<List<Parameter>> list = new List<List<Parameter>>();
                string[] arrName = name.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrValue = value.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrprice = price.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrqty = qty.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                for(int i=0; i < arrName.Length; i++)
                {
                    list.Add(new Dac_Product().CreateUpdateParameter(arrName[i], arrValue[i], arrprice[i], arrqty[i]));
                }
                if (list.Count > 0)
                {
                    result = new Dac_Product().ProductUpdate(list);
                }
            }
            return Json(result);
        }

        #region Excel
        public RedirectResult ExcelImport()
        {
            if (Request.Files.Count > 0)
            {
                if (Request.Files[0].ContentLength > 0)
                {
                    string returnFilename = string.Empty;
                    string oledbProviderString_ver1 = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}; Extended Properties=\"Excel 8.0; HDR=Yes; IMEX=1\"";
                    string oledbProviderString_ver2 = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source={0}; Extended Properties=\"Excel 12.0; HDR=Yes; IMEX=1\"";

                    string file = string.Format("{0}1.xls", Server.MapPath("/Excel/"));
                    Request.Files[0].SaveAs(file);

                    string[] filenames = file.Split('.');
                    string file_extension = filenames[filenames.Length - 1].ToLower();

                    if (file_extension == "xls") returnFilename = String.Format(oledbProviderString_ver1, file);
                    else returnFilename = String.Format(oledbProviderString_ver2, file);

                    OleDbConnection oledbConn = new OleDbConnection(returnFilename);
                    try
                    {
                        // Open connection
                        oledbConn.Open();
                        // Create OleDbCommand object and select data from worksheet Sheet1
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [수입맥주$]", oledbConn);
                        // Create new OleDbDataAdapter
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        oleda.SelectCommand = cmd;
                        DataSet ds = new DataSet();
                        oleda.Fill(ds, "수입맥주");

                        cmd = new OleDbCommand("SELECT * FROM [위스키外$]", oledbConn);
                        oleda.SelectCommand = cmd;
                        oleda.Fill(ds, "위스키外");

                        cmd = new OleDbCommand("SELECT * FROM [크레프트비어$]", oledbConn);
                        oleda.SelectCommand = cmd;
                        oleda.Fill(ds, "크레프트비어");

                        // Bind the data to the GridView
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            InsertProduct(ds);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        // Close connection
                        oledbConn.Close();
                    }

                }
            }
            return Redirect("/Admin/ProductList");
        }

        public void InsertProduct(DataSet ds)
        {
            if (ds.Tables.Count == 3)
            {
                List<List<Parameter>> list = new List<List<Parameter>>();
                DataTable dt = ds.Tables["위스키外"];
                //위스키
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["상품명"].ToString() != "")
                    {
                        list.Add(new Dac_Product().Createparameter(3,
                            dt.Rows[i]["상품명"],
                            "",
                            dt.Rows[i]["구분"],
                            dt.Rows[i]["본수"].ToString() == "" ? "1" : dt.Rows[i]["본수"].ToString(),
                            dt.Rows[i]["용량"],
                            dt.Rows[i]["수입사"],
                            dt.Rows[i]["공급가"],
                            Caculate(dt.Rows[i]["공급가"], dt.Rows[i]["본수"]),
                            0, "N", dt.Rows[i]["용량"], "", 1));
                    }
                }

                dt = ds.Tables["수입맥주"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["상품명"].ToString() != "")
                    {
                        list.Add(new Dac_Product().Createparameter(2, dt.Rows[i]["상품명"], dt.Rows[i]["국가"], dt.Rows[i]["구분"],
                            dt.Rows[i]["본수"].ToString() == "" ? "1" : dt.Rows[i]["본수"].ToString(),
                            dt.Rows[i]["용량"], dt.Rows[i]["수입사"],
                            dt.Rows[i]["공급가"], Caculate(dt.Rows[i]["공급가"], dt.Rows[i]["본수"]), 0, "N", dt.Rows[i]["용량"], "", 1));
                    }
                }

                dt = ds.Tables["크레프트비어"];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["상품명"].ToString() != "")
                    {
                        list.Add(new Dac_Product().Createparameter(2, dt.Rows[i]["상품명"], "", "",
                            dt.Rows[i]["본수"].ToString() == "" ? "1" : dt.Rows[i]["본수"].ToString(),
                            dt.Rows[i]["용량"], dt.Rows[i]["수입사"],
                            dt.Rows[i]["공급가"], Caculate(dt.Rows[i]["공급가"], dt.Rows[i]["본수"]), 0, "N", dt.Rows[i]["용량"], "", 1));
                    }
                }

                if (list.Count > 0)
                {
                    new Dac_Product().ProductInsert(list);
                }
            }
        }

        private int Caculate(object packagePrice, object qty)
        {
            int nqty = DataType.GetInt(qty) == 0 ? 1 : DataType.GetInt(qty);
            int package = DataType.GetInt(packagePrice);
            string price = (package / nqty).ToString();
            price = price.Substring(0, price.Length - 2) + "00";
            int totalPrice = DataType.GetInt(price) + (nqty != 1 ? 200 : 0);
            return totalPrice;
        } 
        #endregion

        public ActionResult Order(int page = 1, string name = "", string id = "", int od = 0, string email="")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page * rows;

            DataSet ds = new Dac_Cart().OrderHistory(sidx, eidx, name, id, od, email);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.name = name;
            ViewBag.id = id;
            ViewBag.od = od;
            ViewBag.email = email;


            List<OrderInfoModel> listModel = DataType.ConvertToList<OrderInfoModel>(ds.Tables[0]);
            List<OrderProductModel> product = DataType.ConvertToList<OrderProductModel>(ds.Tables[2]);
            if (listModel != null)
            {
                foreach (var data in listModel)
                {
                    if (product != null)
                    {
                        List<OrderProductModel> ppModel = new List<OrderProductModel>();
                        foreach (var pp in product)
                        {
                            if (data.OR_IDX == pp.OR_IDX)
                            {
                                ppModel.Add(pp);
                            }
                        }
                        data.ProductList = ppModel;
                    }
                }
            }
            return View(listModel);
        }

        public JsonResult StatusUpdate(int or_idx = 0, string status = "0")
        {
            int result = 0;
            if (or_idx != 0)
            {
                result = new Dac_Cart().OrderStatusUpdate(or_idx, status);
            }
            return Json(result);
        }

        public ActionResult ViewPay(string userid = "")
        {
            List<PayInfoModel> list = DataType.ConvertToList<PayInfoModel>(new Dac_User().PayInfoUse(userid));
            return View(list);
        }

        public ActionResult Down()
        {
            Response.Buffer = true;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=DownForm.xls");
            Response.ContentType = "application/unknown";   // 모든 파일 강제 다운로드
            Response.WriteFile(Server.MapPath("/Down/") + "DownForm.xls");
            return View();
        }

        public ActionResult Promo(int page = 1, string poro_code = "", string use = "", string useid = "", string usedate = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page * rows;

            DataSet ds = new Dac_Promo().PromoList(sidx, eidx, poro_code, use, useid, usedate);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.poro_code = poro_code;
            ViewBag.use = use;
            ViewBag.useid = useid;
            ViewBag.usedate = usedate;

            List<PromoModel> listModel = DataType.ConvertToList<PromoModel>(ds.Tables[0]);

            return View(listModel);
        }

        public ActionResult CreatePromo()
        {
            return View();
        }

        public JsonResult AddPromo(int count =0)
        {
            int rows = new Dac_Promo().CreatePromo(count, AUser().USERID);
            return Json(rows);
        }

        public ActionResult MemberShip()
        {
            return View();
        }
    }
}
