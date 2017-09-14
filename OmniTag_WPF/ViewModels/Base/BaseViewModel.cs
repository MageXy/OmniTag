using System.ComponentModel;
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
        
        public override void OnViewClosing(CancelEventArgs e)
        {
            base.OnViewClosing(e);
            Context.Dispose();
        }
    }
}
