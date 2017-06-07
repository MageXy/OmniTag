using System.Windows;
using NCGLib.WPF.Utility;
using OmniTagWPF.ViewModels;
using OmniTagWPF.Views;

namespace OmniTagWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            ViewFactory.RegisterMapping<EditTagViewModel, EditTagView>();

            var view = ViewFactory.CreateWindow<MainOmniView>();
            view.Show();
        }
    }
}
