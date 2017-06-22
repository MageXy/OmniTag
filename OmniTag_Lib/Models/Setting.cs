using OmniTag.Models.Base;

namespace OmniTag.Models
{
    public class Setting : BaseModelEntity
    {
        public const string AutoTagVerificationThreshold = "Auto_Tag_Verification_Threshold";
        public const string ShowTagSearchOnStartup = "Show_Tag_Search_On_Startup";

        private string _name;
        public string Name
        {
            get { return _name; }
            set { PropNotify.SetProperty(ref _name, value); }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { PropNotify.SetProperty(ref _value, value); }
        }
    }
}
