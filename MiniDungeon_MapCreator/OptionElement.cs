using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiniDungeon_MapCreator
{
    public class OptionElement : Canvas
    {
        public char value;
        public Rectangle colorRect;
        public TextBlock elementText;

        static readonly Color[] mouseEventColors =
        {
            Color.FromArgb(120, 210, 220, 210),
            Color.FromArgb(0, 0, 0, 0)
        };

        public OptionElement(char value)
        {
            Width = 100;
            Height = 25;
            
            this.value = value;
            SetOption(value);

            MouseEnter += OptionElement_MouseEnter;
            MouseLeave += OptionElement_MouseLeave;
            MouseUp += OptionElement_MouseUp;
        }

        private void OptionElement_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow).SetValue(value);
        }

        private void OptionElement_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Background = new SolidColorBrush(mouseEventColors[1]);
        }

        private void OptionElement_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Background = new SolidColorBrush(mouseEventColors[0]);
        }

        public void SetOption(char value)
        {
            GridCell cell = GridArea.gridCellValues[value];

            colorRect = new Rectangle()
            {
                Width = 15,
                Height = 15,
                Fill = new SolidColorBrush(cell.color)
            };

            SetLeft(colorRect, 10);
            SetTop(colorRect, 5);

            elementText = new TextBlock()
            {
                Text = cell.name,
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 210, 225, 210))
            };
            SetLeft(elementText, 30);
            SetTop(elementText, 5);

            Children.Add(colorRect);
            Children.Add(elementText);
        }
    }
}
