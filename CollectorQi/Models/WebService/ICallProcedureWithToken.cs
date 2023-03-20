namespace CollectorQi.Models
{
    public interface ICallProcedureWithToken
    {
        string userLogin(string user);
        string callProcedureWithToken(string pStrToken, string pStrProgramName, string pStrProcedureName, string pStrJsonArgs);
        string GetWSUrl();

    }
}