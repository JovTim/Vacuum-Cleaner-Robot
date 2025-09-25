using System;

namespace VacuumCleanerRobot
{
  public class Program
  {

    public class Map
    {
      private enum CellType { Empty, Dirt, Obstacle, Cleaned }
      private CellType[,] _grid;
      public int Width { get; private set; }
      public int Height { get; private set; }

      public Map(int width, int height)
      {
        this.Width = width;
        this.Height = height;
        _grid = new CellType[width, height];

        for (int x = 0; x < width; x++)
        {
          for (int y = 0; y < height; y++)
          {
            _grid[x, y] = CellType.Empty;
          }
        }
      }
    }

    public static void Main(string[] args)
    {
      Map myMap = new Map(12, 15);

      Console.WriteLine($"Widtd: {myMap.Width}");
      Console.WriteLine($"Height: {myMap.Height}");
    }
  }
}