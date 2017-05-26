using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLClient = JEich.GraphQL.Client;
using System.Threading.Tasks;
using System.Net.Http;
using Moq;
using System.Text;
using System.Threading;
using Moq.Protected;
using Newtonsoft.Json;

namespace JEich.GraphQL.Tests
{
    [TestClass]
    public class ClientTests
    {
        private readonly Uri _baseUri = new Uri("http://localhost/graphql");
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _mockMessageHandler = new Mock<HttpMessageHandler>();
        private QLClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _httpClient = new HttpClient(_mockMessageHandler.Object);
            _client = new QLClient(_baseUri, () => _httpClient);
        }

        [TestMethod]
        public async Task GetAsync_OneField_DeserializesCorrectly()
        {
            SetupGraphQLResponse(@"
                ""hero"": {
                      ""name"": ""R2-D2""
                    }
            ");

            var response = await _client.GetAsync<Data.Hero, Data.Hero>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", response.Result.Name);
        }

        [TestMethod]
        public async Task GetAsync_NestedOnject_DeserializesCorrectly()
        {
            SetupGraphQLResponse(@"
                ""hero"": {
                      ""name"": ""R2-D2"",
                        ""weapon"": {
                            ""name"": ""axe""
                        }
                    }
            ");

            var response = await _client.GetAsync<Data.Hero, Data.Hero>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", response.Result.Name);
            Assert.IsNotNull(response.Result.Weapon);
            Assert.IsNotNull("axe", response.Result.Weapon.Name);
        }

        private void SetupGraphQLResponse(string data)
        {
            SetupMessageHandler($@"{{
                ""data"": {{
                    {data}
                }}
            }}");
        }

        private void SetupMessageHandler(string response)
        {
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(response, Encoding.UTF8, "application/json")
                }));
        }
    }
}
