using JEich.GraphQL.Model.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model
{
    public class Response<T> where T : class
    {
        public IReadOnlyCollection<Error> Errors { get; set; }
        public T Result { get; set; }
        public bool WasSuccessful { get; set; }
    }
}
