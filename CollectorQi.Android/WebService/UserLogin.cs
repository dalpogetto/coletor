using System.Collections.Generic;
using CollectorQi.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(CollectorQi.Droid.UserLogin))]
namespace CollectorQi.Droid
{
    public sealed class UserLogin : IUserLogin
    {
        santacolomba_dts.totvscloud.com.br.WebServiceExecBO ws;
        santacolomba_dts.totvscloud.com.br.userLogin login;

         public UserLogin()
         {
            ws = new santacolomba_dts.totvscloud.com.br.WebServiceExecBO()
            {
                Url = "https://santacolomba-dts-teste.totvscloud.com.br/wsexecbo/WebServiceExecBO?WSDL"
            };
        }

        /*
        public async Task<string> GetAllCustomers(string criteria = null)
        {
            return await Task.Run(() =>
            {
                


                return (ws.userLogin(login).@return);
            });
        }*/

        public string userLogin(string user)
        {
            login = new santacolomba_dts.totvscloud.com.br.userLogin()
            {
                arg0 = user
            };

            return (ws.userLogin(login).@return);           
        }
    }
}