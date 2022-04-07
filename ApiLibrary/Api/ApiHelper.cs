using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Api
{
    public static class ApiHelper
    {
        // Is static to open only one per application
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient(string baseAddr = "")
        {
            if(ApiClient == null)
            {
                // Basic Api client setup
                ApiClient = new HttpClient();
                ApiClient.BaseAddress = new Uri(baseAddr);
                ApiClient.DefaultRequestHeaders.Accept.Clear();
                // Header defines json input type from API
                ApiClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

    }
}
