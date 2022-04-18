using ConsoleApp1.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiLibrary.Api
{
    public class ApiCallsHandler
    {

        private static ApiCallsHandler Handler { get; set;}

        public static ApiCallsHandler GetHandler()
        {
            if(Handler == null)
            {
                Handler = new ApiCallsHandler();
            }

            return Handler;
        }

        private HttpClient currentClient = ApiHelper.ApiClient;

        public async Task<TOut> PostAsync<TIn, TOut>(string endpoint, TIn body)
        {
            try
            {
                //using (var client = currentClient)
                //{

                    var serialized = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = await currentClient.PostAsync(endpoint, serialized))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<TOut>(responseBody);
                    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return default;
        }

        public T Get<T>(string endpoint)
        {
            Task <T> task = GetAsync<T>(endpoint);
            try
            {
                Task.WaitAll(task);
                return task.Result;
            }catch(Exception e)
            {
                Console.WriteLine("Exception Occured during fetch", e);
            }
            return default;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                //using (var client = currentClient)
                //{

                    Console.WriteLine(currentClient.BaseAddress + endpoint);

                    using (HttpResponseMessage response = await currentClient.GetAsync(endpoint))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        return JsonConvert.DeserializeObject<T>(responseBody);
                    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return default;
        }

    }
}
