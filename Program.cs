using System;

namespace VacuumCleanerRobot
{
  public class Map
  {
    private enum CellType { Empty, Dirt, Obstacle, Cleaned }
    private CellType[,] _grid;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public int xRobot { get; set; }
    public int yRobot { get; set; }

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

    public bool IsInBounds(int x, int y)
    {
      return x >= 0 && x < this.Width && y >= 0 && y < this.Height;
    }

    public bool IsDirt(int x, int y)
    {
      return IsInBounds(x, y) && _grid[x, y] == CellType.Dirt;
    }

    public bool IsObstacles(int x, int y)
    {
      return IsInBounds(x, y) && _grid[x, y] == CellType.Obstacle;
    }

    public void AddObstacle(int x, int y)
    {
      _grid[x, y] = CellType.Obstacle;
    }

    public void AddDirt(int x, int y)
    {
      _grid[x, y] = CellType.Dirt;
    }

    public void Clean(int x, int y)
    {
      if (IsInBounds(x, y))
      {
        _grid[x, y] = CellType.Cleaned;
      }
    }

    public void Display(int robotX, int robotY)
    {
      this.xRobot = robotX;
      this.yRobot = robotY;

      // display the 2d grid, it accepts the location of the robot in x and y
      Console.Clear();
      Console.WriteLine("Vacuum cleaner robot simulation");
      Console.WriteLine("---------------------------------");
      Console.WriteLine("Legends: #=Obstacles, D=Dirt, .=Empty, R=Robot, C=Cleaned");

      // display the grid using loop
      for (int y = 0; y < this.Height; y++)
      {
        for (int x = 0; x < this.Width; x++)
        {
          if (x == robotX && y == robotY)
          {
            Console.Write("R ");

          }
          else
          {
            switch (_grid[x, y])
            {
              case CellType.Empty: Console.Write(". "); break;
              case CellType.Dirt: Console.Write("D "); break;
              case CellType.Obstacle: Console.Write("# "); break;
              case CellType.Cleaned: Console.Write("C "); break;

            }
          }
        }
        Console.WriteLine();
      }
      Thread.Sleep(100);
    }
  }

  public class Robot
  {
    private ICleaningStrategy _cleaningStrategy;
    private readonly Map _map;
    public int X { get; set; }
    public int Y { get; set; }

    public Robot(Map map)
    {
      this._map = map;
      this.X = 0;
      this.Y = 0;
    }

    public bool Move(int newX, int newY)
    {
      if (_map.IsInBounds(newX, newY) && !(_map.IsObstacles(newX, newY)))
      {
        // set the new location
        this.X = newX;
        this.Y = newY;
        // display the map with the robot in its location in the grid
        _map.Display(this.X, this.Y);
        return true;
      }
      // it cannot move
      return false;
    }

    public void CleanCurrentSpot()
    {
      if (_map.IsDirt(this.X, this.Y))
      {
        _map.Clean(this.X, this.Y);
        _map.Display(this.X, this.Y);
      }
    }


    public void SetStrategy(ICleaningStrategy cleaningStrategy)
    {
      _cleaningStrategy = cleaningStrategy;
    }

    public void StartCleaning()
    {
      if (_cleaningStrategy == null)
      {
        Console.WriteLine("NO STRATEGY FOUND!");
        return;
      }

      _cleaningStrategy.Clean(this, _map);
    }

  }

  public interface ICleaningStrategy
  {
    void Clean(Robot robot, Map map);

  }

  public class S_PatternStrategy : ICleaningStrategy
  {
    public void Clean(Robot robot, Map _map)
    {
      Console.Write("Start cleaning the room");
      // flag that determines the direction

      int direction = 1;

      // changed the y direction based on the initialized Y position of the robot
      for (int y = _map.yRobot; y < _map.Height; y++)
      {
        // start the robot based on the initialized position X
        int startX = _map.xRobot;
        int endX = (direction == 1) ? _map.Width : -1;

        bool rowInterrupted = false;

        for (int x = startX; x != endX; x += direction)
        {
          if (!_map.IsInBounds(x, y) || _map.IsObstacles(x, y))
          {
            Console.WriteLine($"Obstacle ahead at ({x},{y})");

            rowInterrupted = true;

            direction *= -1;

            break; // stop this row early
          }

          robot.Move(x, y);
          robot.CleanCurrentSpot();

          _map.xRobot = x;
        }

        // only flip if row was completed normally
        if (!rowInterrupted)
        {
          direction *= -1;
        }
      }
    }
  }

