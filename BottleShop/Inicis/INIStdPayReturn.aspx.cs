using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;
using BottleShop.Dac;

public partial class INIStdPayReturn : System.Web.UI.Page
{
    public string url = string.Empty;
    public string paybill = string.Empty;
    public string result = "F";
    public string orderid = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        // 여기에 사용자 코드를 배치하여 페이지를 초기화합니다.
        if (!Page.IsPostBack)
            StartINIStdReturn();
    }

    private void StartINIStdReturn()
    {
        BottleShop.OSIO.Log("start");
        NameValueCollection parameters = Request.Params;

        IEnumerator enumerator = parameters.GetEnumerator();

        StringBuilder sb = new StringBuilder("paramMap : ");
        while (enumerator.MoveNext())
        {
            // get the current query parameter
            string key = enumerator.Current.ToString();

            // insert the parameter into the url
            sb.Append(string.Format("{0}={1}&", key, HttpUtility.UrlEncode(parameters[key])));

        }

        //Console.WriteLine(sb.ToString());


        //#####################
        // 인증이 성공일 경우만
        //#####################
        if ("0000".Equals(parameters["resultCode"]))
        {

            //Response.Write("####인증성공/승인요청####");
            //Response.Write("<br/>");

            //Console.WriteLine("####인증성공/승인요청####");

            //############################################
            // 1.전문 필드 값 설정(***가맹점 개발수정***)
            //############################################

            String mid = parameters.Get("mid");					        // 가맹점 ID 수신 받은 데이터로 설정

            String signKey = "cnl4dktVbk5YcnYxeGdOT0JCM0RIZz09";	    // 가맹점에 제공된 키(이니라이트키) (가맹점 수정후 고정) !!!절대!! 전문 데이터로 설정금지


            string timeTemp = "" + DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds;
            string[] artime = timeTemp.Split('.');
            String timestamp = artime[0];

            String charset = "UTF-8";								    // 리턴형식[UTF-8,EUC-KR](가맹점 수정후 고정)

            String format = "JSON";								        // 리턴형식[XML,JSON,NVP](가맹점 수정후 고정)

            String authToken = parameters.Get("authToken");			    // 취소 요청 tid에 따라서 유동적(가맹점 수정후 고정)

            String authUrl = parameters.Get("authUrl");				    // 승인요청 API url(수신 받은 값으로 설정, 임의 세팅 금지)

            String netCancel = parameters.Get("netCancelUrl");			// 망취소 API url(수신 받은 값으로 설정, 임의 세팅 금지)

            String mKey = ComputeHash(signKey);	                        // 가맹점 확인을 위한 signKey를 해시값으로 변경 (SHA-256방식 사용)

            


            //#####################
            // 2.signature 생성
            //#####################

            String signParam = "authToken=" + @authToken + "&timestamp=" + timestamp;

            String signature = ComputeHash(signParam);


            //#####################
            // 3.API 요청 전문 생성
            //#####################
            System.Collections.Generic.Dictionary<String, String> authMap = new System.Collections.Generic.Dictionary<String, String>();

            authMap.Add("mid"           , mid);			            // 필수
            authMap.Add("authToken"     , HttpUtility.UrlEncode(authToken));	// 필수 - 반드시 urlencode 해서 전달.
            authMap.Add("timestamp"     , timestamp);	            // 필수
            authMap.Add("signature"     , signature);	            // 필수            
            authMap.Add("charset"       , charset);		            // default=UTF-8
            authMap.Add("format"        , format);		            // default=XML
            authMap.Add("mkey"          , mKey);		            // default=XML
            

            //Console.WriteLine("##승인요청 API 요청##");

            try
            {
                //#####################
                // 4.API 통신 시작
                //#####################
                String authResultString = "";
                authResultString = processHTTP(authMap, authUrl);

                //############################################################
                //5.API 통신결과 처리(***가맹점 개발수정***)
                //############################################################
                //Response.Write("## 승인 API 결과 ##");
                String strReplace = authResultString.Replace(",", "&").Replace(":", "=").Replace("\"", "").Replace(" ", "").Replace("\n", "").Replace("}", "").Replace("{", "");
                System.Collections.Generic.Dictionary<string, string> resultMap = parseStringToMap(strReplace);         //문자열을 MAP형식으로 파싱

                //Response.Write("<pre>");
                //Response.Write("<table width='565' border='0' cellspacing='0' cellpadding='0'>");

                /*************************  결제보안 추가 START ****************************/ 
				Dictionary<String , String> secureMap = new Dictionary<String, String>();
				secureMap.Add("mid"			, mid);							//mid
				secureMap.Add("tstamp"		, timestamp);					//timestemp
				secureMap.Add("MOID"		, resultMap["MOID"]);			//MOID
				secureMap.Add("TotPrice"	, resultMap["TotPrice"]);		//TotPrice
				
				// signature 데이터 생성 
				String secureSignature = makeSignatureAuth(secureMap);
				/*************************  결제보안 추가 END ****************************/
                if ("0000".Equals((resultMap.ContainsKey("resultCode") ? resultMap["resultCode"] : "null")) && secureSignature.Equals(resultMap["authSignature"]))	//결제보안 추가
                {

                    /*****************************************************************************
                    * 여기에 가맹점 내부 DB에 결제 결과를 반영하는 관련 프로그램 코드를 구현한다.  
				   
                      [중요!] 승인내용에 이상이 없음을 확인한 뒤 가맹점 DB에 해당건이 정상처리 되었음을 반영함
                              처리중 에러 발생시 망취소를 한다.
                    ******************************************************************************/

                    System.Collections.Generic.Dictionary<string, string> checkMap = new System.Collections.Generic.Dictionary<string, string>();

                    checkMap.Add("mid"          , mid);			// 필수
                    checkMap.Add("authToken"    , HttpUtility.UrlEncode(authToken));	// 필수 - 반드시 urlencode 해서 전달.
                    checkMap.Add("applDate"     , (resultMap.ContainsKey("applDate") ? resultMap["applDate"] : "null"));		// 필수					
                    checkMap.Add("applTime"     , (resultMap.ContainsKey("applTime") ? resultMap["applTime"] : "null"));		// 필수	
                    checkMap.Add("timestamp"    , timestamp);	// 필수
                    checkMap.Add("signature"    , signature);	// 필수            
                    checkMap.Add("charset"      , charset);		// default=UTF-8
                    checkMap.Add("format"       , format);		// default=XML


                    //Response.Write("<tr><th class='line' colspan='2'><p></p></th></tr>");

                    paybill = resultMap.ContainsKey("CARD_BillKey") ? resultMap["CARD_BillKey"] : "";
                    orderid = resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "";
                    result = "S";
					new Dac_Cart().OrderBillKey(orderid, paybill);
                    //new Dac_Cart().OrderStatusUpdate(resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "", "S");
                    //new Dac_Cart().OrderBillKey(resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "", resultMap.ContainsKey("CARD_BillKey") ? resultMap["CARD_BillKey"] : "");

                    //Response.Write("<tr><th class='td01'><p>거래 성공 여부</p></th>");
                    //Response.Write("<td class='td02'><p>성공</p></td></tr>");
                    //Response.Write("<tr><th class='line' colspan='2'><p></p></th></tr>");
                    //Response.Write("<tr><th class='td01'><p>결과 코드</p></th>");
                    //Response.Write("<td class='td02'><p>" + (resultMap.ContainsKey("resultCode") ? resultMap["resultCode"] : "null") + "</p></td></tr>");
                    //Response.Write("<tr><th class='line' colspan='2'><p></p></th></tr>");
                    //Response.Write("<tr><th class='td01'><p>결과 내용</p></th>");
                    //Response.Write("<td class='td02'><p>" + (resultMap.ContainsKey("resultMsg") ? resultMap["resultMsg"] : "null") + "</p></td></tr>");

                }
                else
                {
                    orderid = resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "";
                    result = "F";
                    BottleShop.OSIO.Log("f");
                    //new Dac_Cart().OrderStatusUpdate(resultMap.ContainsKey("MOID") ? resultMap["MOID"] : "", "F");
                }
            }
            catch (Exception ex)
            {
                BottleShop.OSIO.Log(ex.Message);
            }

        }
        else
        {
            BottleShop.OSIO.Log("11111");
        }
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




    private string processHTTP(System.Collections.Generic.Dictionary<string, string> mapParam, string url)
    {


        string postData = "";
        foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in mapParam)
        {
            string param = kvp.Key + "=" + kvp.Value + "&";
            postData += param;
        }

        postData = postData.Substring(0, postData.Length - 1);

        System.Net.WebRequest request = System.Net.WebRequest.Create(url);
        // Set the Method property of the request to POST.
        request.Method = "POST";
        // Create POST data and convert it to a byte array.

        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        // Set the ContentType property of the WebRequest.
        request.ContentType = "application/x-www-form-urlencoded";
        // Set the ContentLength property of the WebRequest.
        request.ContentLength = byteArray.Length;
        // Get the request stream.
        System.IO.Stream dataStream = request.GetRequestStream();
        // Write the data to the request stream.
        dataStream.Write(byteArray, 0, byteArray.Length);
        // Close the Stream object.
        dataStream.Close();
        // Get the response.
        System.Net.WebResponse response = request.GetResponse();
        // Display the status.
        Console.WriteLine(((System.Net.HttpWebResponse)response).StatusDescription);
        // Get the stream containing content returned by the server.
        dataStream = response.GetResponseStream();
        // Open the stream using a StreamReader for easy access.
        System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
        // Read the content.
        string responseFromServer = reader.ReadToEnd();
        // Display the content.
        Console.WriteLine(responseFromServer);
        // Clean up the streams.
        reader.Close();
        dataStream.Close();
        response.Close();

        return responseFromServer;
    }


    System.Collections.Generic.Dictionary<string, string> parseStringToMap(string text)
    {
        System.Collections.Generic.Dictionary<string, string> retMap = new System.Collections.Generic.Dictionary<string, string>();
        string[] arText = text.Split('&');
        for (int i = 0; i < arText.Length; i++)
        {
            string[] arKeyVal = arText[i].Split('=');
            retMap.Add(arKeyVal[0], arKeyVal[1]);
        }
        return retMap;
    }


    // fake server 막기 위해 추가 2016.05.16 김종현
	private string makeSignatureAuth(Dictionary<string, string> parameters) {

		if (parameters == null || parameters.Count == 0) {
			throw new Exception("Parameters can not be empty.");
		}

		string stringToSign = "";															//반환용 text
        string mid          = parameters["mid"];                                                //mid
		string tstamp       = parameters["tstamp"];												//auth timestamp
		string MOID         = parameters["MOID"];												//OID
		string TotPrice     = parameters["TotPrice"];											//total price
		string tstampKey    = parameters["tstamp"].Substring(parameters["tstamp"].Length-1);	// timestamp 마지막 자리 1자리 숫자


		switch (uint.Parse(tstampKey)){
		case 1 :
			stringToSign = "MOID=" + MOID + "&mid=" + mid + "&tstamp=" + tstamp ;
			break;
		case 2 :
			stringToSign = "MOID=" + MOID + "&tstamp=" + tstamp + "&mid=" + mid ;
			break;
		case 3 :
			stringToSign = "mid=" + mid + "&MOID=" + MOID + "&tstamp=" + tstamp ;
			break;
		case 4 :
			stringToSign = "mid=" + mid + "&tstamp=" + tstamp + "&MOID=" + MOID ;
			break;
		case 5 :
			stringToSign = "tstamp=" + tstamp + "&mid=" + mid + "&MOID=" + MOID ;
			break;
		case 6 :
			stringToSign = "tstamp=" + tstamp + "&MOID=" + MOID + "&mid=" + mid ;
			break;
		case 7 :
			stringToSign = "TotPrice=" + TotPrice + "&mid=" + mid + "&tstamp=" + tstamp ;
			break;
		case 8 :
			stringToSign = "TotPrice=" + TotPrice + "&tstamp=" + tstamp + "&mid=" + mid ;
			break;
		case 9 :
			stringToSign = "TotPrice=" + TotPrice + "&MOID=" + MOID + "&tstamp=" + tstamp ;
			break;
		case 0 :
			stringToSign = "TotPrice=" + TotPrice + "&tstamp=" + tstamp + "&MOID=" + MOID ;
			break;
		}


        //Console.WriteLine("stringToSign="+stringToSign) ;
        //Console.WriteLine("tstampKey,tstamp=" + tstampKey + "," + tstamp);

        string signature = ComputeHash(stringToSign);            // sha256 처리하여 hash 암호화

		return signature;
	}

}
