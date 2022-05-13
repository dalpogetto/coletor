using System.Collections.Generic;
using CollectorQi.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(CollectorQi.Droid.CallProcedureWithToken))]
namespace CollectorQi.Droid
{
    public sealed class CallProcedureWithToken : ICallProcedureWithToken
    {
        santacolomba_dts.totvscloud.com.br.WebServiceExecBO ws;
        santacolomba_dts.totvscloud.com.br.userLogin login;
        santacolomba_dts.totvscloud.com.br.callProcedureWithToken callProc;

        public CallProcedureWithToken()
        {
            ws = new santacolomba_dts.totvscloud.com.br.WebServiceExecBO()
            {
                //Url = "https://santacolomba-dts-teste.totvscloud.com.br/wsexecbo/WebServiceExecBO?WSDL",
                Url = "https://santacolomba-dts-prod.totvscloud.com.br/wsexecbo/WebServiceExecBO?wsdl",
                Timeout = -1
            };
        }

        public string userLogin(string user)
        {
            login = new santacolomba_dts.totvscloud.com.br.userLogin()
            {
                arg0 = user
            };

            return (ws.userLogin(login).@return);
        }

        public string callProcedureWithToken(string pStrToken, string pStrProgramName, string pStrProcedureName, string pStrJsonArgs)
        {
            callProc = new santacolomba_dts.totvscloud.com.br.callProcedureWithToken()
            {
                arg0 = pStrToken,
                arg1 = pStrProgramName,
                arg2 = pStrProcedureName,
                arg3 = pStrJsonArgs
            };

            return (ws.callProcedureWithToken(callProc).@return);
            /*
            login = new santacolomba_dts.totvscloud.com.br.userLogin()
            {
                arg0 = user
            };

            return (ws.userLogin(login).@return);*/
        }

        public string GetWSUrl()
        {
            return ws.Url;
        }
    }
}