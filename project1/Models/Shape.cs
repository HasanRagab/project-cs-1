namespace project1.Models;
using System;

public abstract class Shape
{
    public abstract double Area();
    public virtual double Perimeter() => 0;
    public virtual double Volume() => 0;
    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Area: {Area()}");
        Console.WriteLine($"Perimeter: {Perimeter()}");
        Console.WriteLine($"Volume: {Volume()}");
    }

    public override string ToString()
    {
        return $"{GetType().Name}: Area: {Area()}, Perimeter: {Perimeter()}, Volume: {Volume()}";
    }
}

// 1. Square
public class Square : Shape
{
    public double SideLength { get; set; }

    public Square(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double Area() => SideLength * SideLength;
    public override double Perimeter() => 4 * SideLength;
}

// 2. Triangle
public class Triangle : Shape
{
    public double Base { get; set; }
    public double Height { get; set; }

    public Triangle(double baseLength, double height)
    {
        Base = baseLength;
        Height = height;
    }

    public override double Area() => 0.5 * Base * Height;
}

// 3. Circle
public class Circle : Shape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public override double Area() => Math.PI * Radius * Radius;
    public override double Perimeter() => 2 * Math.PI * Radius;
}

// 4. Rhombus
public class Rhombus : Shape
{
    public double Diagonal1 { get; set; }
    public double Diagonal2 { get; set; }

    public Rhombus(double diagonal1, double diagonal2)
    {
        Diagonal1 = diagonal1;
        Diagonal2 = diagonal2;
    }

    public override double Area() => 0.5 * Diagonal1 * Diagonal2;
    public override double Perimeter() => 4 * Math.Sqrt(Math.Pow(Diagonal1 / 2, 2) + Math.Pow(Diagonal2 / 2, 2));
}

// 5. Regular Hexagon
public class RegularHexagon : Shape
{
    public double SideLength { get; set; }

    public RegularHexagon(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double Area() => (3 * Math.Sqrt(3) / 2) * SideLength * SideLength;
    public override double Perimeter() => 6 * SideLength;
}

// 6. Regular Heptagon
public class RegularHeptagon : Shape
{
    public double SideLength { get; set; }

    public RegularHeptagon(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double Area() => (7 / 4.0) * Math.Pow(SideLength, 2) / Math.Tan(Math.PI / 7);
    public override double Perimeter() => 7 * SideLength;
}

// 7. Regular Octagon
public class RegularOctagon : Shape
{
    public double SideLength { get; set; }

    public RegularOctagon(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double Area() => 2 * (1 + Math.Sqrt(2)) * SideLength * SideLength;
    public override double Perimeter() => 8 * SideLength;
}

// 8. Regular Pentagon
public class RegularPentagon : Shape
{
    public double SideLength { get; set; }

    public RegularPentagon(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double Area() => (Math.Sqrt(5 * (5 + 2 * Math.Sqrt(5))) / 4) * SideLength * SideLength;
    public override double Perimeter() => 5 * SideLength;
}

// 9. Cube
public class Cube : Shape
{
    public double SideLength { get; set; }

    public Cube(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double Area() => 6 * SideLength * SideLength;
    public override double Volume() => Math.Pow(SideLength, 3);
}

// 10. Cylinder
public class Cylinder : Shape
{
    public double Radius { get; set; }
    public double Height { get; set; }

    public Cylinder(double radius, double height)
    {
        Radius = radius;
        Height = height;
    }

    public override double Area() => 2 * Math.PI * Radius * (Radius + Height);
    public override double Volume() => Math.PI * Radius * Radius * Height;
}

// 11. Cone
public class Cone : Shape
{
    public double Radius { get; set; }
    public double Height { get; set; }

    public Cone(double radius, double height)
    {
        Radius = radius;
        Height = height;
    }

    public override double Area() => Math.PI * Radius * (Radius + Math.Sqrt(Radius * Radius + Height * Height));
    public override double Volume() => (1.0 / 3) * Math.PI * Radius * Radius * Height;
}

// 12. Pyramid
public class Pyramid : Shape
{
    public double BaseArea { get; set; }
    public double Height { get; set; }

    public Pyramid(double baseArea, double height)
    {
        BaseArea = baseArea;
        Height = height;
    }

    public override double Area() => BaseArea + 2 * Math.Sqrt(BaseArea * Height * Height + BaseArea);
    public override double Volume() => (1.0 / 3) * BaseArea * Height;
}

