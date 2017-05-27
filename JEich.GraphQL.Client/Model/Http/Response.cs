using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model.Http
{
    public class Response<T>
    {
        public Response()
        {
            Errors = new List<Error>();
        }

        public T Data { get; set; }
        public IReadOnlyCollection<Error> Errors { get; set; }
    }
}
