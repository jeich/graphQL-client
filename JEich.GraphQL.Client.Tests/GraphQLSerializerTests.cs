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

        #region Arguments

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

        [Test]
        public void SerializeRequestObject_WithArguments_CorrectlySerializesAndSpecifiedArgumentsOmitted()
        {
            var requestObject = new RequestObject(new Data.Player { Id = "4" });

            string result = GraphQLSerializer.SerializeRequestObject(requestObject);

            AssertEqualIgnoringWhiteSpace(Requests.SpecifiedArgumentsNotRequested, result);
        }

        [Test]
        public void SerializeRequestObject_WithPrimitiveArguments_CorrectlySerializes()
        {
            var requestObject = new RequestObject(new Data.PrimitivePlayer { Id = 4 });

            string result = GraphQLSerializer.SerializeRequestObject(requestObject);

            AssertEqualIgnoringWhiteSpace(Requests.PrimitiveArguments, result);
        }

        [Test]
        public void SerializeRequestObject_WithClrPrimitiveArgument_CorrectlySerializes()
        {
            var requestObject = new RequestObject(new Data.PrimitivePlayer());

            string result = GraphQLSerializer.SerializeRequestObject(requestObject);

            AssertEqualIgnoringWhiteSpace(Requests.ClrUnspecifiedPrimitiveArguments, result);
        }

        [Test]
        public void SerializeRequestObject_WithNullablePrimitiveArgument_CorrectlySerializes()
        {
            var requestObject = new RequestObject(new Data.NullablePrimitivePlayer());

            string result = GraphQLSerializer.SerializeRequestObject(requestObject);

            AssertEqualIgnoringWhiteSpace(Requests.ClrPrimitiveArguments, result);
        }

        #endregion Arguments

        #region Aliases

        [Test]
        public void SerializeRequestObject_AliasedWithArguments_CorrectlySerializes()
        {
            var requestObject = new AliasedObject(new Data.Hero
            {
                Name = "Darth Vader"
            }, "empireHero");
            var requestObject2 = new AliasedObject(new Data.Hero
            {
                Name = "Luke Skywalker"
            }, "jediHero");

            string result = GraphQLSerializer.SerializeRequestObjects(requestObject, requestObject2);

            AssertEqualIgnoringWhiteSpace(Requests.Aliased, result);
        }

        #endregion Aliases


        private void AssertEqualIgnoringWhiteSpace(string expected, string actual)
        {
            Assert.That(_regex.Replace(actual, string.Empty), Is.EqualTo(_regex.Replace(expected, string.Empty)));
        }
    }
}
