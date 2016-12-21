using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INIPAY41Lib;

namespace Bottleshop.Api.Lib
{
    public class TxBilling
    {
        public readonly string tid;
        public readonly string resultcode;
        public readonly string resultmsg;
        public readonly string authcode;
        public readonly string pgauthdate;
        public readonly string pgauthtime;
        public readonly bool result;

        public TxBilling(string price, string billKey)
        {
            INItx41 INIpay = new INItx41();
            int intPInst = INIpay.Initialize("");
            try
            {
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
                INIpay.SetField(ref intPInst, "price", price);//가격 (변경시에만 설정)
                INIpay.SetField(ref intPInst, "currency", "WON");//화폐단위 (변경시에만 설정)
                //INIpay.SetField(ref intPInst, "quotainterest", "1");//무이자할부 여부(1:Yes, 0:No)
                INIpay.SetField(ref intPInst, "billkey", billKey); //BillKey
                /*01:비인증 (공인인증으로 인증받은 빌키를 이용하는 경우, 비밀번호 + 주민번호 필요없음.)
                 *00:인증 (공인인증서로 인증받지 않은 경우, 비밀번호 + 주민번호 필요)
                 */
                INIpay.SetField(ref intPInst, "authentification", "01"); //본인인증 여부
                /*5. 빌링 승인 요청*/
                INIpay.StartAction(ref intPInst);
                tid = INIpay.GetResult(ref intPInst, "tid");
                resultcode = INIpay.GetResult(ref intPInst, "resultcode");
                resultmsg = INIpay.GetResult(ref intPInst, "resultmsg");
                authcode = INIpay.GetResult(ref intPInst, "authcode");
                pgauthdate = INIpay.GetResult(ref intPInst, "pgauthdate");
                pgauthtime = INIpay.GetResult(ref intPInst, "pgauthtime");
                result = INIpay.GetResult(ref intPInst, "resultcode") == "00" ? true : false;
            }
            catch(Exception ex)
            {
                new Log().Error(ex);
                result = false;
                resultmsg = ex.Message;
                resultcode = "XX";
            }
            INIpay.Destroy(ref intPInst);
        }
    }

    public class TxBillingCancle
    {
        public readonly string resultcode;
        public readonly string resultmsg;
        public readonly string CSHR_CancelNum;
        public readonly string pgauthdate;
        public readonly string pgauthtime;
        public readonly bool result;
        public TxBillingCancle(string tid)
        {
            INItx41 INIpay = new INItx41();
            int intPInst = INIpay.Initialize("");
            try
            {
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
                resultcode = INIpay.GetResult(ref intPInst, "resultcode");
                resultmsg = INIpay.GetResult(ref intPInst, "resultmsg");
                CSHR_CancelNum = INIpay.GetResult(ref intPInst, "CSHR_CancelNum");
                pgauthdate = INIpay.GetResult(ref intPInst, "CancelDate");
                pgauthtime = INIpay.GetResult(ref intPInst, "CancelTime");
                result = INIpay.GetResult(ref intPInst, "resultcode") == "00" ? true : false;
                //ResultCode.Text = INIpay.GetResult(ref intPInst, "resultcode");					// 결과코드 ("00"이면 지불성공)
                //ResultMsg.Text = INIpay.GetResult(ref intPInst, "resultmsg");					// 결과내용
                //CancelDate.Text = INIpay.GetResult(ref intPInst, "CancelDate");					// 이니시스 취소날짜
                //CancelTime.Text = INIpay.GetResult(ref intPInst, "CancelTime");					// 이니시스 취소시각
                //CSHR_CancelNum.Text = INIpay.GetResult(ref intPInst, "CSHR_CancelNum");				 //현금영수증 취소 승인번호
            }
            catch (Exception ex)
            {
                new Log().Error(ex);
                result = false;
                resultmsg = ex.Message;
                resultcode = "XX";
            }
            INIpay.Destroy(ref intPInst);
        }
    }
}