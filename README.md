# JEich.GraphQL.Client
A strongly-typed C# GraphQL client. Requires .NET Standard 1.3+. Very work-in-progess.


| master |
| ----- |
| ![build](https://travis-ci.org/jeich/graphQL-client.svg?branch=master) |

## Features

This client will use reflection on the supplied type to determine the properties to request from the GraphQL server.
Aliases are supported via the AliasedObject class for top level objects. For lower level aliases, the name is determined via the property name.
e.g.


<table width="100%">
<tr>
<th width="50%">
C#
</th>
<th>
GraphQL Request
</th>
</tr>
<tr>
<td>
<pre>
public class Hero
{
	...
    public Friend AliasedSpecialFriend { get; set; }
}

public class Friend
{
	...
}

</pre>
</td>
<td><pre>
{
  hero {
    name
    aliasedSpecialFriend: friend {
      name
    }
  }
}
</pre>
</td>
</tr>
</table>

Any properties set on the supplied object will be used as arguments in the query and (TODO) will not be returned in the response.

## Examples

### Basic Query


<table width="100%">
<tr>
<th width="50%">
C#
</th>
<th>
GraphQL Request
</th>
</tr>
<tr>
<td>
<pre>
public class Hero
{
   public string Name { get; set; }
}

var requestObject = new RequestObject(new Hero());
var response = await client.GetAsync(requestObject);
var hero = response.Result.First() as Hero;
</pre>
</td>
<td><pre>
{
  hero {
    name
  }
}
</pre>
</td>
</tr>
</table>

### Query with Arguments

<table>
<tr>
<th width="50%">
C#
</th>
<th>
GraphQL Request
</th>
</tr>
<tr>
<td>
<pre>
public class Human
{
	public string Id { get; set; }
    public string Name { get; set; }
    public string Height { get; set; }
}

var human = new RequestObject(new Human { Id = "1000" });
var response = await client.GetAsync(human);
</pre>
</td>
<td><pre>
{
  human(id: "1000") {
    name
    height
  }
}
</pre>
</td>
</tr>
</table>

### Aliased Query

<table>
<tr>
<th width="50%">
C#
</th>
<th>
GraphQL Request
</th>
</tr>
<tr>
<td>
<pre>
public class Hero
{
    public string Name { get; set; }
    public string Weapon { get; set; }
}

var empireHero = new AliasedObject(new Hero { Name = "Darth Vader" }, "empireHero");
var jediHero = new AliasedObject(new Hero { Name = "Luke Skywalker" }, "jediHero");
var response = await client.GetAsync(empireHero, jediHero);
</pre>
</td>
<td><pre>
{
  empireHero: hero(name: "Darth Vader") {
    name
    weapon
  }
  jediHero: hero(name: "Luke Skywalker") {
    name
    weapon
  }
}
</pre>
</td>
</tr>
</table>

## Powered by

* [JSON.NET](https://json.net)
* [Dotnet core](https://dot.net/)
* [NUnit](https://nunit.org/)
* [Moq](https:/moqthis.com/)
* [TravisCI](https://travis-ci.org/)
