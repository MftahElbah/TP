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

    public class SubTable
    {
        [PrimaryKey, AutoIncrement]
        public int SubId { get; set; }
        public string SubName { get; set; }
        public int SubDep { get; set; } //foreign key to DepTable
        public int SubBranch { get; set; }//foreign key to BranchTable
        public int SubClass { get; set; }

    }
    public class SubForStdTable
    {
        public int SubId { get; set; }
        public int StdId { get; set; }
        public float Deg { get; set; }
        public float MiddelDeg{ get; set; }

        //public float TotalDeg = Deg + MiddelDeg;

    }

}
