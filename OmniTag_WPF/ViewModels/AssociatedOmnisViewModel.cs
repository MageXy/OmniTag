using System.Collections.Generic;
using System.Linq;
using OmniTag.Models;
using OmniTagWPF.ViewModels.Base;

namespace OmniTagWPF.ViewModels
{
    class AssociatedOmnisViewModel : BaseViewModel
    {
        public AssociatedOmnisViewModel(Tag tag)
        {
            _selectedTag = tag;
        }

        #region Properties

        private readonly Tag _selectedTag;

        private List<Omni> _availableOmnis;
        public List<Omni> AvailableOmnis
        {
            get { return _availableOmnis; }
            set { PropNotify.SetProperty(ref _availableOmnis, value); }
        }

        public string SelectedTagText
        {
            get { return $"The following Omnis are associated with the [{_selectedTag.Name}] tag:"; }
        }

        #endregion

        #region Methods

        public override void LoadData()
        {
            AvailableOmnis = _selectedTag.Omnis.ToList();
            if (AvailableOmnis.Count == 0)
                AvailableOmnis = new List<Omni>
                {
                    new Omni
                    {
                        Summary="No associated Omnis"
                    }
                };
        }

        #endregion

        #region Commands



        #endregion
    }
}
