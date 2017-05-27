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
            SetupMessageHandler(Responses.Basic);

            var response = await _client.GetAsync<Data.Hero, Data.Hero>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", response.Result.Name);
        }

        [TestMethod]
        public async Task GetAsync_NestedObject_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedObject);

            var response = await _client.GetAsync<Data.LonelyHero, Data.LonelyHero>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", response.Result.Name);
            Assert.IsNotNull(response.Result.Friend);
            Assert.AreEqual("Luke Skywalker", response.Result.Friend.Name);
        }

        [TestMethod]
        public async Task GetAsync_NestedObjectWithComment_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedObjectWithComment);

            var response = await _client.GetAsync<Data.LonelyHero, Data.LonelyHero>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", response.Result.Name);
            Assert.IsNotNull(response.Result.Friend);
            Assert.AreEqual("Luke Skywalker", response.Result.Friend.Name);
        }

        [TestMethod]
        public async Task GetAsync_NestedArray_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedArray);

            var response = await _client.GetAsync<Data.Hero, Data.Hero>();
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", response.Result.Name);
            Assert.IsNotNull(response.Result.Friends);
            Assert.AreEqual("Luke Skywalker", response.Result.Friends[0].Name);
            Assert.AreEqual("Han Solo", response.Result.Friends[1].Name);
            Assert.AreEqual("Leia Organa", response.Result.Friends[2].Name);
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
