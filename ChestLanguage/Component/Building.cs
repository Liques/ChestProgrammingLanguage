using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestLanguage
{
    public class uBuilding : ILanguageComponent
    {
        public IList<uAttach> Attachments { get; set; }
        public IList<uLevel> Levels { get; set; }

        public uBuilding()
        {
            Attachments = new List<uAttach>();
            Levels = new List<uLevel>();
        }
    }
}
