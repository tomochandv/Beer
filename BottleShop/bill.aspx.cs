using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using INIPAY41Lib;

namespace BottleShop
{
    public partial class bill : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            INIrealbill();
        }

        private void INIrealbill()
        {
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
            INIpay.SetField(ref intPInst, "billkey", "f60f96853ad6ce916963b9cab5588955a16fbe35"); //BillKey


            /*01:비인증 (공인인증으로 인증받은 빌키를 이용하는 경우, 비밀번호 + 주민번호 필요없음.)
             *00:인증 (공인인증서로 인증받지 않은 경우, 비밀번호 + 주민번호 필요)
             */
            INIpay.SetField(ref intPInst, "authentification", "01"); //본인인증 여부

            /*5. 빌링 승인 요청*/
            INIpay.StartAction(ref intPInst);
            string ssss = INIpay.GetResult(ref intPInst, "resultmsg");
            ssss += INIpay.GetResult(ref intPInst, "resultcode");
            Response.Write(ssss);
            /*6. 빌링 승인 결과*/
            //tid.Text = INIpay.GetResult(ref intPInst, "tid");
            //resultCode.Text = INIpay.GetResult(ref intPInst, "resultcode");
            //resultMsg.Text = INIpay.GetResult(ref intPInst, "resultmsg");
            //AuthCode.Text = INIpay.GetResult(ref intPInst, "authcode");
            //PGAuthDate.Text = INIpay.GetResult(ref intPInst, "pgauthdate");
            //PGAuthTime.Text = INIpay.GetResult(ref intPInst, "pgauthtime");

            /*6. 인스턴스 해제*/
            INIpay.Destroy(ref intPInst);
        }
    }
}