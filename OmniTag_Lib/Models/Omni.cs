using System.Collections.Generic;
using OmniTag.Models.Base;

namespace OmniTag.Models
{
    public class Omni : BaseModelEntity
    {
        public Omni()
        {
            _tags = new HashSet<Tag>();
            _images = new HashSet<Image>();
        }

        private string _summary;
        public string Summary
        {
            get { return _summary; }
            set { SetProperty(ref _summary, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        
        private ICollection<Tag> _tags;
        public virtual ICollection<Tag> Tags
        {
            get { return _tags; }
            set { SetProperty(ref _tags, value); }
        } 

        private ICollection<Image> _images;
        public virtual ICollection<Image> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }
    }
}
