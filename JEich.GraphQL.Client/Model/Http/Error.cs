using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model.Http
{
    public class Error
    {
        public Error()
        {
            Locations = new List<Location>();
            Path = new List<string>();
        }

        public string Message { get; set; }
        public IReadOnlyCollection<Location> Locations { get; set; }
        public IReadOnlyCollection<string> Path { get; set; }
    }
}
