using System;
using System.Collections.Generic;
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
            set { PropNotify.SetProperty(ref _helpTopics, value); }
        }

        private HelpTopic _selectedHelpTopic;
        public HelpTopic SelectedHelpTopic
        {
            get { return _selectedHelpTopic; }
            set { PropNotify.SetProperty(ref _selectedHelpTopic, value); }
        }

        [DependsOnProperty(nameof(SelectedHelpTopic))]
        public string RenderedMarkdownHtml
        {
            get
            {
                return SelectedHelpTopic == null
                    ? "&nbsp;"
                    : OmniTextRenderer.Render(SelectedHelpTopic.Details.Replace("\r\n", "\n")
                        .Replace("\n", Environment.NewLine) ?? String.Empty);
            }
        }

        #endregion

        #region Methods

        #endregion

        #region Commands



        #endregion
    }
}
