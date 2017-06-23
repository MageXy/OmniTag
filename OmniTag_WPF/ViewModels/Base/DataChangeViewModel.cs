using System;
using OmniTagWPF.Utility;

namespace OmniTagWPF.ViewModels.Base
{
    abstract class DataChangeViewModel : BaseViewModel
    {
        public event EventHandler DataChanged;

        protected void OnDataChanged()
        {
            var handler = DataChanged;
            if (handler != null)
                handler(this, new DataChangedEventArgs());
        }

        public override void RequestCloseView()
        {
            base.RequestCloseView();

            DataChanged = null;
        }
    }
}
