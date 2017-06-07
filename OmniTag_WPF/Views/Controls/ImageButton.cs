using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OmniTagWPF.Views.Controls
{
    /// <summary>
    /// Actual appearance of this button is defined in the Themes/Generic.xaml file. 
    /// </summary>
    public class ImageButton : Button
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty = 
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageButton), new UIPropertyMetadata());
    }
}
