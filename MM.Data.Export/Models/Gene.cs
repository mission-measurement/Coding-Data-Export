using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MM.Data.Export
{
    public class Gene
    {
        public long GeneID { get; set; }
        public string Name { get; set; }
        public bool IsPresent { get; set; }

        public long GenomeID { get; set; }

        public Gene() { }
      
    }
}
