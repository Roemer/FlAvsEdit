using FlAvsEdit.Gui.Core;

namespace FlAvsEdit.Gui.ViewModels
{
    public class ScriptViewModel : ObservableObject
    {
        public string Title
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
