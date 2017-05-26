using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model
{
    public class Response<T> where T : class
    {
        public bool WasSuccessful { get; set; }
        public T Result { get; set; }
    }
}
