# Integration testing ASP.NET Core with the ScreenPlay pattern

> Summary: Building integration tests for ASP.NET Core can be a lot of work, and even worse, re-work with changing requirements. The **Screenplay Pattern** allows you to write good,  maintainable and easy to understand integration tests.

## Writing Integration Tests for ASP.NET Core

Writing integrations tests and maintaining them typically ends up with **[flaky tests](https://www.lucidchart.com/techblog/2016/12/28/flaky-the-testers-f-word/)**, with lots of repetition and inconsistencies. You end up with the **Large class** anti-pattern, see [Code smell](https://en.wikipedia.org/wiki/Code_smell).

``` csharp
[Fact]
public async Task AUserShouldSeeOnlyTheirTasks()
{
  // Setting up the server with correct configuration
  var contentRoot = Path.Combine(
    Directory.GetCurrentDirectory(), 
    "../../../../../src/WebSite");
  var webHostBuilder = new WebHostBuilder()
  .UseContentRoot(contentRoot)
  .UseEnvironment(EnvironmentName.Development)
  .ConfigureAppConfiguration((hostingContext, config) =>
  {
    var env = hostingContext.HostingEnvironment;

    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

    if (env.IsDevelopment())
    {
      var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
      if (appAssembly != null)
      {
        config.AddUserSecrets(appAssembly, optional: true);
      }
    }
    config.AddEnvironmentVariables();
  })
  .UseStartup<TestStartup>();
  Server = new TestServer(webHostBuilder);

  // Setting up a fake repository
  IToDoRepository repo =
    Server.Host.Services.GetService(typeof(IToDoRepository)) as IToDoRepository;

  // Populate the fake repo with the test data
  TestData.InitialToDos.ForEach(item => repo.AddToDoItem(item));
  await repo.CommitAsync();

  // Setting up the client
  var client = Server.CreateClient();
  // OK, now we are making the actual request
  var response = await client.GetAsync(Uris.Home);
  // Lets assert that we got back the list we populated the
  // repo with in the first place
  var content = await response.Content.ReadAsStringAsync();
  var parser = new HtmlParser();
  var document = await parser.ParseAsync(content);
  var listItems = document.QuerySelectorAll($"{Html.Ul}#todolist>li");
  
  // Wow, finally we get to the real test
  var items = TestData.InitialToDos.Select(item => item.Title);
  foreach (var li in listItems)
  {
    Assert.Contains(li.InnerHtml, items);
  }
}
```

So just look at this test. It's long (and I do admit that you could refactor some
of the code for better re-use) and **hard to read** (a lot of code in a single method
is always hard to read). 
My main problem with this test is that it is **hard to understand**. You actually need
to concentrate on what is going on... And this test does not even need things like 
authentication or XSRF tokens...
 
## Introducing the Screenplay pattern

To reduce maintaining integration tests you can use the **Screenplay pattern**.
Screenplay centers around the user's interaction with the system (based on Behavior 
Driven design aka **BDD**).

Here's the **same test** with the screenplay pattern:

``` csharp
[Fact()]
public async Task AUserShouldSeeOnlyTheirTasks()
{
  // Arrange
  var peter = Actor.Named("Peter").CanUse(Web.Browser<TestStartup>());
  // Act
  await Given.That(peter).HasToDoItems(TestData.InitialToDos)
             .And().CouldGoToItemsPage().Successfully();
  // Assert
  var items = TestData.InitialToDos.Select(item => item.Title);
  peter.Browser().Should().HaveToDoItems(items.ToArray());
}
```

![Holy Moly](https://u2ublogimages.blob.core.windows.net/peter/holymoly.png)

This test is a lot shorter and, more importantly, a lot easier to read! You can almost see the user (oeps, I ment actor) interacting with the web site, as if you would be reading a screen play!

Of course there is a lot of code not shown here. For example the `HaveToDoItems` method:

``` csharp
public static Questions HaveToDoItems(this Questions questions, 
                                           params string[] items)
=> questions.Add(new ShouldHaveToDoItems(items));
```

This is an extension method on the `Questions` class, which is actually used to model the questions that need to be asserted.

The import thing here is the `ShouldHaveToDoItems` class:

``` csharp
public class ShouldHaveToDoItems : Question
{
  private string[] items;

  public ShouldHaveToDoItems(params string[] items)
  {
    this.items = items ?? throw new ArgumentNullException(nameof(items));
  }

  protected override Browser Assert(Browser browser)
  {
    browser.DOM.Should().Contain($"{Html.Ul}#todolist>li", this.items);
    return browser;
  }
}
```

This is a small class (following **[S.O.L.I.D.](https://en.wikipedia.org/wiki/SOLID_(object-oriented_design))** principles) encapsulating the assert. It's highly re-usable and should the UI change, only a small implementation change is required.

---

## Screenplay components

Let's start with the big picture, and then look at each element:

![Screenplay Overview](https://u2ublogimages.blob.core.windows.net/peter/Screenplay%20overview.png)

### Actors have abilities

Take this example, where we want to test a simple interaction:

``` csharp
public async Task UserOpensDefaultPage()
{
  // Arrange
  var peter = Actor.Named("Peter").CanUse(Web.Browser<TestStartup>());
  // Act
  await Given.That(peter).CouldGoToDefaultPage().Successfully();
  // Assert
  peter.Browser().Should().HaveHtmlHeader("U2U Training")
       .And().Should().HaveContentType("text/html; charset=utf-8");
}
```    

An actor is someone or something who wants to accomplish a goal with the system. Actors can be created with the easy to ready syntax:

``` csharp
var peter = Actor.Named("Peter");
```

Actors need abilities, for example an actor needs to use a browser. Abilities can be added with the `CanUse` method:

``` csharp
Actor.Named("Peter").CanUse(Web.Browser<TestStartup>());
```  

Abilities are not limited to the browser ability, for example this could also be an api client for testing REST services:

``` csharp
var apiClient = Web.ApiClient<TestStartup>()
                   .WithAcceptJsonHeader()
                   .WithFakeClaimsPrincipal(TestPrincipals.FullClaimsPrincipal);
var peter = Actor.Named("Peter").CanUse(apiClient);
```




## Links

https://www.infoq.com/articles/Beyond-Page-Objects-Test-Automation-Serenity-Screenplay














