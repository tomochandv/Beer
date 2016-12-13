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
using INIPAY41Lib;
using System.IO;

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
        public ActionResult CreatePwd()
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
        public ActionResult CounterPay(string userId)
        {
            ViewBag.userId = userId;
            return View();
        }
        public JsonResult CounterPaySave(string userId, string sdate, string edate)
        {
            int row = new Dac_User().UserPay(userId, "B", 20000, DateTime.Parse(sdate), DateTime.Parse(edate), "Y", "Bottleshop");
            return Json(row);
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

        public ActionResult ProcuctAddView()
        {
            return View();
        }
        public JsonResult ProductInsert(int cat = 0, string name="", string country = "", string gubun = "", int qty = 0, int liter = 0, string inNm = "", 
            int price = 0, int sellPrice = 0, int sellQty = 0)
        {
            int coutn = 0;
            try
            {
                List<List<Parameter>> list = new List<List<Parameter>>();
                list.Add(new Dac_Product().Createparameter(cat,
                                name,
                                country,
                                gubun,
                                qty,
                               liter,
                               inNm,
                               price,
                               sellPrice,
                                0, "N", liter, "", sellQty));
                new Dac_Product().ProductInsert(list);
                coutn = 1;
            }
            catch(Exception ex)
            {
                coutn = 0;
            }
            return Json(coutn);
        }
        public JsonResult SaleUpdate(string name = "" , string value = "", string price = "", string qty = "", string cat = "")
        {
            int result = 0;
            if(name != "")
            {
                List<List<Parameter>> list = new List<List<Parameter>>();
                string[] arrName = name.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrValue = value.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrprice = price.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrqty = qty.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                string[] arrCat = cat.Replace("[", "").Replace("]", "").Replace("\"", "").Split(',');
                for(int i=0; i < arrName.Length; i++)
                {
                    list.Add(new Dac_Product().CreateUpdateParameter(arrName[i], arrValue[i], arrprice[i], arrqty[i], arrCat[i]));
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

        public ActionResult Order(int page = 1, string name = "", string id = "", int od = 0, string email="", string birth = "")
        {
            int rows = 20;
            int sidx = ((page - 1) * rows) + 1;
            int eidx = page * rows;

            DataSet ds = new Dac_Cart().OrderHistory(sidx, eidx, name, id, od, email, birth);
            int totalRows = DataType.GetInt(ds.Tables[1].Rows[0][0]);
            double dd = totalRows / rows;
            ViewBag.page = page;
            ViewBag.total = totalRows;
            ViewBag.Pages = Math.Ceiling(dd);
            ViewBag.name = name;
            ViewBag.id = id;
            ViewBag.od = od;
            ViewBag.birth = birth;
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
                result = new Dac_Cart().OrderPrStatusUpdates(or_idx, status);
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
            //Response.Buffer = true;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename=DownForm.xls");
            Response.ContentType = "application/octet-stream";   // 모든 파일 강제 다운로드
            Response.WriteFile(Server.MapPath("/Down/") + "DownForm.xls");
            return View();
        }

        #region promotion
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

        public JsonResult SendPromo(string poro_code = "")
        {
            int row = new Dac_Promo().SendPromo(poro_code);
            return Json(row);
        }

        public ActionResult CreatePromo()
        {
            return View();
        }

        public JsonResult AddPromo(int count = 0)
        {
            int rows = new Dac_Promo().CreatePromo(count, AUser().USERID);
            return Json(rows);
        } 
        #endregion

        public ActionResult MemberShip()
        {
            return View();
        }

        #region notice
        public ActionResult NoticeWrite(int idx = 0)
        {
            ViewBag.idx = idx;
            List<NoticeModel> listModel = new List<NoticeModel>();
            if (idx != 0)
            {
                DataSet ds = new Dac_Notice().ViewNotice(idx);
                listModel = DataType.ConvertToList<NoticeModel>(ds.Tables[0]);
            }
            return View(listModel);
        }

        [ValidateInput(false)]
        public JsonResult NoticeSave(int idx = 0, string title = "", string content = "")
        {
            int count = new Dac_Notice().SaveNotice(idx, title, content);
            return Json(count);
        }
        public JsonResult NoticeDelete(int idx = 0)
        {
            int count = new Dac_Notice().DeleteNotice(idx);
            return Json(count);
        }
        public ActionResult NoticeList(int page = 1)
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
        #endregion

        public JsonResult CurrentOrderBottle()
        {
            DataSet ds = new Dac_Cart().SelectCurrentProduct();
            List<CurrentPrModel> listModel = DataType.ConvertToList<CurrentPrModel>(ds.Tables[0]);
            List<int> list = new List<int>();
            for (int i = 0; i < ds.Tables[1].Rows.Count; i++ )
            {
                list.Add(int.Parse(ds.Tables[1].Rows[i][0].ToString()));
            }
            if (listModel.Count > 0)
            {
                listModel[0].OR_IDX = list;
            }
            return Json(listModel);
        }

        public JsonResult StatusUpdateAll(string or_idx = "", string status = "3")
        {
            int result = 0;
            if (or_idx != "")
            {
                string[] arror_idx = or_idx.Split(',');
                for (int i = 0; i < arror_idx.Length; i++ )
                {
                    result = new Dac_Cart().OrderPrStatusUpdates(int.Parse(arror_idx[i]), status);
                }
            }
            return Json(result);
        }

        #region 빌링
        public ActionResult Bill()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DateTime sdate = DateTime.Now.AddDays(-7);
            DateTime edate = DateTime.Now;
            ViewBag.sdate = sdate;
            ViewBag.edate = edate;
            DataSet ds = new Dac_Cart().SelectBillList(sdate, edate);
            for (int i = 0; i < 8; i++)
            {
                string count = "No";
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    if (DateTime.Parse(ds.Tables[0].Rows[j]["INDATE"].ToString()).ToShortDateString() == sdate.AddDays(i).ToShortDateString())
                    {
                        count = "Yes";
                    }
                }
                dic.Add(sdate.AddDays(i).ToShortDateString(), count);
            }
            return View(dic);
        }

        public ActionResult Bill1(string sdate = "", string edate = "", string userid= "", string name = "", string iscancle = "")
        {
            ViewBag.tsdate = sdate;
            ViewBag.tedate = edate;
            ViewBag.tid = userid;
            ViewBag.tname = name;
            ViewBag.tcancle = iscancle;
            DateTime nsdate = DateTime.Parse(sdate == "" ? DateTime.Now.ToShortDateString() + " 00:00:00" : sdate + " 00:00:00");
            DateTime nedate = DateTime.Parse(edate == "" ? DateTime.Now.ToShortDateString() + " 23:59:59" : edate + " 23:59:59");
            DataSet ds = new Dac_Cart().SelectTotalBill(userid, name, nsdate, nedate, iscancle);

            return View(ds.Tables[0]);
        }

        public JsonResult BillTarget()
        {
            DataSet ds = new Dac_Cart().SelectBilTargetlList();
            List<AdminBillListModel> listModel = DataType.ConvertToList<AdminBillListModel>(ds.Tables[0]);
            var newList = from p in listModel
                          select new
                          {
                              IDX = p.IDX,
                              USERID = p.USERID,
                              PRICE = p.PRICE,
                              SDATE = p.SDATE.ToShortDateString(),
                              EDATE = p.EDATE.ToShortDateString(),
                              BILLKEY = p.BILL_KEY
                          };
            return Json(newList.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public JsonResult BillRun()
        {
            int count = 0;
            DataSet ds = new Dac_Cart().SelectBilTargetlList1();
            List<AdminBillListModel> listModel = DataType.ConvertToList<AdminBillListModel>(ds.Tables[0]);
            if (listModel != null && listModel.Count > 0)
            {
                foreach (var data in listModel)
                {
                    //TX
                    /*1. 객체 생성*/
                    INItx41 INIpay = new INItx41();
                    /*2. 인스턴스 초기화*/
                    int intPInst = INIpay.Initialize("");
                    /*3. 거래 유형 설정*/
                    INIpay.SetActionType(ref intPInst, "REQREALBILL");
                    /*4. 정보 설정*/
                    INIpay.SetField(ref intPInst, "pgid", "INIpayBill");//PG ID(고정)
                    INIpay.SetField(ref intPInst, "spgip", "203.238.3.10");//예비 PG ID(고정)
                    INIpay.SetField(ref intPInst, "uip", "203.238.3.10");//예비 PG ID(고정)
                    INIpay.SetField(ref intPInst, "mid", "bthebottle");//상점 아이디
                    INIpay.SetField(ref intPInst, "admin", "1111");//키 패스워드 (상점아이디에 따라 변경)
                    INIpay.SetField(ref intPInst, "url", "http://thebottleshop.co.kr");//홈페이지 주소 (URL)
                    INIpay.SetField(ref intPInst, "paymethod", "Card");//지불방법
                    INIpay.SetField(ref intPInst, "cardquota", "00");//할부기간 (변경시에만 설정)
                    INIpay.SetField(ref intPInst, "price", "20000");//가격 (변경시에만 설정)
                    INIpay.SetField(ref intPInst, "currency", "WON");//화폐단위 (변경시에만 설정)
                    //INIpay.SetField(ref intPInst, "quotainterest", "1");//무이자할부 여부(1:Yes, 0:No)
                    INIpay.SetField(ref intPInst, "billkey", data.BILL_KEY); //BillKey
                    /*01:비인증 (공인인증으로 인증받은 빌키를 이용하는 경우, 비밀번호 + 주민번호 필요없음.)
                     *00:인증 (공인인증서로 인증받지 않은 경우, 비밀번호 + 주민번호 필요)
                     */
                    INIpay.SetField(ref intPInst, "authentification", "01"); //본인인증 여부
                    /*5. 빌링 승인 요청*/
                    INIpay.StartAction(ref intPInst);
                    string resultCode = INIpay.GetResult(ref intPInst, "resultcode");
                    string result = "F";
                    if (resultCode == "00")
                    {
                        count++;
                        result = "Y";
                    }
                    new Dac_Cart().AutoBill_Insert(data.USERID, "S", 20000, data.SDATE.AddDays(1), data.EDATE.AddMonths(1), result, INIpay.GetResult(ref intPInst, "tid"), data.BILL_KEY);
                    new Dac_Cart().OrderBillResult(data.BILL_KEY, INIpay.GetResult(ref intPInst, "tid"), INIpay.GetResult(ref intPInst, "resultcode"), INIpay.GetResult(ref intPInst, "resultmsg"),
                        INIpay.GetResult(ref intPInst, "authcode"), INIpay.GetResult(ref intPInst, "pgauthdate"), INIpay.GetResult(ref intPInst, "pgauthtime"));
                    INIpay.Destroy(ref intPInst);
                }
            }
            return Json(count);
        }

        public ActionResult BillResult(string sdate, string edate)
        {
            DataSet ds = new Dac_Cart().Selectautobillresult(sdate, edate);
            return View(ds);
        }

        public JsonResult CancleBill(string tid,string moid)
        {
            string result = "F";
            try
            {
                INItx41 INIpay = new INItx41();
                /*2. 인스턴스 초기화*/
                int intPInst = INIpay.Initialize("");
                /*3. 거래 유형 설정*/
                INIpay.SetActionType(ref intPInst, "cancel");

                //###############################################################################
                //# 4. 정보 설정 #
                //################
                INIpay.SetField(ref intPInst, "pgid", "IniTechPG_"); //PG ID (고정)

                INIpay.SetField(ref intPInst, "mid", "bthebottle");			// 상점아이디
                INIpay.SetField(ref intPInst, "uip", "203.238.3.10");//예비 PG ID(고정)
                /**************************************************************************************************
                '* admin 은 키패스워드 변수명입니다. 수정하시면 안됩니다. 1111의 부분만 수정해서 사용하시기 바랍니다.
                '* 키패스워드는 상점관리자 페이지(https://iniweb.inicis.com)의 비밀번호가 아닙니다. 주의해 주시기 바랍니다.
                '* 키패스워드는 숫자 4자리로만 구성됩니다. 이 값은 키파일 발급시 결정됩니다. 
                '* 키패스워드 값을 확인하시려면 상점측에 발급된 키파일 안의 readme.txt 파일을 참조해 주십시오.
                '**************************************************************************************************/
                INIpay.SetField(ref intPInst, "admin", "1111");						// 키패스워드(상점아이디에 따라 변경)

                INIpay.SetField(ref intPInst, "tid", tid);			// 취소할 거래번호
                INIpay.SetField(ref intPInst, "CancelMsg", "사용자 이유");	// 취소 사유
                INIpay.SetField(ref intPInst, "CancelReason", "");	// 취소 코드
                INIpay.SetField(ref intPInst, "debug", "false");						// 로그모드(실서비스시에는 "false"로)

                //###############################################################################
                //# 5. 취소 요청 #
                //################
                INIpay.StartAction(ref intPInst);

                //###############################################################################
                //# 6. 취소 결과 #
                //################
                if (INIpay.GetResult(ref intPInst, "resultcode") == "00")
                {
                    result = "S";
                    new Dac_Cart().OrderStatusUpdateCancle(moid);
                    new Dac_Cart().OrderBillResult("C", tid, INIpay.GetResult(ref intPInst, "resultcode"), INIpay.GetResult(ref intPInst, "resultmsg"),
                       INIpay.GetResult(ref intPInst, "CancelDate"), INIpay.GetResult(ref intPInst, "CancelTime"), INIpay.GetResult(ref intPInst, "CSHR_CancelNum"));
                    INIpay.Destroy(ref intPInst);
                }
                //ResultCode.Text = INIpay.GetResult(ref intPInst, "resultcode");					// 결과코드 ("00"이면 지불성공)
                //ResultMsg.Text = INIpay.GetResult(ref intPInst, "resultmsg");					// 결과내용
                //CancelDate.Text = INIpay.GetResult(ref intPInst, "CancelDate");					// 이니시스 취소날짜
                //CancelTime.Text = INIpay.GetResult(ref intPInst, "CancelTime");					// 이니시스 취소시각
                //CSHR_CancelNum.Text = INIpay.GetResult(ref intPInst, "CSHR_CancelNum");				 //현금영수증 취소 승인번호


                //###############################################################################
                //# 7. 인스턴스 해제 #
                //####################
                INIpay.Destroy(ref intPInst);
            }
            catch (Exception ex)
            {

            }
            return Json(result);
        } 
        #endregion

        public ActionResult PopUp()
        {
            List<PopupModel> listModel = new List<PopupModel>();
            DataSet ds = new Dac_Popup().GetPopUp();
            listModel = DataType.ConvertToList<PopupModel>(ds.Tables[0]);
            return View(listModel);
        }
         [ValidateInput(false)]
        public JsonResult PopupSave(string title = "", string content = "", string useyn = "")
        {
            int count = new Dac_Popup().SetPopUp(title, content, useyn);
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
                 if(arrImageName.Length > 0)
                 {
                     string extension = arrImageName[arrImageName.Length - 1];
                     name = string.Format("{0}.{1}",DateTime.Now.ToString("yyyyMMddHmmss"), extension);
                 }
                 string path = System.IO.Path.Combine(Server.MapPath("~/Upload/Images"), name);
                 upload.SaveAs(path);
             }
         }
         public ActionResult uploadPartial()
         {
             var appData = Server.MapPath("~/Upload/Images");
             var images = Directory.GetFiles(appData).Select(x => new imagesviewModel
             {
                 Url = Url.Content("/Upload/Images/" + Path.GetFileName(x))
             });
             return View(images);
         }
        
         #endregion
    }
}
