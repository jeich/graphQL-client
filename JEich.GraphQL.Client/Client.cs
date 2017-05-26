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
            where TResponse : class, new()
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
                            var dataObj = child.Value.ToObject<JObject>();
                            string typeName = typeof(TResponse).Name.ToLower();
                            if (dataObj[typeName] == null)
                            {
                                throw new Exception("Data format invalid");//TODO: tidy up
                            }
                            var item = DeserializeObjectAndChildren<TResponse>(dataObj[typeName].ToObject<JObject>());
                            return new Response<TResponse>
                            {
                                Result = item,
                                WasSuccessful = true
                            };
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

        protected static T DeserializeObjectAndChildren<T>(JObject obj)
            where T : new()
        {
            return (T)DeserializeObjectAndChildren(obj, typeof(T));
        }
        protected static object DeserializeObjectAndChildren(JObject obj, Type t)
        {
            var result = Activator.CreateInstance(t);
            var properties = t.GetRuntimeProperties().ToDictionary(x => x.Name.ToLower(), x => x);
            foreach (var token in obj)
            {
                if (properties.ContainsKey(token.Key))
                {
                    if (token.Value.Type == JTokenType.Object)
                    {
                        properties[token.Key].SetValue(result, DeserializeObjectAndChildren(token.Value.ToObject<JObject>(), properties[token.Key].PropertyType));
                    }
                    else
                    {
                        properties[token.Key].SetValue(result, token.Value.ToObject(properties[token.Key].PropertyType));
                    }
                }
            }
            return result;
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
