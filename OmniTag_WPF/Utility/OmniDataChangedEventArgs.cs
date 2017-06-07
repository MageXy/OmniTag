using System;
using OmniTag.Models;


namespace OmniTagWPF.Utility
{
    class OmniDataChangedEventArgs : EventArgs
    {
        public OmniDataChangedEventArgs(Omni omniChanged)
        {
            ModifiedOmni = omniChanged;
        }

        public Omni ModifiedOmni { get; private set; }
    }
}
