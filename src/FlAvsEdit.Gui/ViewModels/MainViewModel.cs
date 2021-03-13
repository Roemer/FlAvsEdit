using System.Collections.ObjectModel;
using System.Windows.Input;
using FlAvsEdit.Gui.Core;
using Microsoft.Win32;

namespace FlAvsEdit.Gui.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<ScriptViewModel> Scripts { get; set; }

        public ICommand FileOpenCommand { get; }

        public MainViewModel()
        {
            Scripts = new ObservableCollection<ScriptViewModel>();

            FileOpenCommand = new RelayCommand((o) =>
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "AviSynth Files (*.avs;*.avsi)|*.avs;*.avsi|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    System.Diagnostics.Debug.WriteLine(openFileDialog.FileName);
                    Scripts.Add(new ScriptViewModel() { Title = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName) });
                }
            });
        }
    }
}
