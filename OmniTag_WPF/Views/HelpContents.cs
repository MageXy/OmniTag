using System.Collections.Generic;

namespace OmniTagWPF.Views
{
    static class HelpContents
    {
        public static readonly HelpTopic Introduction = new HelpTopic(_introduction, _introductionText);
        public static readonly HelpTopic Omnis = new HelpTopic(_omnis, _omnisText);
        public static readonly HelpTopic OmniMarkdown_Intro = new HelpTopic(_omniDE, _omniDEText);
        public static readonly HelpTopic OmniMarkdown_Basics = new HelpTopic(_omniMD1, _omniMD1Text);
        public static readonly HelpTopic OmniMarkdown_Advanced = new HelpTopic(_omniMD2, _omniMD2Text);
        public static readonly HelpTopic Tags = new HelpTopic(_tags, _tagsText);

        private const string _introduction = "Introduction";
        private const string _omnis = "Omnis";
        private const string _omniDE = "   Description Editor";
        private const string _omniMD1 = "     Basic Markdown";
        private const string _omniMD2 = "     Advanced Markdown";
        private const string _tags = "Tags";
        private const string _tagManager = "Tag Manager";

        private const string _introductionText =
            "Welcome to OmniTag, a flexible knowledge repository that specializes in quick reference and lookups. " +
            "OmniTag can be used to keep track of any sort of information - shopping lists, to-do lists, step-by-step guides, " +
            "meeting minutes, notes... Anything you want to remember, OmniTag will track for you! Through the use of a " +
            "intuitive tagging system, users can mark specific tidbits of knowledge with tags that make future reference " +
            "quick and easy.";

        private const string _omnisText =
            "An **Omni** is that core part of the OmniTag application. A single Omni represents a piece of information. " +
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
            "number of markdown codes are implemented, so please see the sub-topics for more details. ";

        private const string _omniMD1Text =
            "OmniTag support the following \"basic\" commands in Markdown.\n\n" +
            "| Example Output|Code Syntax|\n" +
            "|---|---|\n" +
            "|*Italic text*|`*Italic text*`|\n" +
            "|**Bold text**|`**Bold text**`|\n" +
            "|<strike>Strike text</strike>|`<strike>Strike text</strike>`|\n" +
            "|`Code text`|\\``Code text`\\`|\n" +
            "|[Hyperlink](https://www.website_name_here.com)|`[Hyperlink](https://www.website_name_here.com)`|\n" +
            "|[Image]|`![Image alt text](image_url_here.jpg)`|";

        private const string _omniMD2Text =
            "### Lists\n\n" +
            "Making a list is as simple as beginning each item with a marker. Accepted markers are the characters " +
            "`*`, `+`, and `-`, as shown below: \n\n" +
            "` - Item 1`<br>\n" +
            "` - Item 2`<br>\n" +
            "` - Item 3`\n\n" +
            "turns into\n\n" +
            "- Item 1\n" +
            "- Item 2\n" +
            "- Item 3\n\n" +
            "Alternatively, you can use numbers as markers instead, which will create an ordered list:\n\n" +
            "`1. Item 1`<br>\n" +
            "`2. Item 2`<br>\n" +
            "`3. Item 3`\n\n" +
            "turns into\n\n" +
            "1. Item 1\n" +
            "2. Item 2\n" +
            "3. Item 3\n\n" +
            "###Tables\n\n" +
            "Tables can be created using vertical pipes (typically located above your Enter key) and hyphens, such as in the following example:\n\n" +

            "`| Column Header 1 | Column Header 2 |`<br>\n" +
            "`|-----------------|-----------------|`<br>\n" +
            "`| Columns don't have to line up| as you can see. |`<br>\n" +
            "`| Space columns between | vertical pipes.|`\n\n" +
            "This example would create the following table:\n\n" +
            "| Column Header 1 | Column Header 2 |\n" +
            "|-----------------|-----------------|\n" +
            "| Columns don't have to line up| as you can see. |\n" +
            "| Space columns between | vertical pipes.|\n\n" +
            "The `|---|` line signifies the beginning of a table.If a row appears above this line, that row will be the " +
            "*column headers*.\n\n" +
            "Tables do not have to be perfectly aligned in the code, though it does look nicer that way. As long as the " +
            "vertical pipes are placed between the content of columns, the table will format correctly.\n\n" +
            "###Code Blocks\n\n" +
            "OmniTag also supports limited syntax highlighting for certain programming languages.You can specify the " +
            "code's language by entering the following prior to any code blocks: \n\n" +
            "    ###CodeLanguage:language_here\n\n" +
            "As an example, you could enter the following C# code:\n\n" +
            "`###CodeLanguage:C#`<br>\n" +
            "`public static void TestMethod(string test)`\n\n" +
            "And the resulting output would be the following:\n\n" +
            "    ###CodeLanguage:C#\n" +
            "    public static void TestMethod(string test)\n\n" +
            "Please note that this feature is only available for code blocks, which you can create by starting each line " +
            "in the block with 4 spaces.\n" +
            "For a full list of supported languages, you can click on the \"Insert Code Text\" button in the Omni description editor.\n\n" +
            "##Further Reference\n\n" +
            "For a full list of possible Markdown commands, please see the [official site](https://daringfireball.net/projects/markdown/syntax) " +
            "for details. Most features should be implemented in OmniTag, though some features are unsupported.";

        private const string _tagsText = "A tag is a way to simply and effectively organize your Omnis. Think of a tag like a folder - you can " +
                                       "label the folder, and anything you put inside should be related. Unlike a folder, however, the benefit of " +
                                       "using tags is that a single Omni can belong to multiple tags. For example, a list of your favorite books " +
                                       "might belong to the [books] tag... but it could also belong to the [favorite], [lists], and [reading] tags. " +
                                       "\n\nTags have three key details. The tag name can be anything (up to 20 characters) and can include " +
                                       "numbers, spaces, and symbols. Tag names are unique and can't be duplicated. The Tag details are just a " +
                                       "short description to explain what sort of Omnis should belong to the given tag. Finally, a tag's " +
                                       "verification status determines if the tag has been marked as a valid tag.\n\nIn the Omni creation " +
                                       "screen, you can enter any tag names you like. If you enter the name of a tag that doesn't already " +
                                       "exist, it will be created automatically. These \"auto-tags\" will be unverified, meaning that if " +
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
                OmniMarkdown_Intro,
                OmniMarkdown_Basics,
                OmniMarkdown_Advanced,
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
