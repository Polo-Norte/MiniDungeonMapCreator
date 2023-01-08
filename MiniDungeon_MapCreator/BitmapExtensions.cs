using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MiniDungeon_MapCreator
{
    static class BitmapExtensions
    {
        static public void DrawCell(this WriteableBitmap bitmap, GridArea grid, Point point, Color color)
        {
            //float2 pos = grid.GetGridPos(point);
            int2 pos = grid.GetGridCoord(point);
            Int32Rect rect = new Int32Rect((int)pos.x * MainWindow.pixelCount, (int)pos.y * MainWindow.pixelCount, (int)(MainWindow.pixelCount), (int)(MainWindow.pixelCount));
            float stride = ((rect.Width + 1) * bitmap.Format.BitsPerPixel + 7) / 8;
            stride = (int)(stride / 4) * 4;
            byte[] colors = new byte[] { color.B, color.G, color.R, color.A };
            byte[] buffer = Enumerable.Range(0, (int)stride * (int)(rect.Height + 1)).Select(i => colors[i % 4]).ToArray();
            //Console.WriteLine("Grid size: (" + grid.GridSize.x + "," + grid.GridSize.y + ")");
            //Console.WriteLine("Offset: (" + offset.x + "," + offset.y + ")");

            bitmap.WritePixels(rect, buffer, (int)(stride / 4) * 4, 0);
        }

        static public void PaintArea(this WriteableBitmap bitmap, GridArea grid, int2 point1, int2 point2, Color color)
        {
            int2 areaSize = new int2(Math.Abs(point2.x - point1.x), Math.Abs(point2.y - point1.y));
            int2 drawDirection = new int2(Math.Sign(point2.x - point1.x), Math.Sign(point2.y - point1.y));
            for (int i = 0; i != areaSize.x; i += drawDirection.x)
                for (int j = 0; j != areaSize.y; j += drawDirection.y)
                {
                    DrawCell(bitmap, grid, new Point(i * grid.GridSize.x, j * grid.GridSize.y), color);
                }
        }
    }
}