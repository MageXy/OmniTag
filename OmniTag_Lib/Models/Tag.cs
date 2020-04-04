using System.Collections.Generic;
using OmniTag.Models.Base;

namespace OmniTag.Models
{
    public class Tag :BaseModelEntity
    {
        public Tag()
        {
            _omnis = new HashSet<Omni>();
        }
         
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        /**
         * While entering info for a new Omni, a user can manually type in tags that
         * the Omni belongs to. When entering the name of a tag that does not exist yet,
         * the tag will be automatically created but marked as unverified. This means
         * that the user has not confirmed that this is a tag they want to keep (it
         * might be a one-time flag).  A verified tag is one that the user has 
         * explicitly marked as being valid. Unverified tags will not show up in 
         * certain contexts, such as search suggestions. 
         **/
        private bool _isVerified;
        public bool IsVerified
        {
            get { return _isVerified; }
            set { SetProperty(ref _isVerified, value); }
        }

        /**
         * Some tags can be unverified, which means that they may have been entered
         * as a one-time flag or perhaps as a typo. The user can go into the Tag
         * Manager and manually specify that a tag is verified. However, another way
         * for the tag to be verified is if enough Omnis are associated with the tag.
         * In this case, the tag will be "auto-verified". The ManuallyVerified property
         * is only true when the user has explicitly set the tag to be verified. 
         **/
        private bool _manuallyVerified;
        public bool ManuallyVerified
        {
            get { return _manuallyVerified; }
            set { SetProperty(ref _manuallyVerified, value); }
        }
        

        private ICollection<Omni> _omnis;
        public virtual ICollection<Omni> Omnis
        {
            get { return _omnis; }
            set { SetProperty(ref _omnis, value); }
        }
    }
}
