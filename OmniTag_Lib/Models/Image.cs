using OmniTag.Models.Base;

namespace OmniTag.Models
{
    public class Image : BaseModelEntity
    {
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }

        private byte[] _imageData;
        public byte[] ImageData
        {
            get { return _imageData; }
            set { SetProperty(ref _imageData, value); }
        }

        private Omni _omni;
        public virtual Omni Omni
        {
            get { return _omni; }
            set { SetProperty(ref _omni, value); }
        }
    }
}
