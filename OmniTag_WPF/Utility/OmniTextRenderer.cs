using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ColorCode;
using MarkdownDeep;
using NCGLib.Extensions;
using OmniTag.Models;
using OmniTagDB;

namespace OmniTagWPF.Utility
{
    static class OmniTextRenderer
    {
        public const string LangDefinitionText = "###CodeLanguage:";
        public const string CodeBlockStart = "<pre><code>";
        public const string CodeBlockEnd = "</code></pre>";
        public static readonly string DefaultEmbeddedImageLocation = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OmniTag/TempImages/");

        public static readonly Dictionary<string, ILanguage> LanguageMap;

        static OmniTextRenderer()
        {
            LanguageMap = new Dictionary<string, ILanguage>();
            var propList = typeof(Languages).GetProperties().Where(p => p.PropertyType == typeof(ILanguage));
            foreach (var propInfo in propList)
            {
                var lang = (ILanguage)propInfo?.GetValue(null);
                LanguageMap.Add(lang.Name, lang);
            }
        }

        /// <summary>
        /// Renders a given Markdown string, converting it into HTML for display in a WebBrowser. 
        /// If given a list of embedded Images, it will write the images to the temp directory
        /// first in order to guarantee that the images are there when the HTML is displayed.
        /// </summary>
        /// <param name="inputText">The original Markdown text.</param>
        /// <param name="imageFolder">The folder that will contain the temporary images.</param>
        /// <param name="images">A list of embedded images from the database.</param>
        /// <returns></returns>
        public static string Render(string inputText, string imageFolder, List<Image> images = null)
        {
            var md = new Markdown() { ExtraMode = true };
            var color = new CodeColorizer();

            if (images != null)
            {
                Directory.CreateDirectory(imageFolder);
                foreach (var img in images)
                {
                    var filePath = Path.Combine(imageFolder, img.FileName);
                    if (!File.Exists(filePath))
                        File.WriteAllBytes(filePath, img.ImageData);
                }
            }

            #region Decode embedded images

            var defaultImageLocation = imageFolder.Replace(@"\", "/");
            var outputString = Regex.Replace(inputText, @"(?<=\()(dbfile:\/\/\/)(?=.*?\))", new Uri(defaultImageLocation).AbsoluteUri);

            #endregion

            outputString = md.Transform(outputString);

            #region Colorize code blocks
            var n = 1;
            var startIndex = outputString.IndexOfNth(CodeBlockStart, n);
            while (startIndex != -1)
            {
                var endIndex = outputString.IndexOfNth(CodeBlockEnd, n);
                var chunk1 = outputString.Substring(0, startIndex);

                var chunk2 = String.Empty;
                var codeText = outputString.SubstringByIndex(startIndex + CodeBlockStart.Length, endIndex-1);
                ILanguage codeLanguage = null;
                string codeLangStr = null;
                if (codeText.StartsWith(LangDefinitionText))
                {
                    codeLangStr = codeText.Substring(0, codeText.IndexOf("\n"));
                    codeLanguage = ParseCodeLanguage(codeLangStr.Trim());
                }
                if (codeLanguage != null)
                {
                    codeText = codeText.Replace(codeLangStr, String.Empty);

                    //The Colorize method adds an extra newline, so remove it
                    codeText = codeText.Trim();

                    chunk2 = color.Colorize(codeText, codeLanguage);
                }

                var chunk3 = (chunk2 == String.Empty)
                    ? outputString.Substring(startIndex)
                    : outputString.Substring(endIndex + CodeBlockEnd.Length);

                outputString = chunk1 + chunk2 + chunk3;

                startIndex = outputString.IndexOfNth(CodeBlockStart, ++n);
            }
            #endregion
            
            // HTML styling for tables so they have borders between cells
            outputString = "<style>" +
                                "table, th, td { border:1px solid black; border-collapse: collapse; } " +
                                "th, td { padding-right: 5px; padding-left: 5px; }" +
                             "</style>" + 
                             outputString;
            
            return outputString;
        }

        private static ILanguage ParseCodeLanguage(string str)
        {
            var startsWith = "###CodeLanguage:";
            if (!str.StartsWith(startsWith))
                return null;

            var lang = str.Substring(startsWith.Length);
            ILanguage retVal;
            if (LanguageMap.TryGetValue(lang, out retVal))
                return retVal;
            else
                return null;
        }
    }
}
