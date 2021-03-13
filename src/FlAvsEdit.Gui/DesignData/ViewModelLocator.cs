using FlAvsEdit.Gui.ViewModels;

namespace FlAvsEdit.Gui.DesignData
{
    internal static class ViewModelLocator
    {
        private static MainViewModel main;

        public static MainViewModel Main
        {
            get
            {
                if (main == null)
                {
                    main = new MainViewModel();
                    main.Scripts.Add(new ScriptViewModel() { Title = "Script 1" });
                    main.Scripts.Add(new ScriptViewModel() { Title = "Script 2" });
                    main.Scripts.Add(new ScriptViewModel() { Title = "Script 3" });
                }

                return main;
            }
        }
    }
}
