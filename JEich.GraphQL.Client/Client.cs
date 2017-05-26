using JEich.GraphQL.Model;
using Http = JEich.GraphQL.Model.Http;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JEich.GraphQL
{
    public class Client
    {
        private readonly Uri _baseUri;
        private Func<HttpClient> _httpClientProvider;

        public Client(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Client(Uri baseUri, Func<HttpClient> httpClientProvider) : this(baseUri)
        {
            _httpClientProvider = httpClientProvider;
        }

        public async Task<Response<TResponse>> GetAsync<TRequest, TResponse>()
            where TRequest : class 
            where TResponse : class
        {
            using (var client = GetHttpClient())
            {
                var httpRequest = new HttpRequestMessage
                {
                    RequestUri = _baseUri,
                    Method = HttpMethod.Get,
                    Content = new StringContent(CreateRequestObject<TRequest>(), Encoding.UTF8, "application/json")
                };
                var httpResponse = await client.SendAsync(httpRequest);
                if (httpResponse.IsSuccessStatusCode)
                {
                    string content = await httpResponse.Content.ReadAsStringAsync();
                    var jobject = JObject.Parse(content);
                    foreach (var child in jobject)
                    {
                        if (child.Key == "data")
                        {
                            foreach (var dataChild in child.Value.Value<JObject>())
                            {
                                if (dataChild.Key == typeof(TResponse).Name.ToLower())
                                {
                                    return new Response<TResponse>
                                    {
                                        Result = dataChild.Value.ToObject<TResponse>(),
                                        WasSuccessful = true
                                    };
                                }
                            }
                        }
                    }
                    var response = JsonConvert.DeserializeObject<Http.Response<TResponse>>(content);
                    return new Response<TResponse>
                    {
                        Result = null,
                        WasSuccessful = false
                    };
                }
                else
                {
                    return new Response<TResponse>
                    {
                        Result = null,
                        WasSuccessful = false
                    };
                }
            }
        }

        protected static string CreateRequestObject<T>()
        {
            return $"{{ {typeof(T).Name.ToLower()} {{ {SerializeInnerObject<T>()} }} }}";
        }

        protected static string SerializeInnerObject<T>()
        {
            return string.Join(Environment.NewLine, typeof(T).GetRuntimeProperties().Select(x => x.Name.ToLower()));
        }

        protected HttpClient GetHttpClient()
        {
            return _httpClientProvider == null
                ? new HttpClient()
                : _httpClientProvider();
        }
    }
}
