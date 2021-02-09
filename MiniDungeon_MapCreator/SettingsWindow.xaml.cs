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
    /// <summary>
    /// Lógica interna para SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, ISubWindow
    {
        MainWindow mainWindow;

        public SettingsWindow()
        {
            InitializeComponent();
            sizeBox.Text = GridArea.cellCount.ToString();
            wallBox.Text = GridArea.wallSize.ToString();
            doorBox.Text = GridArea.doorSize.ToString();
        }

        public void Create(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            Owner = mainWindow;
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            GridArea.cellCount = Int32.Parse(sizeBox.Text);
            GridArea.wallSize = Int32.Parse(wallBox.Text);
            GridArea.doorSize = Int32.Parse(doorBox.Text);
            MainWindow.SaveConfig();
            mainWindow.Setup();
            Close();
        }
    }
}
