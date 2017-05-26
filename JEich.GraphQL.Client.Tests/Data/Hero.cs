using System;
using System.Collections.Generic;
using System.Text;

namespace JEich.GraphQL.Tests.Data
{
    public class Hero
    {
        public string Name { get; set; }
        public Weapon Weapon { get; set; }
    }

    public class Weapon
    {
        public string Name { get; set; }
    }
}
