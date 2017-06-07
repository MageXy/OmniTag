using System.Windows.Controls;
using System.Windows.Input;

namespace OmniTagWPF.Views.Controls
{
    class AutoCompleteBox : ComboBox
    {
        public AutoCompleteBox()
        {
            
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // do not call the base method
        }
    }
}
