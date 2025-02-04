using System;

public class Face
{
    public string Color { get; set; }
    public double Height { get; set; }

    public Face(string color, double height)
    {
        Color = color;
        Height = height;
    }
    
    public void SetHeight(double height)
    {
        if (height <= 0)
        {
            throw new ArgumentException("La hauteur doit être positive.");
        }
        Height = height;
    }



    public void SetColor(string color)
    {
        Color = color;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Color: {Color}, Height: {Height}");
    }
}