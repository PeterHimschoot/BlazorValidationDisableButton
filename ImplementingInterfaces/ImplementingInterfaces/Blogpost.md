# Be Carefull with the Implicit Implementation for Interfaces

In this blog post I will discuss some pitfall when implementing interfaces and how to avoid it.

## Examining the problem

Create a new **Console** project (either .NET Framework or .NET Core, same difference).

Let's start with the `ICar` interface. This interface has a single `Accelerate` method.

``` csharp
interface ICar
{
  void Accelerate();
}
```

And we have a `Car` class implementing the `ICar` interface.

``` csharp
class Car : ICar
{
  public void Accelerate() 
    => Console.WriteLine("Vroem!!");
}
```

We create an instance of the `Car` class and call the `Accelerate` method from both `Car` and `ICar` reference.

``` csharp
Console.WriteLine("Calling Accelerate method on Car.");
Car c = new Car();
ICar ic = c;
Console.Write("Car.Accelerate =>    ");
c.Accelerate();
Console.Write("ICar.Accelerate =>   ");
ic.Accelerate();
```

Running the application generates the expected output.

``` console
Calling Accelerate method on Car.
Car.Accelerate =>    Vroem!!
ICar.Accelerate =>   Vroem!!
```

Let's inherit a `Tesla` class from the `Car` class.

``` csharp
class Tesla : Car
{
}
```

We will extend our program calling the Tesla's `Accelerate` method.

```
Console.WriteLine("Calling Accelerate method on Tesla.");
Tesla t = new Tesla();
Console.Write("Tesla.Accelerate =>  ");
t.Accelerate();
Console.Write("Car.Accelerate =>    ");
c = t;
c.Accelerate();
Console.Write("ICar.Accelerate =>   ");
ic = t;
ic.Accelerate();
```

We haven't overwritten the `Accelerate` method (yet), so calling the methods on the Tesla instance gives us the same output, since `Tesla` inherits the `Accelerate` method.

``` console
Calling Accelerate method on Tesla.
Tesla.Accelerate =>  Vroem!!
Car.Accelerate =>    Vroem!!
ICar.Accelerate =>   Vroem!!
```

So far, so good... But Teslas don't make noise, so let's overwrite the `Tesla.Accelerate` method:

``` csharp
class Tesla : Car
{
  public void Accelerate() 
    => Console.WriteLine("Zzzzzzzzz");
}
```

Running the program again now shows a different and weird result! Since you're calling the same method on the same object you would expect all invocations to return `Zzzzzzzzz`!

``` console
Calling Accelerate method on Tesla.
Tesla.Accelerate =>  Zzzzzzzzz
Car.Accelerate =>    Vroem!!
ICar.Accelerate =>   Vroem!!
```

So why do we get different output? Look again at the `Tesla` class. You'll see a twiggly line beneath the `Accelerate` method, telling you that `Tesla.Accelerate()` hides inherited member `Car.Accelerate()`.
Actually, our implementation defaults to the `new` keyword, so the code looks like this:

``` csharp
class Tesla : Car
{
  public new void Accelerate()
    => Console.WriteLine("Zzzzzzzzz");
}
```

Now things make a little more sense. If you call the `Tesla` instance using a `Car` reference you'll call the `Car.Accelerate` method, and if you call the instance using a `Tesla` reference you'll call the `Tesla.Accelerate` method! Our `Tesla` instance has two `Accelerate` methods, and you can choose which one to call using the reference type, the class or the interface. 

> If you want to learn a little more about the difference between the `override` and `new` keyword in polymorphish, you should read this article from [Microsoft C# docs](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/knowing-when-to-use-override-and-new-keywords).

### Now we're at the actual problem.

Imagine you have a method taking a `Car` or `ICar` argument, and you call it with a `Tesla` instance. You'll end up with different behavior! 
Imagine debugging this. When you step into the `Accelerate` method using a `Tesla` reference, you'll get a different implementation then when you step into the `Accelerate` method in your method! This will baffle you for a while, and can result in subtle bugs!

## Fixing the problem

The cause of all this confusion comes from the `new` keyword. Let's replace it with the `override` keyword! 

``` csharp
class Tesla : Car
{
  public override void Accelerate()
    => Console.WriteLine("Zzzzzzzzz");
}
```

In order for the override to compile, we need to make this method virtual:

``` csharp
class Car : ICar
{
  public virtual void Accelerate() 
    => Console.WriteLine("Vroem!!");
}
```

Now the program gives the expected output!

``` console
Calling Accelerate method on Car.
Car.Accelerate =>    Vroem!!
ICar.Accelerate =>   Vroem!!
Calling Accelerate method on Tesla.
Tesla.Accelerate =>  Zzzzzzzzz
Car.Accelerate =>    Zzzzzzzzz
ICar.Accelerate =>   Zzzzzzzzz
```

But hey! We have changed this method to be virtual! And `virtual` calls are more expensive then normal calls!

### The C# runtime uses virtual calls for interface methods
As it turns out, any method you write to implement an interface is called as a virtual method behind the scenes anyway.
Undo your changes so the `Car.Accelerate` is no longer virtual and your code compiles.
Open **ILSpy**, and open your console application with it. 
Now decompile `Program.Main()` using **IL with C#**. Scroll down to the first call of the `Accelerate` method.

```
// car.Accelerate();
	IL_0025: ldloc.0
	IL_0026: callvirt instance void ImplementingInterfaces.Car::Accelerate()
```

As you can see, the runtime uses a virtual call (`callvirt`) to the `Accelerate` method, even when it has not been declared to be virtual, since it's an interface method.

### Overriding non-virtual interface methods
You could also argue that you don't want derived classes to override the interface's method behavior. But you can do this anyway. How? Look at this example, where I simply re-implement the `ICar` interface:

``` csharp
class Car : ICar
{
  public void Accelerate() 
    => Console.WriteLine("Vroem!!");
}

class Tesla : Car, ICar
{
  public new void Accelerate()
    => Console.WriteLine("Zzzzzzzzz");
}

```

Running the program results in this output. Calling `Accelerate` using the `ICar` interface now invokes the `Tesla.Accelerate` method, effectively overriding the method in the `ICar` interface.

``` console
Calling Accelerate method on Car.
 Car.Accelerate =>    Vroem!!
ICar.Accelerate =>   Vroem!!
Calling Accelerate method on Tesla.
Tesla.Accelerate =>  Zzzzzzzzz
Car.Accelerate =>    Vroem!!
ICar.Accelerate =>   Zzzzzzzzz
```

So we were able to re-implement the interface method in the derived class. Again, making the base class interface method non-virtual does not protect agains overriding the interface method in a derived class!


## Conclusion

Implement your class's interface non-private methods to be `virtual`, this will allow derived classes to override the method in a consistent way. Otherwise you might end up with confusing behavior depending on the reference used to invoke the method.
In many cases, this will also allow you to easily fake these classes during unit testing, since you can inherit from the actual class, and fake any interface method simply by overriding it.







