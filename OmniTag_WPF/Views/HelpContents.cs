namespace OmniTagWPF.Views
{
    static class HelpContents
    {
        public const string Introduction = "Introduction";
        public const string Omnis = "Omnis";
        public const string Tags = "Tags";

        public const string IntroductionText = "Welcome to OmniTagger, the flexible organization solution that can be used to sort " +
                                               "effectively anything.";

        public const string OmnisText = "An Omni is a way to store a single piece of information into a single concrete record. Omnis " +
                                        "can contain many types of information - a step-by-step set of instructions on how to bake a pie, " +
                                        "a friend's birthday date, a shopping list, etc... Practically anything that you want to document " +
                                        "in some way can be stored in an Omni.\n\nEach Omni is made up of three main parts: a summary, " +
                                        "a description, and the associated tags. The summary is merely a description of the contents of an " +
                                        "Omni (for example, \"Recipe for Chocolate Cake\"). The description is the details (\"1 cup sugar, " +
                                        "2 cups flour, 10 buckets chocolate milk. Mix all ingredients together, bake for an hour, then eat.\" " +
                                        "Finally, the tags are the method of organizing your Omnis to be in discrete groups. Our recipe " +
                                        "example might have the tags [recipe],[food], and [guilty pleasure].\n\n(Side note: Don't try that " +
                                        "recipe. I'm pretty sure you'll end up with choco-chunk milksoup.)";

        public const string TagsText = "A Tag is a way to simply and effectively organize your Omnis. Think of a Tag like a folder - you can " +
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
    }
}
