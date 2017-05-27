using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model
{
    public class RequestObject
    {
        protected readonly object _obj;
        protected readonly string _alias;

        public RequestObject(object obj) : this(obj, obj.GetType().Name.ToLower())
        {

        }

        protected RequestObject(object obj, string name)
        {
            _obj = obj;
            _alias = name;
        }

        public object Object => _obj;
        public string Name => _alias;
    }
}
