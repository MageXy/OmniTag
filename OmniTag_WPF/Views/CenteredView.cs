using System.Windows;
using NCGLib.WPF.Templates.Views;

namespace OmniTagWPF.Views
{
    public class CenteredView : NCGLibView
    {
        public CenteredView() : base()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }
}
