using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP
{
    public class UsersAccountTable
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string Name { get; set; }
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; } //1 = admin, 2 = Teacher, 3 = Student
    }
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


    public class SubTable
    {
        [PrimaryKey, AutoIncrement]
        public int SubId { get; set; }
        public string SubName { get; set; }
        public int SubDep { get; set; } //foreign key to DepTable
        public int SubBranch { get; set; }//foreign key to BranchTable
        public int SubClass { get; set; }
        public bool ShowDeg {  get; set; }
        public int UserId { get; set; } //Foreign key to user
    }
    public class SubForStdTable
    {
        public int SubId { get; set; } //foreignkey to Subject
        public int StdId { get; set; } //foreignkey to UsertsAccount UserId type 3
        public float Deg { get; set; }
        public float MiddelDeg{ get; set; }

        //public float TotalDeg = Deg + MiddelDeg;

    }



}
