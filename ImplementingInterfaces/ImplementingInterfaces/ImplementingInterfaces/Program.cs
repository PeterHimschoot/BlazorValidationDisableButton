using System;

namespace ImplementingInterfaces
{

  interface ICar
  {
    void Accelerate();
  }

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


  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Calling Accelerate method on Car.");
      Car c = new Car();
      ICar ic = c;

      Console.ReadKey();

      Console.Write("Car.Accelerate =>    ");
      c.Accelerate();
      Console.Write("ICar.Accelerate =>   ");
      ic.Accelerate();

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

    }
  }
}
