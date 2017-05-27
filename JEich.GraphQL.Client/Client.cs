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
using System.Dynamic;

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

        public async Task<Response> GetAsync(params RequestObject[] objs)
        {
            using (var client = GetHttpClient())
            {
                var httpRequest = new HttpRequestMessage
                {
                    RequestUri = _baseUri,
                    Method = HttpMethod.Get,
                    Content = new StringContent(CreateRequestObjects(objs), Encoding.UTF8, "application/json")
                };
                var requestObjects = objs.ToDictionary(x => x.Name, x => x);
                var httpResponse = await client.SendAsync(httpRequest);
                string content = await httpResponse.Content.ReadAsStringAsync();
                var jobject = JObject.Parse(content);
                var responseObjects = new List<object>();
                List<Http.Error> errors = new List<Http.Error>();
                foreach (var child in jobject)
                {
                    if (child.Key == "data")
                    {
                        var innerData = child.Value;
                        if (innerData.Type == JTokenType.Object)
                        {
                            foreach (var property in innerData.Children<JProperty>())
                            {
                                var name = property.Name;
                                if (requestObjects.ContainsKey(name))
                                {
                                    var requestObject = requestObjects[name];
                                    responseObjects.Add(DeserializeObjectAndChildren(property.Value, requestObject.Object.GetType()));
                                }

                            }
                        }
                    }
                    else if (child.Key == "errors")
                    {
                        errors = child.Value.ToObject<List<Http.Error>>();
                    }
                }
                return new Response
                {
                    Result = responseObjects,
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
                    if (properties.ContainsKey(property.Name))
                        properties[property.Name].SetValue(result, property.Value.ToObject(properties[property.Name].PropertyType));
                }
                else if (child is JObject)
                {
                    var parent = child.Parent as JProperty;
                    if (properties.ContainsKey(parent.Name))
                        properties[parent.Name].SetValue(result, DeserializeObjectAndChildren(child, properties[parent.Name].PropertyType));
                }
                else
                {
                    var parent = child.Parent as JProperty;
                    if (properties.ContainsKey(parent.Name))
                        properties[parent.Name].SetValue(result, DeserializeObjectAndChildren(child, properties[parent.Name].PropertyType));
                }
            }
            return result;
        }

        protected static string CreateRequestObjects(params RequestObject[] objs)
        {
            return $"{{ {string.Join(",", objs.Select(x => x.Name + "{" + SerializeInnerObject(x) + "}"))} }}";
        }

        protected static string SerializeInnerObject(RequestObject obj)
        {
            return string.Join(Environment.NewLine, obj.GetType().GetRuntimeProperties().Select(x => x.Name.ToLower()));
        }

        protected HttpClient GetHttpClient()
        {
            return _httpClientProvider == null
                ? new HttpClient()
                : _httpClientProvider();
        }
    }
}
