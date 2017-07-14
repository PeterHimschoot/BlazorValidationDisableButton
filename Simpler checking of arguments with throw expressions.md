# Simpler checking of arguments with throw expressions

C# 7 has made a simple and important change to C# throw statements. They are now **expressions**.

So what does that mean for you? It means that now you can check your arguments with a simpler statement.

One such recurring pattern is in the constructor of a class, where you get passed some mandatory dependency.
This dependency cannot be null, and then needs to be stored. If the dependency is null, you want to throw an `ArgumentNullException`.

Here is an example in **C# 6**

```C#
if( context != null )
{
  throw new ArgumentNullException(nameof(context));
}
this.context = context;
```

Compare this with the **C# 7** version:


```C#
this.context = context ?? throw new ArgumentNullException(nameof(context));
```

We assign the context field a value with the **null-coalescing operator** which takes the left side if it is null (and doesn't evaluate the right side). If the left side is null, it evaluates the right side, which throws an `ArgumentNullException`.

Way simpler and elegant!
