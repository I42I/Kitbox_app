using System;

public class Box
{
    public Face Face1 { get; set; }
    public Face Face2 { get; set; }
    public Face Face3 { get; set; }
    public Face Face4 { get; set; }

    public Box(Face face1, Face face2, Face face3, Face face4)
    {
        if (face1.Height != face2.Height || face1.Height != face3.Height || face1.Height != face4.Height)
        {
            throw new ArgumentException("All faces must have the same height.");
        }

        Face1 = face1;
        Face2 = face2;
        Face3 = face3;
        Face4 = face4;
    }

    public void DisplayInfo()
    {
        Console.WriteLine("Face 1:");
        Face1.DisplayInfo();
        Console.WriteLine("Face 2:");
        Face2.DisplayInfo();
        Console.WriteLine("Face 3:");
        Face3.DisplayInfo();
        Console.WriteLine("Face 4:");
        Face4.DisplayInfo();
    }
}