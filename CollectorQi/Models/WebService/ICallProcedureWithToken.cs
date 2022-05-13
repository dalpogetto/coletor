using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectorQi.Models
{
    public interface ICallProcedureWithToken
    {
        string userLogin(string user);
        string callProcedureWithToken(string pStrToken, string pStrProgramName, string pStrProcedureName, string pStrJsonArgs);
        string GetWSUrl();

    }
}
