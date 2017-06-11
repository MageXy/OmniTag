using System.Windows.Controls;
using System.Windows.Input;

namespace OmniTagWPF.Views.Controls
{
    class KeylessComboBox : ComboBox
    {
        public KeylessComboBox()
        {
            
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // do not call the base method
        }
    }
}
