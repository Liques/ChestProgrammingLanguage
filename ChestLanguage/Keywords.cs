using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestLanguage
{
    public static class Keywords
    {
        public const string KeywordAttach = "attach";
        public const string KeywordBuilding = "building";
        public const string KeywordLevel = "level";
        public const string KeywordDepartment = "department";
        public const string KeywordChest = "chest";
        public const string KeywordGo = "go";
        public const string KeywordJudge = "judge";
        public const string KeywordPoke = "poke";

        public static IEnumerable<string> Keys { get; set; }
        public static IEnumerable<string> Modifiers { get; set; }

        static Keywords()
        {
            Keys = new string[] {
                KeywordAttach,
                KeywordBuilding,
                KeywordLevel,
                KeywordDepartment,
                KeywordChest,
                KeywordGo,
                KeywordJudge,
                KeywordPoke,
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
