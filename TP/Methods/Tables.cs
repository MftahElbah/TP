using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP
{
    public class DepTable
    {
        [PrimaryKey, AutoIncrement]
        public int DepId { get; set; }
        public string DepName { get; set; }
    }

    public class BranchTable
    {
        [PrimaryKey, AutoIncrement]
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string DepName { get; set; } // Changed from DepName to DepId for foreign key reference
    }
}
