using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CollectorQi.Services
{
    public interface IHTTPClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
