using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model.Http
{
    public class Response<T>
    {
        public T Data { get; set; }
    }
}
