using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestCompiler
{
    public class CodeLine
    {
        public string Keyword { get; set; }
        public string Value { get; set; }
        public string Modifier { get; set; }
        public string Line { get; set; }

        public CodeLine(string line)
        {
            this.Line = line;   
            this.Keyword = Keywords.Keys.FirstOrDefault(f => line.StartsWith(f, StringComparison.InvariantCultureIgnoreCase));
            this.Modifier = Keywords.Modifiers.FirstOrDefault(f => line.EndsWith(f, StringComparison.InvariantCultureIgnoreCase));

            this.Value = line.Replace(this.Keyword ?? "#$%^&*()", String.Empty).Replace(this.Modifier ?? "#$%^&*()", String.Empty).Trim();           

        }

        public override string ToString()
        {
            return this.Line;
        }
    }
}
