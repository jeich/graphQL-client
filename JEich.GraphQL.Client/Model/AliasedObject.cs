using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Model
{
    public class AliasedObject : RequestObject
    {
        public AliasedObject(object obj, string alias) : base(obj, alias)
        {

        }
    }
}
