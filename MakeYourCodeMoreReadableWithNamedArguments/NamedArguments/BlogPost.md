# Make your code easier to understand with named arguments

![Named Arguments](https://u2ublogimages.blob.core.windows.net/peter/MakeYourCodeMoreReadableWithNamedArguments.PNG)
Code gets written once, but gets read many times. Therefore it is essential that you make your code as easy to read as possible. 
Using named arguments is a very easy way to make certain methods easier to understand.

Let's start with an example.

```
Type type = Type.GetType("System.Random", false);
```

What does the `false` argument do? Maybe you have written lots of code using .NET reflection, but otherwise I am pretty sure you have no idea from sight what that `false` argument means!

Now let's add a **named argument** to the example. You can pass an argument to a method by specifying the name of the argument followed by a colon and then the value.

```
var type = Type.GetType("System.Random", throwOnError: false);
```

Do you now get a better idea what that is about?

Here is another example using a product class:

```
var product = new Product("Nutella", 5, 4);
```

You might know that the first argument is the product name, but what are those numbers? Even if you know that the numbers mean price and units in stock, did you get the order right?
Compare that to this.

```
var product = new Product("Nutella", price: 5, unitsInStock: 4);
```

Let's have a look at the product class' constructor.

```
public Product(string name, decimal price, decimal unitsInStock)
{
  this.Name = name ?? throw new ArgumentNullException(nameof(name));
  this.Price = (price > 0) ? price : throw new ArgumentException(paramName: nameof(price), message: "Price should be a positive value");
  this.UnitsInStock = unitsInStock;
}
```

Check out the second statement in the constructor. 
Here we throw an exception if the price is zero or negative. 
The thing is that normally this constructor takes the message as the first argument and the parameter name as the second. 
But I like it better when the parameter name comes first. With named arguments I can pass in the arguments in any order! 
However in this case you need to name each argument. From C# 7.2 that's no longer a requirement, in that case unnamed arguments simply need to be in the right position.
For example this works in C# 7.2

```
product = new Product("Nutella", price: 0, 0);
```

So, do you need to use named arguments all over the place now? Of course not. Simply use named arguments where the meaning of the argument is not immediatly clear from the context.

Happy coding!

