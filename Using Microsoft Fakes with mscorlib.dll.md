# Using Microsoft Fakes with mscorlib.dll

Microsoft Fakes is a tool that allows you to replace any method of any .NET class. This makes it ideal for testing untestable code, for example code that uses `DateTime.Now` in its implementation...

However when using Microsoft Fakes with `mscorlib.dll`, the assembly that contains `DateTime`, you will encounter an error `Error	CS0234`:

```
The type or namespace name 'EventSourceCreatedEventArgs' does not exist in the 
namespace 'System.Diagnostics.Tracing' (are you missing an assembly reference?)
```

This actually should not be a problem, because it is actually a sign of something else: that you're faking too much (no pun intended :). Using Microsoft Fakes will generate a fake type for every type in the target assembly, which is very wastefull of resources. You should actually only fake the type you're testing.

Open the fakes file (for example `mscorlib.fakes`):

```
<Fakes xmlns="http://schemas.microsoft.com/fakes/2011/">  <Assembly Name="mscorlib" Version="4.0.0.0"/>  <StubGeneration>    <Clear />  </StubGeneration>  <ShimGeneration>    <Clear />    <Add FullName="System.DateTime!" />  </ShimGeneration></Fakes>
```

As you can see here, you add each type you want to stub/shim in the `<StubGeneration>` and `<ShimGeneration>` section.

For more information about this, you can find more documentation at <https://msdn.microsoft.com/en-us/library/hh708916.aspx#BKMK_Configuring_code_generation_of_stubs>