using System.Collections.Generic;

namespace OmniTagWPF.Views
{
    static class HelpContents
    {
        public static readonly HelpTopic Introduction = new HelpTopic(_introduction, _introductionText);
        public static readonly HelpTopic Omnis = new HelpTopic(_omnis, _omnisText);
        public static readonly HelpTopic OmniMarkdown = new HelpTopic(_omniDE, _omniDEText);
        public static readonly HelpTopic Tags = new HelpTopic(_tags, _tagsText);

        private const string _introduction = "Introduction";
        private const string _omnis = "Omnis";
        private const string _omniDE = "   Description Markdown";
        private const string _tags = "Tags";
        private const string _tagManager = "Tags";

        private const string _introductionText =
            "Welcome to OmniTag, a flexible knowledge repository that specializes in quick reference and lookups. " +
            "OmniTag can be used to keep track of any sort of information - shopping lists, to-do lists, step-by-step guides, " +
            "meeting minutes, notes... Anything you want to remember, OmniTag will track for you! Through the use of a " +
            "intuitive tagging system, users can mark specific tidbits of knowledge with tags that make future reference " +
            "quick and easy.";

        private const string _omnisText =
            "An **Omni** is that core part of the OmniTag applicatin. A single Omni represents a piece of information. " +
            "That information might be something like recipe for chocolate cake, or a guide on how to run a program on " +
            "your computer, or a list of tings you want to buy your friend for their birthday. Literally *any* scrap of " +
            "knowledge can be an Omni.\n\nAn Omni is primarily made up of two main parts: the **summary**, and the **details**. " +
            "The summary, as you might guess, gives a short explanation of the main idea of what the Omni is. The description " +
            "is the details about the Omni. So, using the chocolate cake recipe above, the summary might be something like " +
            "\"Recipe for Chocolate Cake\" and the description would provide step-by-step instructions on how to mix " +
            "ingredients together, how long to bake the cake, how to apply frosting, etc.";
        
        private const string _omniDEText = 
            "On the Omni editor screen, you can edit the Omni description to give details about the Omni. One of the " +
            "features of OmniTag is that ability to include special formatting for your description. This can be " +
            "achieved using special markdown codes, similar to several online forums.\n\nUsing the OmniTag markdown " +
            "can *add a little **extra flair*** to your notes, plus it can help readability as well. Only a limited " +
            "number of markdown codes are implemented, so please see below for a list of code syntax that you can use. " +
            "\n\n" +
           @"|Example Output     |Code Syntax       |
|-------------------|------------------|
|*Italic Text*      |`*Italic Text*`   |";

        private const string _tagsText = "A Tag is a way to simply and effectively organize your Omnis. Think of a Tag like a folder - you can " +
                                       "label the folder, and anything you put inside should be related. The benefit of using tags, however," +
                                       "is that a single Omni can belong to multiple Tags. For example, a list of your favorite books might " +
                                       "belongs to the [books] tag... but it could also belong to the [favorite], [lists], and [reading] tags. " +
                                       "\n\nTags have three key details. The tag name can be anything (up to 20 characters) and can include " +
                                       "numbers, spaces, and symbols. Tag names are unique and can't be duplicated. The Tag details are just a " +
                                       "short description to explain what sort of Omnis should belong to the given tag. Finally, a tag's " +
                                       "verification status determines if the tag has been marked as a valid tag.\n\nIn the Omni creation " +
                                       "screen, you can enter any tag names you like. If you enter the name of a tag that doesn't already " +
                                       "exist, it will be created automatically. These \"auto-Tags\" will be unverified, meaning that if " +
                                       "there are no Omnis associated with the given tag, it will be deleted. Verifying the tag will guarantee " +
                                       "that the Tag will remain even if no Omnis are associated with it. Verified Tags can only be deleted " +
                                       "manually.";

        private const string _tagManagerText = "Tags";

        public static IEnumerable<HelpTopic> GetAllHelpTopics()
        {
            return new List<HelpTopic>
            {
                Introduction,
                Omnis,
                OmniMarkdown,
                Tags
            };
        }
    }

    class HelpTopic
    {
        public HelpTopic(string topicName, string topicDetails)
        {
            Name = topicName;
            Details = topicDetails;
        }

        public string Name { get; set; }

        public string Details { get; set; }
    }
}