  public class RandomPathStrategy : ICleaningStrategy
  {
    private Random random = new Random();
    private List<int[]> arr = new List<int[]>();

    public void Clean(Robot robot, Map _map)
    {
      while (true)
      {
        // add if top exist
        if (_map.IsInBounds(_map.xRobot - 1, _map.yRobot) && !(_map.IsObstacles(_map.xRobot - 1, _map.yRobot)))
        {
          arr.Add(new int[] { _map.xRobot - 1, _map.yRobot });
        }

        // add if top left exist
        if (_map.IsInBounds(_map.xRobot - 1, _map.yRobot - 1) && !(_map.IsObstacles(_map.xRobot - 1, _map.yRobot - 1)))
        {
          arr.Add(new int[] { _map.xRobot - 1, _map.yRobot - 1 });
        }

        // add if top right exist

        if (_map.IsInBounds(_map.xRobot - 1, _map.yRobot + 1) && !(_map.IsObstacles(_map.xRobot - 1, _map.yRobot + 1)))
        {
          arr.Add(new int[] { _map.xRobot - 1, _map.yRobot + 1 });
        }

        // add if left exist
        if (_map.IsInBounds(_map.xRobot, _map.yRobot - 1) && !(_map.IsObstacles(_map.xRobot, _map.yRobot - 1)))
        {
          arr.Add(new int[] { _map.xRobot, _map.yRobot - 1 });
        }

        // add if right exist
        if (_map.IsInBounds(_map.xRobot, _map.yRobot + 1) && !(_map.IsObstacles(_map.xRobot, _map.yRobot + 1)))
        {
          arr.Add(new int[] { _map.xRobot, _map.yRobot + 1 });
        }

        // add if bottom exist
        if (_map.IsInBounds(_map.xRobot + 1, _map.yRobot) && !(_map.IsObstacles(_map.xRobot + 1, _map.yRobot)))
        {
          arr.Add(new int[] { _map.xRobot + 1, _map.yRobot });
        }

        // add if bottom left exist
        if (_map.IsInBounds(_map.xRobot + 1, _map.yRobot - 1) && !(_map.IsObstacles(_map.xRobot + 1, _map.yRobot - 1)))
        {
          arr.Add(new int[] { _map.xRobot + 1, _map.yRobot - 1 });
        }

        // add if bottom right exist
        if (_map.IsInBounds(_map.xRobot + 1, _map.yRobot + 1) && !(_map.IsObstacles(_map.xRobot + 1, _map.yRobot + 1)))
        {
          arr.Add(new int[] { _map.xRobot + 1, _map.yRobot + 1 });
        }


        int r = random.Next(arr.Count);

        //Console.WriteLine(r);

        int[] temp = arr[r];

        robot.Move(temp[0], temp[1]);
        robot.CleanCurrentSpot();
        arr.Clear();
      }
    }

  }
  public class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine("Initialize Robot");

      Map map = new Map(10, 10);

      map.AddObstacle(4, 3);
      map.AddObstacle(5, 3);
      map.AddObstacle(5, 5);
      map.AddObstacle(5, 4);
      map.AddObstacle(4, 5);
      map.AddObstacle(4, 4);
      map.AddObstacle(4, 6);
      map.AddObstacle(5, 6);

      map.AddDirt(0, 5);
      map.AddDirt(0, 6);
      map.AddDirt(9, 6);
      map.AddDirt(9, 5);

      map.Display(2, 2);



      Robot robot = new Robot(map);

      robot.SetStrategy(new S_PatternStrategy());
      robot.StartCleaning();

      robot.SetStrategy(new RandomPathStrategy());
      robot.StartCleaning();




    }
  }
}