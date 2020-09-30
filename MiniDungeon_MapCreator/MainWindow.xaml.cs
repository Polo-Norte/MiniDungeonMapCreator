using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MiniDungeon_MapCreator
{
    /// <summary>
    /// Interação lógica para MainWindow.xamN
    /// </summary>
    public partial class MainWindow : Window
    {

        public const int gridCount = 64;
        private char curValue = ' ';
        private float curSize = 1;
        public OptionElement selectedElement;

        private Regex sizeRegex = new Regex("[^0-9.-]+");

        float2 gridSize;
        bool drawing = false;
        WriteableBitmap bitmap;

        public MainWindow()
        {
            InitializeComponent();
            gridSize = new float2((float)gridCanvas.Width / gridCount, (float)gridCanvas.Height / gridCount);

            gridCanvas.GridSize = gridSize;

            Panel.SetZIndex(gridCanvas, 2);

            bitmap = new WriteableBitmap(600, 600, 92, 92, PixelFormats.Bgr32, null);
            canvas.Source = bitmap;
            
            gridCanvas.bitmap = bitmap;
            gridCanvas.SetupGrid(1 << 0 | 1 << 1 | 1 << 2 | 1 << 3);

            KeyValuePair<char, GridCell>[] gridValues = GridArea.gridCellValues.ToArray();
            for (int i = 0; i < gridValues.Length - 2; i++)
            {
                selectionPanel.Children.Add(new OptionElement(gridValues[i].Key));
            }

            selectionPanel.Height = gridValues.Length * 25;

            selectedElement = new OptionElement(curValue);
            menuPanel.Children.Add(selectedElement);
            Canvas.SetTop(selectedElement, 250);

        }

        public void SetValue(char value)
        {
            curValue = value;

            GridCell cell = GridArea.gridCellValues[value];
            selectedElement.colorRect.Fill = new SolidColorBrush(cell.color);
            selectedElement.elementText.Text = cell.name;
        }


        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            gridCanvas.SetGridValue(e.GetPosition(gridCanvas), curValue, GridArea.EditMode.EDIT);
            drawing = true;
        }
        
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing)
                gridCanvas.SetGridValueCircle(e.GetPosition(gridCanvas), curSize, curValue, GridArea.EditMode.EDIT);

            int2 coords = gridCanvas.GetGridCoord(e.GetPosition(gridCanvas));
            coordLabel.Content = coords.x + " / " + coords.y;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            drawing = false;
        }

        private void sizeBox_TextChange(object sender, TextChangedEventArgs e)
        {
            string text = sizeBox.Text;
            if (text.Length == 0)
                return;

            if (sizeRegex.IsMatch(text))
            {
                sizeBox.Text = text.Remove(text.Length - 1, 1);
                return;
            }

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.CurrencyDecimalSeparator = ".";

            try
            {
                curSize = (float)double.Parse(text, numberFormatInfo);
            }
            catch
            {
                sizeBox.Text = "";
            }
                
            
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog fileDialog = new System.Windows.Forms.SaveFileDialog();
            fileDialog.Filter = "Text Document (*.txt)|*.txt";
          
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine("Saved at: " + fileDialog.FileName);
                StreamWriter file = new StreamWriter(fileDialog.FileName);
                file.Write(gridCanvas.GetGridValues());
                file.Close();
            }
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Text Document (.txt) | *.txt";
            
            bool? result = fileDialog.ShowDialog();

            if ((bool)result)
            {
                Console.WriteLine("Load File Selected at: " + fileDialog.FileName);
                try
                {
                    string text = File.ReadAllText(fileDialog.FileName);
                    gridCanvas.LoadGrid(text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NewClick(object sender, RoutedEventArgs e)
        {
            NewWindow newButtonWindow = new NewWindow(this);

            Point pos = PointToScreen(new Point(0, 0));

            newButtonWindow.Left = pos.X + 150;
            newButtonWindow.Top = pos.Y + 150;

            newButtonWindow.ShowDialog();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

public struct float2
{
    public float x;
    public float y;

    public float2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    static public float Distance(float2 p1, float2 p2)
    {
        float2 vector = new float2(p2.x - p1.x, p2.y - p1.y);
        return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
    }
}

public struct int2
{
    public int x;
    public int y;

    public int2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    
}
