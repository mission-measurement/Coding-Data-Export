using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Data.Export
{
    public class Outcome
    {
        public long OutcomeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPresent { get; set; }
    }
}
