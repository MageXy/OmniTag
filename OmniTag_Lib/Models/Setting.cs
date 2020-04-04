using OmniTag.Models.Base;

namespace OmniTag.Models
{
    public class Setting : BaseModelEntity
    {
        public const string AutoTagVerificationThreshold = "Auto_Tag_Verification_Threshold";
        public const string ShowTagSearchOnStartup = "Show_Tag_Search_On_Startup";
        public const string EmbeddedImageTempDirectory = "Embedded_Image_Temp_Directory";

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
    }
}
