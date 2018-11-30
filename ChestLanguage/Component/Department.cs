using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChestLanguage
{
    public class uDepartment : Chest
    {
        public IList<Chest> Chests { get; set; }

        public uDepartment()
        {
            Chests = new List<Chest>();
        }
    }
}
