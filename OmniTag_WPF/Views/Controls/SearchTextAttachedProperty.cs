using System;
using System.Windows;
using System.Windows.Controls;

namespace OmniTagWPF.Views.Controls
{
    class SearchTextAttachedProperty
    {
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.RegisterAttached(
            "SearchText",
            typeof(string),
            typeof(SearchTextAttachedProperty),
            new FrameworkPropertyMetadata(String.Empty));

        public static void SetSearchText(DependencyObject obj, string value)
        {
            obj.SetValue(SearchTextProperty, value);
        }

        public static string GetSearchText(DependencyObject obj)
        {
            return (string)obj.GetValue(SearchTextProperty);
        }
    }
}
