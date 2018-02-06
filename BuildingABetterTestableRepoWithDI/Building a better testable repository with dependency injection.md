# Building a better testable repository with dependency injection
##The problem

The question is: how do you build a data access layer that is easier to test, and maybe even better, how do you build a repository with Entity Framework Core that is usable from both *"classic"* .NET and .NET Core, and how do you enable testing with different databases? The technique I am going to describe also works with Entity Framework 6, ADO.NET, and other database technologies, the details are left as an exercise!

When building integration tests for your data access layer you will encounter the problem of using a different database connection for testing. Many testing frameworks allow you to use a different configuration file for testing, so you might think problem solved. But I want to use different databases for different tests! How do you specify different connection strings for different tests? You could of course use a different test project for each connection string, but read on!
## The solution
The solution is simple. The connection itself should be seen as a dependency. Your data access layer should not be responsible for figuring out where to get the connection string, this should be injected into your code.
