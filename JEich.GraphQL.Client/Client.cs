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
using System.Collections.Generic;

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
                string content = await httpResponse.Content.ReadAsStringAsync();
                var jobject = JObject.Parse(content);
                TResponse data = null;
                List<Http.Error> errors = new List<Http.Error>();
                foreach (var child in jobject)
                {
                    if (child.Key == "data")
                    {
                        var dataObj = child.Value;
                        string typeName = typeof(TResponse).Name.ToLower();
                        if (dataObj[typeName] == null)
                        {
                            throw new Exception("Data format invalid");//TODO: tidy up
                        }
                        data = DeserializeObjectAndChildren<TResponse>(dataObj[typeName]);
                    }
                    else if (child.Key == "errors")
                    {
                        errors = child.Value.ToObject<List<Http.Error>>();
                    }
                }
                var response = JsonConvert.DeserializeObject<Http.Response<TResponse>>(content);
                return new Response<TResponse>
                {
                    Result = data,
                    WasSuccessful = httpResponse.IsSuccessStatusCode && !errors.Any(),
                    Errors = errors
                };
            }
        }

        protected static T DeserializeObjectAndChildren<T>(JToken token)
            where T : new()
        {
            return (T)DeserializeObjectAndChildren(token, typeof(T));
        }

        protected static object DeserializeObjectAndChildren(JToken token, Type t)
        {
            if (t.IsArray)
            {
                var children = token.Children().ToArray();
                Array arrayResult = Array.CreateInstance(t.GetElementType(), children.Count());
                for (int i = 0; i < token.Children().Count(); i++)
                {
                    arrayResult.SetValue(DeserializeObjectAndChildren(children[i].ToObject<JObject>(), t.GetElementType()), i);
                }
                return arrayResult;
            }
            object result = Activator.CreateInstance(t);
            var properties = t.GetRuntimeProperties().ToDictionary(x => x.Name.ToLower(), x => x);
            foreach (var child in token.Children())
            {
                if (child is JProperty)
                {
                    var property = (JProperty)child;
                    properties[property.Name].SetValue(result, property.Value.ToObject(properties[property.Name].PropertyType));
                }
                else if (child is JObject)
                {
                    var parent = child.Parent as JProperty;
                    properties[parent.Name].SetValue(result, DeserializeObjectAndChildren(child, properties[parent.Name].PropertyType));
                }
                else
                {
                    var parent = child.Parent as JProperty;
                    properties[parent.Name].SetValue(result, DeserializeObjectAndChildren(child, properties[parent.Name].PropertyType));
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
