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
        public string DepName { get; set; }
    }

    public class StdTable
    {
        [PrimaryKey]
        public int StdId { get; set; }
        public string StdName { get; set; }

        public string StdDep { get; set; }
        public string StdBranch { get; set; }
        public int StdClass { get; set; }

    }
}
