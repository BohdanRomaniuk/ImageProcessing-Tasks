using Equalization_and_Filters.ViewModels;
using System.Windows;

namespace Equalization_and_Filters
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
