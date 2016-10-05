<%@ Page Language="C#" AutoEventWireup="true" CodeFile="INIStdPayBill.aspx.cs" Inherits="INIStdPayBill" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
    <title>바틀샵 결재</title>

    <style type="text/css">
        body { background-color: #efefef;}
        body, tr, td {font-size:9pt; font-family:굴림,verdana; color:#433F37; line-height:19px;}
        table, img {border:none}
    </style>

    <!-- 이니시스 표준결제 js -->
    <script type="text/javascript" src="https://stgstdpay.inicis.com/stdjs/INIStdPay.js" charset="UTF-8"></script>

    <script type="text/javascript">
        function send()
        {
            INIStdPay.pay('SendPayForm_id');
        }
        //send();
    </script>
</head>
<body bgcolor="#FFFFFF" text="#242424" leftmargin=0 topmargin=15 marginwidth=0 marginheight=0 bottommargin=0 rightmargin=0>
	<form id="SendPayForm_id" method="post"    runat="server">   
	<div style="padding:10px;background-color:#f3f3f3;width:100%;font-size:13px;color: #ffffff;background-color: #000000;text-align: center">
		더바틀샵 맴버십 결제
	</div>
	
	<table width="650" border="0" cellspacing="0" cellpadding="0" style="padding:10px; display:none;" align="center" >
		<tr>
			<td bgcolor="6095BC" align="center" style="padding:10px">
				<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#FFFFFF" style="padding:20px">                        
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td style="text-align:left;">
                                                <br/>
                                                <div style="border:2px #dddddd double;padding:10px;background-color:#f3f3f3;text-align:left">


                                                    <br/><b>version</b> :
                                                    <br/><asp:textbox    id="version" Text="" runat="server" Width="400px" />

                                                    <br/><b>mid</b> :
                                                    <br/><asp:textbox    id="mid" Text="" runat="server" Width="400px" />

                                                    <br/><b>goodname</b> :
                                                    <br/><asp:textbox   id="goodname" Text="" runat="server" Width="400px" />

                                                    <br/><b>oid</b> :
                                                    <br/><asp:textbox   id="oid" Text="" runat="server" Width="400px" />

                                                    <br/><b>price</b> :
                                                    <br/><asp:textbox   id="price" Text="" runat="server" Width="400px" />

                                                    <br/><b>currency</b> :
                                                    <br/>[WON|USD]
                                                    <br/><asp:textbox   id="currency" Text="" runat="server" Width="400px" />

                                                    <br/><b>buyername</b> :
                                                    <br/><asp:textbox   id="buyername" Text="" runat="server" Width="400px" />

                                                    <br/><b>buyertel</b> :
                                                    <br/><asp:textbox   id="buyertel" Text="" runat="server" Width="400px" />

                                                    <br/><b>buyeremail</b> :
                                                    <br/><asp:textbox   id="buyeremail" Text="" runat="server" Width="400px" />
                                                    <br/>
                                                    <br/><b>returnUrl</b> : 
                                                    <br/><asp:textbox   id="returnUrl" Text="" runat="server" Width="400px"/>
                                                    <br/>
                                                    <br/><b>timestamp</b> : HiddenField
                                                    <br/><asp:textbox    id="timestamp" Text=""  runat="server" Width="400px" />
                                                    <br/>
                                                    <br/><b>signature</b> : HiddenField
                                                    <br/><asp:textbox   id="signature" runat="server" Width="400px" />
                                                    <br/>
                                                    <br/><b>MKEY(가맹점키)</b> : HiddenField
                                                    <br/><asp:TextBox  id="mKey" runat="server" Width="400px"/>
                                                </div>
                                                <br/><br/>
											    <b>***** 기본 옵션 *****</b>
											    <div style="border:2px #dddddd double;padding:10px;background-color:#f3f3f3;">
                                                    <%-- <input type="hidden" style="width:100%;" value=""> --%>
                                                    <asp:HiddenField runat="server" id="gopaymethod" Value="" />
                                                    <b>offerPeriod</b> : 제공기간
                                                    <br/>ex)20150101-20150331, [Y2:년단위결제, M2:월단위결제, yyyyMMdd-yyyyMMdd : 시작일-종료일]
                                                    <br/><asp:textbox   id="offerPeriod" Text="M2"  runat="server"  Width="400px" />
                                                    <br/>
                                                    <br/><b>acceptmethod :</b> 
                                                    <br/>ex) billauth(card) , billauth(hpp) 
                                                    <br/><asp:textbox  id="acceptmethod" Text="BILLAUTH(card)" runat="server" Width="500px" />
                                                    <br/><br/>
                                                    <b>결제일 알림 메세지</b> : 결제일 알림 메세지
												    <br/><asp:TextBox runat="server"  style="width:500px;" id="billPrint_msg" Text="고객님의 매월 결제일은 24일 입니다." />
                                                </div>
												<br/><br/>
											    <b>***** 표시 옵션 *****</b>
											    <div style="border:2px #dddddd double;padding:10px;background-color:#f3f3f3;">
                                                    <br/><b>charset</b> : 리턴 인코딩
                                                    <br/>[UTF-8|EUC-KR] (default:UTF-8)
                                                    <br/><asp:textbox  id="charset" Text="" runat="server" Width="200px" />
                                                    <br/>
                                                    <br/><b>payViewType</b> : 결제창 표시방법
                                                    <br/>[overlay] (default:overlay)
                                                    <br/><asp:textbox  id="payViewType" Text="" runat="server" Width="200px"/>
                                                    <br/>
                                                    <br/><b>closeUrl</b> : payViewType='overlay','popup'시 취소버튼 클릭시 창닫기 처리 URL(가맹점에 맞게 설정)
                                                    <br/>close.jsp 샘플사용(생략가능, 미설정시 사용자에 의해 취소 버튼 클릭시 인증결과 페이지로 취소 결과를 보냅니다.)
                                                    <br/><asp:textbox  id="closeUrl" Text="http://127.0.0.1/close.aspx" runat="server" Width="400px" />
                                                    <br/>
                                                    <br/><b>popupUrl</b> : payViewType='popup'시 팝업을 띄울수 있도록 처리해주는 URL(가맹점에 맞게 설정)
                                                    <br/>popup.jsp 샘플사용(생략가능,payViewType='popup'으로 사용시에는 반드시 설정)
                                                    <br/><asp:textbox  id="popupUrl" Text="http://127.0.0.1/popup.aspx" runat="server" Width="400px"/>
                                                    <br/><br/>
                                                </div>
                                                <br /><br />
                                                <b>***** 추가 옵션 *****</b>
											    <div style="border:2px #dddddd double;padding:10px;background-color:#f3f3f3;">
												    <br/><b>merchantData</b> : 가맹점 관리데이터(2000byte)
												    <br/>인증결과 리턴시 함께 전달됨
												    <br/><asp:textbox runat="server" style="width:100%;" id="merchantData" Text="" />
											    </div>
                                        
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

    <table width="650" border="0" cellspacing="0" cellpadding="0" style="padding:10px;" align="center">
		<tr>
			<td bgcolor="6095BC" align="center" style="padding:10px">
				<table width="100%" border="0" cellspacing="0" cellpadding="0" bgcolor="#FFFFFF" style="padding:20px">                        
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td style="text-align:left;">
                          
                                                 <asp:Button id="Button1"  runat="server" Text="결제요청" OnClientClick="send();return false;" style="padding:10px"></asp:Button>
                                                <br/>
                                                <!-- 필수 -->
                                                <br/><b>***** 결재정보 *****</b>
                                                <div style="border:2px #dddddd double;padding:10px;background-color:#f3f3f3;text-align:left">


                                                    <br/><b>가격</b> :
                                                    <br/><asp:textbox    id="Textbox1" Text="" runat="server" Width="400px" ReadOnly="true" />

                                                    <br/><b>기간</b> :
                                                    <br/><asp:textbox    id="Textbox2" Text="" runat="server" Width="400px" ReadOnly="true" />

                                                    <br/><b>성함</b> :
                                                    <br/><asp:textbox   id="Textbox3" Text="" runat="server" Width="400px" ReadOnly="true" />

                                                    <br/><b>이메일</b> :
                                                    <br/><asp:textbox   id="Textbox4" Text="" runat="server" Width="400px" ReadOnly="true" />

                                                    <br/><b>전화번호</b> :
                                                    <br/><asp:textbox   id="Textbox5" Text="" runat="server" Width="400px" ReadOnly="true" />
                                                </div>
                                     
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
            </form>
</body>
</html>
