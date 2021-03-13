using System.Windows;
using FlAvsEdit.Gui.ViewModels;

namespace FlAvsEdit.Gui.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            var vm = new MainViewModel();
            DataContext = vm;
        }
    }
}
