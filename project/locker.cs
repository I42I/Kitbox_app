using System;
using System.Collections.Generic;

public class Locker
{
    public List<Box> Boxes { get; set; } // here i make a list of boxes to make the lockers

    public Locker()
    {
        Boxes = new List<Box>();
    }

    public void AddBox(Box box)
    {
        Boxes.Add(box);
    }

    public double GetTotalHeight()
    {
        double totalHeight = 0;
        foreach (var box in Boxes)
        {
            if (box.Face1 != null)
            {
                totalHeight += box.Face1.Height;
            }
        }
        return totalHeight;
    }

    public void DisplayInfo()
    {
        for (int i = 0; i < Boxes.Count; i++)
        {
            Console.WriteLine($"Box {i + 1}:");
            Boxes[i].DisplayInfo();
        }
        Console.WriteLine($"Total Height: {GetTotalHeight()}");
    }
}