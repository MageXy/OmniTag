using System;
using System.Windows;
using System.Windows.Media.Imaging;
using NCGLib.WPF.Templates.Views;

namespace OmniTagWPF.Views
{
    public class CenteredView : NCGLibView
    {
        public CenteredView() : base()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Icon = new BitmapImage(new Uri("pack://application:,,,/OmniTagWPF;component" + Images.Images.Logo));
        }
    }
}
