using JEich.GraphQL.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JEich.GraphQL.Tests
{
    [TestFixture]
    public class GraphQLSerializerTests
    {
        private Regex _regex;
        [SetUp]
        public void SetUp()
        {
            _regex = new Regex(@"[^\S\r\n]", RegexOptions.Compiled);
        }

        [Test]
        public void SerializeRequestObject_Basic_CorrectlySerializes()
        {
            var requestObject = new RequestObject(new Data.Hero());

            string result = GraphQLSerializer.SerializeRequestObject(requestObject);

            AssertEqualIgnoringWhiteSpace(Requests.Basic, result);
        }

        [Test]
        public void SerializeRequestObject_WithArguments_CorrectlySerializes()
        {
            var requestObject = new RequestObject(new Data.Hero
            {
                Name = "Obi-wan Kanobi"
            });

            string result = GraphQLSerializer.SerializeRequestObject(requestObject);

            AssertEqualIgnoringWhiteSpace(Requests.Arguments, result);
        }


        private void AssertEqualIgnoringWhiteSpace(string expected, string actual)
        {
            Assert.That(_regex.Replace(actual, string.Empty), Is.EqualTo(_regex.Replace(expected, string.Empty)));
        }
    }
}
