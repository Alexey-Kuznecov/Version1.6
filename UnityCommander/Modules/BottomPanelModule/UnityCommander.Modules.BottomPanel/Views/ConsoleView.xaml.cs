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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityCommander.Modules.BottomPanel.ViewModels;

namespace UnityCommander.Modules.BottomPanel.Views
{
    /// <summary>
    /// Логика взаимодействия для ConsoleTabView.xaml
    /// </summary>
    public partial class ConsoleView : UserControl
    {
        public ConsoleView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is ConsoleViewModel vm)
                    vm.SendCommandCommand.Execute();

                e.Handled = true;
            }
        } 
    }
}
