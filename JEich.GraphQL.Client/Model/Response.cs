using JEich.GraphQL.Model.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model
{
    public class Response
    {
        public IReadOnlyCollection<Error> Errors { get; set; }
        public IReadOnlyCollection<object> Result { get; set; }
        public bool WasSuccessful { get; set; }
    }
}
