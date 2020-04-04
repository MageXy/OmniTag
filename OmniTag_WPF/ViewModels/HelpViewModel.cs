using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NCGLib;
using NCGLib.WPF.Templates.ViewModels;
using OmniTagWPF.Utility;
using OmniTagWPF.Views;

namespace OmniTagWPF.ViewModels
{
    class HelpViewModel : SimpleBaseViewModel
    {
        public HelpViewModel()
        {
            HelpTopics = HelpContents.GetAllHelpTopics();
            SelectedHelpTopic = HelpTopics.Single(h => h == HelpContents.Introduction);
        }

        #region Properties

        private IEnumerable<HelpTopic> _helpTopics;
        public IEnumerable<HelpTopic> HelpTopics
        {
            get { return _helpTopics; }
            set { SetProperty(ref _helpTopics, value); }
        }

        private HelpTopic _selectedHelpTopic;
        public HelpTopic SelectedHelpTopic
        {
            get { return _selectedHelpTopic; }
            set { SetProperty(ref _selectedHelpTopic, value); }
        }

        [DependsOnProperty(nameof(SelectedHelpTopic))]
        public string RenderedMarkdownHtml
        {
            get
            {
                return SelectedHelpTopic == null
                    ? "&nbsp;"
                    : OmniTextRenderer.Render(
                            SelectedHelpTopic.Details.Replace("\r\n", "\n").Replace("\n", Environment.NewLine) ?? String.Empty,
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmniTag/TempImages/")
                        );
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Commands



        #endregion
    }
}
