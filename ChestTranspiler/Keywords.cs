using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestTranspiler
{
    public static class Keywords
    {
        public const string KeywordAttach = "attach";
        public const string KeywordBuilding = "building";
        public const string KeywordOffice = "office";
        public const string KeywordEmployee = "employee";
        public const string KeywordChest = "chest";
        public const string KeywordGo = "go";
        public const string KeywordDecide = "decide";
        public const string KeywordPoke = "poke";
        public const string KeywordSketch = "sketch";
        public const string KeywordInstruction = "instruction";
        public const string KeywordNeed = "need";
        public const string KeywordCreate = "create";

        public static IEnumerable<string> Keys { get; set; }
        public static IEnumerable<string> Modifiers { get; set; }

        static Keywords()
        {
            Keys = new string[] {
                KeywordAttach,
                KeywordBuilding,
                KeywordOffice,
                KeywordEmployee,
                KeywordChest,
                KeywordGo,
                KeywordDecide,
                KeywordPoke,
                KeywordSketch,
                KeywordInstruction,
                KeywordNeed,
                KeywordCreate,
            };
            
            Modifiers = new string[] {
                "open door",
                "closed door",
                "exclusive door",
            };
        }

        public static bool IsKeyword(string keyword, string targetKeyword)
        {
            return targetKeyword.Equals(keyword,StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
