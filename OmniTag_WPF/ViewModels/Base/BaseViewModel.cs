using NCGLib.WPF.Templates.ViewModels;
using OmniTagDB;

namespace OmniTagWPF.ViewModels.Base
{
    public abstract class BaseViewModel : NCGLibViewModel
    {
        protected BaseViewModel()
        {
            Context = OmniTagDatabaseContextFactory.GetNewContext();
        }

        protected OmniTagContext Context { get; set; }

        public override void RequestCloseView()
        {
            base.RequestCloseView();

            Context.Dispose();
        }
    }
}
