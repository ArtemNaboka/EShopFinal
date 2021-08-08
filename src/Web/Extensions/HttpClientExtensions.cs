using Microsoft.eShopWeb.Web.Settings;
using System;
using System.Net.Http;

namespace Microsoft.eShopWeb.Web.Extensions
{
    public static class HttpClientExtensions
    {
        public static void ConfigureForHttpFunction(this HttpClient client, IHttpFuntionSettings funtionSettings)
        {
            client.BaseAddress = new Uri(funtionSettings.FunctionUri);
            client.DefaultRequestHeaders.Add("x-functions-key", funtionSettings.FunctionKey);
        }
    }
}
