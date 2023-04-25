using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Internals;

namespace CollectorQualiIT.Resources.DataBaseHelper
{
/*
    public class ServiceDS
    {
        private static string UrlSoap = "https://santacolomba-dts-teste.totvscloud.com.br/wsexecbo/WebServiceExecBO?wsdl";
        private static string ProgramName = "qip/qi0004.p"; 
        private static string ProcedureNameGetItem = "GetItem";

        private static BasicHttpBinding CreateBasicHttp()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                MaxBufferPoolSize = 2147483647,
                AllowCookies = false,
                BypassProxyOnLocal = false,
                UseDefaultWebProxy = true,
                TransferMode = TransferMode.Streamed,
                TextEncoding = UTF8Encoding.Default
            };
            TimeSpan timeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            binding.CloseTimeout = timeout;
            return binding;
        }

        //public static async System.Threading.Tasks.Task<string> callProcedureAsync(string pStrProgName, string pStrProcedureName, string pStrParameters)
        public static string callProcedureAsync(string pStrProgName, string pStrProcedureName, string pStrParameters)
        {
            string strResponse = "";
            try
            {
                BasicHttpBinding binding = CreateBasicHttp();
                var endpoint = new EndpointAddress(UrlSoap);

                //ExecBOServiceEndpointClient ws = new ExecBOServiceEndpointClient(binding, endpoint);

                ExecBOServiceEndpointClient execboserviceendpointclient = new ExecBOServiceEndpointClient(binding, endpoint);

                
                //ws.callProcedureAsync();
                //await ws.OpenAsync();
                userLogin userlogin = new userLogin();
                //userAndPasswordLogin userandpasswordlogin = new userAndPasswordLogin();
                 userlogin.arg0 = "super";
                //userandpasswordlogin.arg0 = "";

                //userAndPasswordLoginRequest userandpasswordloginrequest = new userAndPasswordLoginRequest(userandpasswordlogin);
                userLoginRequest userloginrequest = new userLoginRequest(userlogin);
                //ws.userLoginAsync(userloginrequest).Wait();

                userLoginResponse userloginresponse = new userLoginResponse();

                userLoginResponse1 userloginresponse1 = new userLoginResponse1(userloginresponse);

                userloginrequest.userLogin.arg0 = userlogin.arg0;

                //userLoginResponse userloginresponse = new userLoginResponse();
                //userloginresponse = await ws.userLoginAsync(userloginrequest);
                //userLoginResponse1 userloginresponse1 = new userLoginResponse1(userloginrequest);

                //ws.OpenAsync().RunSynchronously();
                //await execboserviceendpointclient.OpenAsync();
                //userloginresponse1 = execboserviceendpointclient.userLoginAsync(userlogin).Result; //(userloginrequest);

                //callProcedure callprocedure = new callProcedure();
                
                callProcedureWithToken callprocedurewithtoken = new callProcedureWithToken();
                callprocedurewithtoken.arg0 = userloginresponse1.userLoginResponse.@return;
                callprocedurewithtoken.arg1 = pStrProgName;
                callprocedurewithtoken.arg2 = pStrProcedureName;
                callprocedurewithtoken.arg3 = pStrParameters;

                callProcedureWithTokenRequest callprocedurewithtokenrequest = new callProcedureWithTokenRequest(callprocedurewithtoken);
                //callProcedureRequest callprocedurerequest = new callProcedureRequest(callprocedure);

                //callProcedureWithTokenResponse1 callprocedurewithtokenresponse1 = execboserviceendpointclient.callProcedureWithTokenAsync(callprocedurewithtoken).Result;

                //strResponse = callprocedurewithtokenresponse1.callProcedureWithTokenResponse.@return;
            }
            catch (Exception ex)
            {
                Log.Warning("SQLite Exception", ex.Source + " " +  ex.Message);
            }
            return strResponse;
        }


        public  string GetDeposito()
        {

            try
            {

                string strJson = "[{                                                              " +
                                        "       \"name\": \"ttDeposito\",                              " +
                                        "       \"type\": \"output\",                                   " +
                                        "       \"dataType\": \"temptable\",                            " +
                                        "       \"value\": {                                            " +
                                        "           \"name\": \"ttDeposito\",                        " +
                                        "           \"fields\": [{                                      " +
                                        "                   \"name\": \"codDepos\",                   " +
                                        "                   \"label\": \"codDepos\",                  " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }, {                                            " +
                                        "                   \"name\": \"nome\",                     " +
                                        "                   \"label\": \"nome\",                    " +
                                        "                   \"type\": \"character\"                     " +
                                        "               }" +
                                        "           ],                                                  " +
                                        "           \"records\": [{}                                    " +
                                        "           ]                                                   " +
                                        "       }                                                       " +
                                         "  }                                                           " +
                                        "]";

                Console.WriteLine("### 2 ");

                 string strToken = App.CallProcedureWithToken.userLogin(UsuarioLogin);

                 Console.WriteLine("### 3 ");

                 strJson = strJson.Trim().Replace("\t", "");

                 Console.WriteLine("### 4 ");



                 String strResponse = App.CallProcedureWithToken.callProcedureWithToken(strToken, ProgramNameCollector, ProcedureNameGetDeposito, strJson);

                 return "Chegou StrResponse " + strResponse;

                 Console.WriteLine("### 5 ");

                 Console.WriteLine("######## " + strResponse);

                 var lstRecords = DeserializeJsonWS<List<ModelDeposito>>(strResponse.ToString());


                strJson = strJson.Trim().Replace("\t", "");

                string strResponse = callProcedureAsync(ProgramName, "GetDeposito", strJson);

                return strResponse;
                
                var lstRecords = DeserializeJsonWS<List<ModelRelogioPontoValue>>(strResponse);




                // return lstRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine("### Erro: " + ex.Message);
                return "Erro Cat" + ex.Message;
            }

        }

        public string GetItem(int byVersao)
        {
            string strJsonItem = "[{ " +
                                    " \"dataType\": \"integer\", " +
                                    " \"value\": \"1\", " +
                                    " \"name\": \"piSeqIntegr\", " +
                                    " \"type\": \"input\" " +
                                    " }, { " +
                                    " \"name\": \"ttItem\", " +
                                    " \"type\": \"output\", " +
                                    " \"dataType\": \"temptable\", " +
                                    " \"value\": { " +
                                    " \"name\": \"ttItem\", " +
                                    " \"fields\": [{ " +
                                    " \"name\": \"ItCodigo\", " +
                                    " \"label\": \"itCodigo\", " +
                                    " \"type\": \"character\" " +
                                    " }, { " +
                                    " \"name\": \"DescItem\", " +
                                    " \"label\": \"DescItem\", " +
                                    " \"type\": \"character\" " +
                                    " }, { " +
                                    " \"name\": \"DepositoPad\", " +
                                    " \"label\": \"DepositoPad\", " +
                                    " \"type\": \"character\" " +
                                    " }, { " +
                                    " \"name\": \"CodLocaliz\", " +
                                    " \"label\": \"CodLocaliz\", " +
                                    " \"type\": \"character\" " +
                                    " }, { " +
                                    " \"name\": \"LocUnica\", " +
                                    " \"label\": \"LocUnica\", " +
                                    " \"type\": \"logical\" " +
                                    " }, { " +
                                    " \"name\": \"TipoConEst\", " +
                                    " \"label\": \"TipoConEst\", " +
                                    " \"type\": \"integer\" " +
                                    " }, { " +
                                    " \"name\": \"ContrQualid\", " +
                                    " \"label\": \"ContrQualid\", " +
                                    " \"type\": \"logical\" " +
                                    " }, { " +
                                    " \"name\": \"LogOrigExt\", " +
                                    " \"label\": \"LogOrigExt\", " +
                                    " \"type\": \"logical\" " +
                                    " }, { " +
                                    " \"name\": \"Fraciona\", " +
                                    " \"label\": \"Fraciona\", " +
                                    " \"type\": \"logical\" " +
                                    " }, { " +
                                    " \"name\": \"Un\", " +
                                    " \"label\": \"Un\", " +
                                    " \"type\": \"character\" " +
                                    " } " +
                                    " ], " +
                                    " \"records\": [{} " +
                                    " ] " +
                                    " } " +
                                    " } " +
                                    " ] ";

            string strJson = strJsonItem.Trim().Replace("\t", "");

            string strResponse = callProcedureAsync(ProgramName, ProcedureNameGetItem, strJson);

            //var lstRecords = DeserializeJsonWS<List<ModelRelogioPontoValue>>(strResponse);

            //return lstRecords;
            return strResponse;
        }

    }*/
}
