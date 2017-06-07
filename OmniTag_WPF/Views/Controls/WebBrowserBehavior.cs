using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NCGLib.Extensions;

namespace OmniTagWPF.Views.Controls
{
    public class WebBrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(WebBrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtml(WebBrowser d)
        {
            return (string)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
            {
                var newVal = e.NewValue as string;
                var html = newVal.IsEmpty() ? "&nbsp;" : newVal;
                webBrowser.NavigateToString(html);
            }
                
        }
    }
}
