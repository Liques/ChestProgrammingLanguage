using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChestTranspiler
{
    public class CodeBlock
    {
        private const string tabDelimiter = "\\t";
        private const string tabSpaceSize = "    ";

        public string ID { get; set; }
        public CodeLine Line { get; set; }
        public IList<CodeBlock> TextBlocks { get; set; }

        private CodeBlock()
        {
            this.ID = Guid.NewGuid().ToString();
        }

        public static IEnumerable<CodeBlock> Convert(string codeText)
        {
            var codeLines = codeText.Split(Environment.NewLine.ToCharArray())
                                    .Where(w =>
                                        // Clean empty rows
                                        w.Length != 0
                                        )
                                    .SelectMany(s =>
                                        // Cut by signals
                                        CutBySignals(s)
                                     )
                                    .Select(s =>
                                        // Replace space to tab
                                        DelimitTab(s)
                                     )
                                    .ToList();


            return Convert(codeLines);
        }

        public string ValueConcat()
        {
            var builder = new StringBuilder();
            builder.Append(this.Line.Value);

            foreach (var block in this.TextBlocks ?? new CodeBlock[] { })
            {
                builder.Append(" ");
                builder.Append(block.Line.Value);
            }

            return builder.ToString();
        }


        private static CodeBlock ConvertToSingleBlock(IList<string> codeLines)
        {
            if (codeLines == null || codeLines.Count() == 0)
                return null;

            var textBlock = new CodeBlock();
            textBlock.Line = new CodeLine(codeLines.Single(w => !w.StartsWith(tabDelimiter)));
            textBlock.TextBlocks = Convert(codeLines.Where(w => w.StartsWith(tabDelimiter))
                .Select(s => s.Substring(s.Length >= 2 ? 2 : 0, s.Length - 2)).ToArray());

            if (textBlock.TextBlocks.Count == 0 || textBlock.TextBlocks.First() == null)
            {
                textBlock.TextBlocks = null;
            }

            return textBlock;

        }

        public static IList<CodeBlock> Convert(IList<string> codeLines)
        {
            var blockOfLines = new List<IList<string>>();

            var currentBlock = -1;

            for (int i = 0; i < codeLines.Count(); i++)
            {
                var normalBlockLine = codeLines[i].StartsWith(tabDelimiter);

                if (codeLines[i].Contains("number11"))
                {
                }

                if (!normalBlockLine)// && blockOfLines.Count < currentBlock++)
                {
                    blockOfLines.Add(new List<string>());
                    currentBlock++;
                }

                blockOfLines[currentBlock].Add(codeLines[i]);

            }

            return blockOfLines.Select(s => ConvertToSingleBlock(s)).ToList();

        }

        private static string DelimitTab(string line)
        {
            return line.Replace(tabSpaceSize, tabDelimiter);

            var lineCharacters = line.ToArray();

            var newLine = new StringBuilder();

            bool startEnded = false;

            for (int i = 0; i < lineCharacters.Length; i++)
            {

                if (!startEnded && lineCharacters[i] == ' ')
                {
                    newLine.Append(@"\t");
                }
                else
                {
                    startEnded = true;
                    newLine.Append(lineCharacters[i]);
                }


            }

            return newLine.ToString();

        }


        private static IList<string> CutBySignals(string line)
        {
            if (line.IndexOf("->") < 0)
                return new List<string> { line };



            int numberTabs = Regex.Split(line, tabSpaceSize).Length - 1;
            var listLines = Regex.Split(line, "->")
                .Select(s => s.Trim())
                //.Where(w => !String.IsNullOrEmpty(w))
                .ToArray();

            for (int i = 0; i < listLines.Length; i++)
            {
                for (int j = 0; j < numberTabs; j++)
                {
                    listLines[i] = tabSpaceSize + listLines[i];
                }
                numberTabs++;
            }

            return listLines;

        }

#if DEBUG
        public override string ToString()
        {
            return this.Line.ToString() ?? "--no line--";
        }
#endif
    }
}
