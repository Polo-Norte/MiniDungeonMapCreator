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
            float2 pos = grid.GetGridPos(point);

            Int32Rect rect = new Int32Rect((int)pos.x, (int)pos.y, (int)(grid.GridSize.x + 1), (int)(grid.GridSize.y + 1));
            var stride = ((grid.GridSize.x + 0.5f) * bitmap.Format.BitsPerPixel + 7) / 8;
            byte[] colors = new byte[] { color.B, color.G, color.R, color.A };
            byte[] buffer = Enumerable.Range(0, (int)stride * (int)(grid.GridSize.y + 1)).Select(i => colors[i % 4]).ToArray();

            bitmap.WritePixels(rect, buffer, (int)(stride / 4) * 4, 0);
        }

        public static void PaintArea(this WriteableBitmap bitmap, GridArea grid, int2 point1, int2 point2, Color color)
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