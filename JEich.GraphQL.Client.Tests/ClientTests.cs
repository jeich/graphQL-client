using System;
using QLClient = JEich.GraphQL.Client;
using System.Threading.Tasks;
using System.Net.Http;
using Moq;
using System.Text;
using System.Threading;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using NUnit.Framework;
using JEich.GraphQL.Model;

namespace JEich.GraphQL.Tests
{
    [TestFixture]
    public class ClientTests
    {
        private readonly Uri _baseUri = new Uri("http://localhost/graphql");
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _mockMessageHandler = new Mock<HttpMessageHandler>();
        private QLClient _client;

        [SetUp]
        public void SetUp()
        {
            _httpClient = new HttpClient(_mockMessageHandler.Object);
            _client = new QLClient(_baseUri, () => _httpClient);
        }

        [Test]
        public async Task GetAsync_OneField_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.Basic, HttpStatusCode.OK);

            var response = await _client.GetAsync(new RequestObject(new Data.Hero()));
            var hero = response.Result.First() as Data.Hero;
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", hero.Name);
        }

        [Test]
        public async Task GetAsync_ResponseContainsAdditionalField_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedObject, HttpStatusCode.OK);

            var response = await _client.GetAsync(new RequestObject(new Data.LonelyHero()));
            var hero = response.Result.First() as Data.LonelyHero;
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", hero.Name);
        }

        [Test]
        public async Task GetAsync_NestedObject_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedObject, HttpStatusCode.OK);

            var response = await _client.GetAsync(new RequestObject(new Data.LonelyHero()));
            var hero = response.Result.First() as Data.LonelyHero;
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", hero.Name);
            Assert.IsNotNull(hero.Friend);
            Assert.AreEqual("Luke Skywalker", hero.Friend.Name);
        }

        [Test]
        public async Task GetAsync_MultipleRequestObjects_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.MultipleObjects, HttpStatusCode.OK);

            var response = await _client.GetAsync(new RequestObject(new Data.LonelyHero()), new RequestObject(new Data.Hero()));
            var hero = response.Result.First() as Data.LonelyHero;
            var anotherHero = response.Result.Skip(1).First() as Data.Hero;

            Assert.That(response.WasSuccessful);
            Assert.That(hero.Name, Is.EqualTo("R2-D2"));
            Assert.That(hero.Friend, Is.Not.Null);
            Assert.That(hero.Friend.Name, Is.EqualTo("Luke Skywalker"));

            Assert.That(anotherHero.Name, Is.EqualTo("Obi-wan Kanobi"));
        }

        [Test]
        public async Task GetAsync_MultipleAliasedRequestObjects_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.AliasedObjects, HttpStatusCode.OK);

            var response = await _client.GetAsync(new AliasedObject(new Data.Hero(), "Qui-gon Xin"), new AliasedObject(new Data.Hero(), "Obi-wan Kanobi"));
            var hero = response.Result.First() as Data.Hero;
            var anotherHero = response.Result.Skip(1).First() as Data.Hero;

            Assert.That(response.WasSuccessful);
            Assert.That(hero.Name, Is.EqualTo("Obi-wan Kanobi"));
            Assert.That(hero.Friends[0], Is.Not.Null);
            Assert.That(hero.Friends[0].Name, Is.EqualTo("Luke Skywalker"));

            Assert.That(anotherHero.Name, Is.EqualTo("Qui-gon Xin"));
        }

        [Test]
        public async Task GetAsync_NestedObjectWithComment_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedObjectWithComment, HttpStatusCode.OK);

            var response = await _client.GetAsync(new RequestObject(new Data.LonelyHero()));
            var hero = response.Result.First() as Data.LonelyHero;
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", hero.Name);
            Assert.IsNotNull(hero.Friend);
            Assert.AreEqual("Luke Skywalker", hero.Friend.Name);
        }

        [Test]
        public async Task GetAsync_NestedArray_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedArray, HttpStatusCode.OK);

            var response = await _client.GetAsync(new RequestObject(new Data.Hero()));
            var hero = response.Result.First() as Data.Hero;
            Assert.IsTrue(response.WasSuccessful);
            Assert.AreEqual("R2-D2", hero.Name);
            Assert.IsNotNull(hero.Friends);
            Assert.AreEqual("Luke Skywalker", hero.Friends[0].Name);
            Assert.AreEqual("Han Solo", hero.Friends[1].Name);
            Assert.AreEqual("Leia Organa", hero.Friends[2].Name);
        }

        [Test]
        public async Task GetAsync_NestedArrayWithErrors_DeserializesCorrectly()
        {
            SetupMessageHandler(Responses.NestedArrayWithErrors, HttpStatusCode.BadRequest);

            var response = await _client.GetAsync(new RequestObject(new Data.Hero()));
            var hero = response.Result.First() as Data.Hero;
            Assert.IsFalse(response.WasSuccessful);
            Assert.AreEqual("R2-D2", hero.Name);
            Assert.IsNotNull(hero.Friends);
            Assert.AreEqual("Luke Skywalker", hero.Friends[0].Name);
            Assert.AreEqual("Leia Organa", hero.Friends[2].Name);
            Assert.IsNotNull(response.Errors);
            var errors = response.Errors.ToList();
            Assert.AreNotEqual(0, response.Errors.Count());
            Assert.AreEqual("Name for character with ID 1002 could not be fetched.", errors[0].Message);
            Assert.AreEqual(6, errors[0].Locations.Single().Line);
            Assert.AreEqual(7, errors[0].Locations.Single().Column);
            Assert.That(errors[0].Path, Is.EquivalentTo(new string[] { "hero", "heroFriends", "1", "name" }));
        }

        private void SetupMessageHandler(string response, HttpStatusCode status)
        {
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(response, Encoding.UTF8, "application/json")
                }));
        }
    }
}
