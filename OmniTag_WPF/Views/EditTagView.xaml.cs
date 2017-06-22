using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using OmniTag.Models;
using OmniTagWPF.ViewModels;
using OmniTagWPF.Views.Controls;

namespace OmniTagWPF.Views
{
    /// <summary>
    /// Interaction logic for EditTagView.xaml
    /// </summary>
    public partial class EditTagView : CenteredView
    {
        public EditTagView()
        {
            InitializeComponent();
        }
    }

    class VerifiedLabelTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orig = value as bool?;
            if ((orig ?? false) == false)
                return Boolean.FalseString;
            return Boolean.TrueString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class TagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var orig = value as IEnumerable<Tag>;

            return orig?.Select(t => t.Name).ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
