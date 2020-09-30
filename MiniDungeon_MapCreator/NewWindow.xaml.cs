using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiniDungeon_MapCreator
{
    public partial class NewWindow : Window
    {
        MainWindow mainWindow;
        public NewWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            Owner = mainWindow;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            byte setup = 0;
            setup |= (byte)((bool)northBox.IsChecked ? 1 << 0 : 0);
            setup |= (byte)((bool)eastBox.IsChecked ? 1 << 1 : 0);
            setup |= (byte)((bool)southBox.IsChecked ? 1 << 2 : 0);
            setup |= (byte)((bool)westBox.IsChecked ? 1 << 3 : 0);
            setup |= (byte)((bool)centerBox.IsChecked ? 1 << 4 : 0);

            mainWindow.gridCanvas.SetupGrid(setup);
            Close();
        }
    }
}
