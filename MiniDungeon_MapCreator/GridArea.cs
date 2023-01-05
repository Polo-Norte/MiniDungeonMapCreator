using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiniDungeon_MapCreator
{
    [Flags]
    public enum DoorDirections
    {
        NONE = 0b0000_0000,
        NORTH = 0b0000_0001,
        SOUTH = 0b0000_0010,
        WEST = 0b0000_0100,
        EAST = 0b0000_1000,
        CENTER = 0b0001_0000
    }

    class GridArea : Canvas
    {
        public static int cellCount = 17;
        public static int wallSize = 1;
        public static int doorSize = 2;

        public static Dictionary<char, GridCell> gridCellValues = new Dictionary<char, GridCell>()
        {
            ['#'] = new GridCell(Colors.Black, "Wall"),
            ['@'] = new GridCell(Colors.Cyan, "Hole"),
            [' '] = new GridCell(Colors.Wheat, "Empty"),
            ['Q'] = new GridCell(Colors.Purple, "Op.Door"),
            ['E'] = new GridCell(Color.FromArgb(255, 210, 60, 60), "Enemy T1"),
            ['H'] = new GridCell(Color.FromArgb(255, 160, 40, 40), "Enemy T2"),
            ['K'] = new GridCell(Color.FromArgb(255, 110, 20, 20), "Enemy T3"),
            ['J'] = new GridCell(Color.FromArgb(255, 60, 10, 10), "Boss"),
            ['1'] = new GridCell(Color.FromArgb(255, 160, 220, 160), "Loot T1"),
            ['2'] = new GridCell(Color.FromArgb(255, 140, 180, 140), "Loot T2"),
            ['3'] = new GridCell(Color.FromArgb(255, 120, 140, 120), "Loot T3"),
            ['d'] = new GridCell(Color.FromArgb(255, 119, 106, 204), "Decoration"),
            ['F'] = new GridCell(Color.FromArgb(255, 41, 22, 168), "Fluid"),
            ['f'] = new GridCell(Color.FromArgb(255, 17, 9, 71), "D. Fluid"),
            ['M'] = new GridCell(Color.FromArgb(255, 168, 186, 255), "Trap 1"),
            ['m'] = new GridCell(Color.FromArgb(255, 103, 132, 245), "Trap 2"),
            ['N'] = new GridCell(Color.FromArgb(255, 47, 86, 237), "Trap 3"),
            ['n'] = new GridCell(Color.FromArgb(255, 3, 36, 161), "Trap 4"),
            ['X'] = new GridCell(Colors.DarkRed, "Door"),
        };

        static readonly char[] fixedCells = { 'X', 'O', 'T', 'e' };
        private char[] gridValues;
        public WriteableBitmap bitmap;
        public MainWindow mainWindow;

        private float2 gridSize;
        private int2 gridCount;
        public float2 GridSize
        {
            get { return gridSize; }
            set
            {
                CreateGrid(value);
                gridSize = value;
            }
        }

        private DoorDirections display = 0; // Doors position display (North, South, West, East and Center)

        // Set the initial data
        public void SetupGrid(byte wallDisplay)
        {
            SetGridValueRect(new int2(0, 0), new int2(cellCount, cellCount), '#', EditMode.CONSTRUCTION);
            SetGridValueRect(new int2(wallSize, wallSize), new int2(cellCount - wallSize, cellCount - wallSize), ' ', EditMode.CONSTRUCTION);

            display = (DoorDirections)wallDisplay;

            if (ContainsBit((byte)display, 1 << 0))
                SetGridValueRect(new int2(cellCount / 2 - doorSize, 0), new int2(cellCount / 2 + doorSize, wallSize), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit((byte)display, 1 << 1))
                SetGridValueRect(new int2(cellCount - wallSize, cellCount / 2 - doorSize), new int2(cellCount, cellCount / 2 + doorSize), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit((byte)display, 1 << 2))
                SetGridValueRect(new int2(cellCount / 2 - doorSize, cellCount - wallSize), new int2(cellCount / 2 + doorSize, cellCount), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit((byte)display, 1 << 3))
                SetGridValueRect(new int2(0, cellCount / 2 - doorSize), new int2(wallSize, cellCount / 2 + doorSize), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit((byte)display, 1 << 4))
            {
                Point center = new Point(31 * gridSize.x, 31 * gridSize.y);
                SetGridValueCircle(center, 2.5f, 'O', EditMode.CONSTRUCTION);
                SetGridValue(center, 'e', EditMode.CONSTRUCTION);
            }
            if (ContainsBit((byte)display, 1 << 5))
                SetGridValueCircle(new Point(31 * gridSize.x, 31 * gridSize.y), 2.5f, 'T', EditMode.CONSTRUCTION);
                
        }

        private void CreateGrid(float2 gridSize)
        {
            Children.Clear();
            gridCount = new int2((int)(Width / gridSize.x), (int)(Height / gridSize.y));
            Brush gridBrush = new SolidColorBrush(Color.FromArgb(180, 210, 127, 127));

            for (int i = 1; i < cellCount; i++)
            {
                Line line = new Line
                {
                    X1 = 0,
                    X2 = Width,
                    Y1 = i * gridSize.y
                };
                line.Y2 = line.Y1;

                line.Stroke = gridBrush;
                Children.Add(line);
            }

            for (int i = 1; i < cellCount; i++)
            {
                Line line = new Line
                {
                    X1 = i * gridSize.x,
                    Y1 = 0,
                    Y2 = Height
                };
                line.X2 = line.X1;
                line.Stroke = gridBrush;
                Children.Add(line);

                Brush gridBrushBack = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                Background = gridBrushBack;
            }

            this.gridSize = gridSize;
            //this.gridCount = gridCount;
            gridValues = new char[cellCount * cellCount];
        }

        public float2 GetGridPos(Point pos)
        {
            return new float2((int)(pos.X / gridSize.x) * gridSize.x, (int)(pos.Y / gridSize.y) * gridSize.y);
        }

        public int2 GetGridCoord(Point pos)
        {
            //float2 point = GetGridPos(pos);
            float posi = (float)(pos.X / gridSize.x);
            return new int2((int)(pos.X / gridSize.x), (int)(pos.Y / gridSize.y));
        }

        public void SetGridValue(Point point, char value, EditMode mode)
        {
            if (!gridCellValues.ContainsKey(value))
            {
                SetGridValue(point, ' ', EditMode.CONSTRUCTION);
                return;
            }

            int2 gridCoord = GetGridCoord(point);

            switch (mode)
            {
                // Reserved for more behaviours
                case EditMode.EDIT:
                    if (gridCoord.x < wallSize || gridCoord.y < wallSize || gridCoord.x >= cellCount - wallSize || gridCoord.y >= cellCount - wallSize || fixedCells.Contains(gridValues[gridCoord.x + gridCoord.y * cellCount]))
                        return;
                    break;
            }
            
            bitmap.DrawCell(this, point, gridCellValues[value].color);
            gridValues[gridCoord.x + gridCoord.y * cellCount] = value;
        }

        public void SetGridValueRect(int2 point1, int2 point2, char value, EditMode mode)
        {
            int2 drawDirection = new int2(Math.Sign(point2.x - point1.x), Math.Sign(point2.y - point1.y));
            for (int i = point1.x; i != point2.x; i += drawDirection.x)
                for (int j = point1.y; j != point2.y; j += drawDirection.y)
                    SetGridValue(new Point(i * GridSize.x, j * GridSize.y), value, mode);
        }

        public void SetGridValueCircle(Point point, float radius, char value, EditMode mode)
        {
            point.X = (int)(point.X / gridSize.x);
            point.Y = (int)(point.Y / gridSize.y);

            for (int i = (int)(point.X - radius) + 1; i < point.X + radius; i++)
            {
                double distance = i - point.X;
                double height = Math.Sqrt(radius * radius - distance * distance);

                for (int j = (int)(point.Y - height) + 1; j < point.Y + height; j++)
                    SetGridValue(new Point(i * gridSize.x, j * gridSize.y), value, mode);
            }
        }

        bool ContainsBit(byte value, byte bit)
        {
            return (value & bit) != 0;
        }

        // Builds a string with all data
        public void GetGridValues(System.IO.StreamWriter streamWriter)
        {
            //string result = "";
            //result += Encoding.ASCII.GetString(new byte[] { display }) + "\n";

            char[][] gridSaveValues = new char[cellCount][];

            Console.WriteLine("Display Set: " + display);

            for (int i = 0; i < cellCount; i++)
            {
                char[] gridLine = new char[cellCount];
                for (int j = 0; j < cellCount; j++)
                {
                    //result += gridValues[j + i * cellCount];
                    gridLine[j] = gridValues[j + i * cellCount];
                }

                gridSaveValues[i] = gridLine;
            }

            Newtonsoft.Json.JsonSerializer serializer = Newtonsoft.Json.JsonSerializer.Create();
            serializer.Serialize(streamWriter, new GridData(display, gridSaveValues));
        }

        // Loads a map by a string
        public void LoadGrid(char[][] gridValue)
        {
            cellCount = gridValue.Length;
            MainWindow.SaveConfig();
            mainWindow.Setup();
            for (int i = 0; i < cellCount; i++)
                for (int j = 0; j < cellCount; j++)
                {
                    SetGridValue(new Point(j * gridSize.x , i * gridSize.y), gridValue[i][j], EditMode.CONSTRUCTION);
                }
        }

        public enum EditMode
        {
            CONSTRUCTION,
            EDIT
        }
    }

    public struct GridCell
    {
        public Color color;
        public string name;

        public GridCell(Color color, string name)
        {
            this.color = color;
            this.name = name;
        }
    }

    [System.Serializable]
    public class GridData
    {
        public DoorDirections doorDirections;
        public char[][] gridData;

        public GridData(DoorDirections doorDirections, char[][] gridData)
        {
            this.doorDirections = doorDirections;
            this.gridData = gridData;
        }
    }
}