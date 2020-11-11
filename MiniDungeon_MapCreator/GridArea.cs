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
    class GridArea : Canvas
    {

        public static Dictionary<char, GridCell> gridCellValues = new Dictionary<char, GridCell>()
        {
            ['#'] = new GridCell(Colors.Black, "Wall"), // Parede
            [' '] = new GridCell(Colors.Wheat, "Empty"), // Vazio
            ['Q'] = new GridCell(Colors.Purple, "Op.Door"), //
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
            ['P'] = new GridCell(Color.FromArgb(255, 148, 212, 188), "Mov. Plat."),
            ['p'] = new GridCell(Color.FromArgb(255, 190, 212, 188), "Mov. Plat. H"),
            ['R'] = new GridCell(Color.FromArgb(255, 108, 172, 148), "Roller"),
            ['r'] = new GridCell(Color.FromArgb(255, 180, 180, 55), "Rope"),
            ['e'] = new GridCell(Color.FromArgb(255, 122, 122, 82), "Elevator"),
            ['T'] = new GridCell(Color.FromArgb(255, 220, 220, 240), "Roof"),
            ['X'] = new GridCell(Colors.DarkRed, "Door"), // Porta
            ['O'] = new GridCell(Color.FromArgb(255, 82, 82, 82), "Hole") // Abertura (Chão);
        };

        static readonly char[] fixedCells = { 'X', 'O', 'T', 'e' };
        private char[] gridValues;
        public WriteableBitmap bitmap;

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

        private byte display = 0; // Doors position display (North, South, West, East and Center)

        // Set the initial data
        public void SetupGrid(byte wallDisplay)
        {
            SetGridValueRect(new int2(0, 0), gridCount, '#', EditMode.CONSTRUCTION);
            SetGridValueRect(new int2(2, 2), new int2(gridCount.x - 2, gridCount.y - 2), ' ', EditMode.CONSTRUCTION);

            display = wallDisplay;

            if (ContainsBit(display, 1 << 0))
                SetGridValueRect(new int2(30, 0), new int2(34, 2), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit(display, 1 << 1))
                SetGridValueRect(new int2(62, 30), new int2(64, 34), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit(display, 1 << 2))
                SetGridValueRect(new int2(30, 62), new int2(34, 64), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit(display, 1 << 3))
                SetGridValueRect(new int2(0, 30), new int2(2, 34), 'X', EditMode.CONSTRUCTION);
            if (ContainsBit(display, 1 << 4))
            {
                Point center = new Point(31 * gridSize.x, 31 * gridSize.y);
                SetGridValueCircle(center, 2.5f, 'O', EditMode.CONSTRUCTION);
                SetGridValue(center, 'e', EditMode.CONSTRUCTION);
            }
            if (ContainsBit(display, 1 << 5))
                SetGridValueCircle(new Point(31 * gridSize.x, 31 * gridSize.y), 2.5f, 'T', EditMode.CONSTRUCTION);
                
        }

        private void CreateGrid(float2 gridSize)
        {
            Children.Clear();

            int2 gridCount = new int2((int)(Width / gridSize.x), (int)(Height / gridSize.y));
            Brush gridBrush = new SolidColorBrush(Color.FromArgb(180, 210, 127, 127));

            for (int i = 1; i < gridCount.x; i++)
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

            for (int i = 1; i < gridCount.y; i++)
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
            this.gridCount = gridCount;
            gridValues = new char[gridCount.x * gridCount.y];
        }

        public float2 GetGridPos(Point pos)
        {
            return new float2((int)(pos.X / gridSize.x) * gridSize.x, (int)(pos.Y / gridSize.y) * gridSize.y);
        }

        public int2 GetGridCoord(Point pos)
        {
            float2 point = GetGridPos(pos);
            return new int2((int)(point.x / gridSize.x), (int)(point.y / gridSize.y));
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
                    if (gridCoord.x < 2 || gridCoord.y < 2 || gridCoord.x >= gridCount.x - 2 || gridCoord.y >= gridCount.y - 2 || fixedCells.Contains(gridValues[gridCoord.x + gridCoord.y * gridCount.x]))
                        return;
                    break;
            }
            
            bitmap.DrawCell(this, point, gridCellValues[value].color);
            gridValues[gridCoord.x + gridCoord.y * gridCount.x] = value;
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
        public string GetGridValues()
        {
            string result = "";
            result += Encoding.ASCII.GetString(new byte[] { display }) + "\n";

            Console.WriteLine("Display Set: " + display);

            for (int i = 0; i < gridCount.y; i++)
            {
                for (int j = 0; j < gridCount.x; j++)
                {
                    result += gridValues[j + i * gridCount.x];
                }
                result += "\n";
            }
            
            return result;
        }

        // Loads a map by a string
        public void LoadGrid(string gridValue)
        {
            string[] valueLines = gridValue.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            display = Encoding.ASCII.GetBytes(valueLines[0].ToCharArray(), 0, 1)[0];
           
            for (int i = 0; i < gridCount.y; i++)
                for (int j = 0; j < gridCount.x; j++)
                {
                    SetGridValue(new Point(j * gridSize.x , i * gridSize.y), valueLines[i + 1].ElementAt(j), EditMode.CONSTRUCTION);
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
}