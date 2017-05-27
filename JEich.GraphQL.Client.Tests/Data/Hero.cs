using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace JEich.GraphQL.Tests.Data
{
    public class Hero
    {
        public string Name { get; set; }
        public Friend[] Friends { get; set; }
    }
}
