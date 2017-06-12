using System;
using System.Collections.Generic;
using System.Linq;
using ColorCode;
using HeyRed.MarkdownSharp;

namespace OmniTagWPF.Utility
{
    static class OmniTextRenderer
    {
        public const string LangDefinitionText = "###CodeLanguage:";

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

        public static string Render(string inputText)
        {
            var strArray = inputText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var codeBlocks = GetCodeBlocks(strArray);
            var outputString = GetRenderedHtml(strArray, codeBlocks);

            return "<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'>"
                   + outputString;
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

        private static List<CodeBlock> GetCodeBlocks(string[] inputArray)
        {
            var codeBlocks = new List<CodeBlock>();
            CodeBlock currentCodeBlock = null;
            
            for (var index = 0; index < inputArray.Length; index++)
            {
                var str = inputArray[index];
                if (!str.StartsWith("    ")) // four spaces
                {
                    if (currentCodeBlock != null)
                    {
                        codeBlocks.Add(currentCodeBlock);
                    }

                    currentCodeBlock = null;
                    continue;
                }



                if (currentCodeBlock == null)
                {
                    currentCodeBlock = new CodeBlock
                    {
                        CodeLanguage = ParseCodeLanguage(str.Trim()),
                        StartingLine = index,
                        NumLines = 0
                    };
                    if (currentCodeBlock.CodeLanguage != null)
                    {
                        currentCodeBlock.NumLines++;
                        continue; // do not include the code language definition in the output
                    }
                }

                if (currentCodeBlock.CodeLanguage == null)
                    currentCodeBlock.CodeText += str + Environment.NewLine;
                else
                    currentCodeBlock.CodeText += str.Substring(3) + Environment.NewLine; // remove first four spaces
                currentCodeBlock.NumLines++;
            }
            if (currentCodeBlock != null)
                codeBlocks.Add(currentCodeBlock);

            return codeBlocks;
        }

        private static string GetRenderedHtml(string[] inputArray, List<CodeBlock> codeBlocks)
        {
            var md = new Markdown();
            var color = new CodeColorizer();

            var outputString = String.Empty;
            var skipCount = 0;

            for (var index = 0; index < inputArray.Length; index++)
            {
                if (skipCount > 0)
                {
                    skipCount--;
                    continue;
                }

                var codeBlock = codeBlocks.SingleOrDefault(cb => cb.StartingLine == index);
                if (codeBlock == null)
                    outputString += md.Transform(inputArray[index]);
                else
                {
                    if (codeBlock.CodeLanguage == null)
                        outputString += md.Transform(codeBlock.CodeText);
                    else
                    {
                        // The Colorize method adds an extra newline, so remove it
                        if (codeBlock.CodeText.EndsWith(Environment.NewLine))
                            codeBlock.CodeText = codeBlock.CodeText.Substring(0, codeBlock.CodeText.Length - Environment.NewLine.Length);

                        outputString += color.Colorize(codeBlock.CodeText, codeBlock.CodeLanguage);
                    }


                    skipCount = codeBlock.NumLines - 1; // Minus one because we already processed this line
                }
            }

            return outputString;
        }


        private class CodeBlock
        {
            public int StartingLine { get; set; }
            public string CodeText { get; set; }
            public ILanguage CodeLanguage { get; set; }
            public int NumLines { get; set; }
        }
    }
}
