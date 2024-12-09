using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods
{
    public class SubTableView
    {
        public int SubId { get; set; }
        public string SubName { get; set; }
        public string DepName { get; set; } // Name from DepTable
        public string BranchName { get; set; } // Name from BranchTable
        public int SubClass { get; set; }
    }

}
