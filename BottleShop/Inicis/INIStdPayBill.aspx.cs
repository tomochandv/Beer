using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using BottleShop;
using BottleShop.Dac;

public partial class INIStdPayBill : System.Web.UI.Page
{
    string uid = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        // 여기에 사용자 코드를 배치하여 페이지를 초기화합니다.
        if (!Page.IsPostBack)
            uid = Request["uid"];
            StartINIStd();

    }

    private void StartINIStd()
    {
        //각 필드 설정
        string strMid = "INIBillTst";
        mid.Text = strMid;

        string strVersion = "1.0";
        version.Text = strVersion;

        string strGoodName = "바틀샵 맴버십";
        goodname.Text = strGoodName;

        string strPrice = "1000";
        price.Text = strPrice;
        Textbox1.Text = strPrice;


        string strCurrency = "WON";
        currency.Text = strCurrency;

        string strBuyerName = AUser().NAME;
        buyername.Text = strBuyerName;
        Textbox3.Text = strBuyerName;

        string strBuyerTel = AUser().TELL;
        buyertel.Text = strBuyerTel;
        Textbox5.Text = strBuyerTel;

        string strBuyerEmail = AUser().EMAIL;
        buyeremail.Text = strBuyerEmail;
        Textbox4.Text = strBuyerEmail;

        Textbox2.Text = string.Format("{0} ~ {1} ", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));

        // TimeStamp 생성
        string timeTemp = "" + DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
        string[] artime = timeTemp.Split('.');
        timestamp.Text = artime[0];

        oid.Text = mid.Text + "_" + timestamp.Text;

        //Signature 생성 - 알파벳 순으로 정렬후 hash
        string param = "oid=" + oid.Text + "&price=" + price.Text + "&timestamp=" + timestamp.Text;
        signature.Text = ComputeHash(param);

        //closeURL
        string close = ConfigurationManager.AppSettings["Domain"] + "/Inicis/close.aspx";
        closeUrl.Text = close;

        //popupURL
        string popup = ConfigurationManager.AppSettings["Domain"] + "/Inicis/popup.aspx";
        popupUrl.Text = popup;

        //가맹점 전환 페이지 설정
        string strReturnUrl = ConfigurationManager.AppSettings["Domain"] + "/Inicis/INIStdPayReturn.aspx";
        //string strReturnUrl = "http://127.0.0.1/INIStdPayRelay.aspx";
        returnUrl.Text = strReturnUrl;

        // 가맹점확인을 위한 signKey 를 해쉬값으로 변경(SHA-256)
        string signKey = "SU5JTElURV9UUklQTEVERVNfS0VZU1RS";   // 가맹점에 제공된 키(이니라이트키) (가맹점 수정후 고정) !!!절대!! 전문 데이터로 설정금지
        mKey.Text = ComputeHash(signKey);


        InsertPayInfo(mid.Text + "_" + timestamp.Text, 1000, DateTime.Now, DateTime.Now.AddMonths(1));
    }

    // SHA256  256bit 암호화
    private string ComputeHash(string input)
    {
        System.Security.Cryptography.SHA256 algorithm = System.Security.Cryptography.SHA256Managed.Create();
        Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hashedBytes.Length; i++)
        {
            sb.Append(String.Format("{0:x2}", hashedBytes[i]));
        }


        return sb.ToString();
    }

    public UserModel AUser()
    {
        UserModel info = new UserModel();
        FormsIdentity id = (FormsIdentity)User.Identity;
        if (id.IsAuthenticated)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            info = serializer.Deserialize<UserModel>(id.Ticket.UserData);
        }
        return info;
    }

    public void InsertPayInfo(string moid, int price, DateTime stime, DateTime edate)
    {
        new Dac_User().UserPay(AUser().USERID, "R", price, stime, edate, "Y", moid);
    }
}