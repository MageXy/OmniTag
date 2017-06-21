using NCGLib;
using NCGLib.WPF.Templates.ViewModels;
using OmniTag.Models;
using Image = OmniTagWPF.Images.Images;

namespace OmniTagWPF.ViewModels.Controls
{
    public class TagButtonViewModel : SimpleBaseViewModel
    {
        public TagButtonViewModel(Tag tag)
        {
            CurrentTag = tag;
        }
        
        private Tag _currentTag;
        public Tag CurrentTag
        {
            get { return _currentTag; }
            set { PropNotify.SetProperty(ref _currentTag, value); }
        }

        [DependsOnProperty("CurrentTag")]
        public string ButtonLabel
        {
            get { return CurrentTag.Name; }
        }

        [DependsOnProperty("CurrentTag")]
        public string ButtonImage
        {
            get { return CurrentTag.IsVerified ? Image.CheckMark : Image.ExclamationMark; }
        }

        //public override void LoadData()
        //{
        //    // If this tag already exists in the DB, replace the given tag with the pre-exisiting one.
        //    var tag = Context.Tags.SingleOrDefault(t => t.Name == CurrentTag.Name);
        //    if (tag != null)
        //        CurrentTag = tag;
        //}
    }
}
