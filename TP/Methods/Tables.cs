using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP
{
    //to save Session to not resignup everytime - Offline
    public class UserSessionTable
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string Password { get; set; }
    }
    //Online
    public class UsersAccountTable
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string Name { get; set; }
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; } //1 = Teacher, 2 = Student
        public bool IsActive { get; set; }
    }
    
    //Online
    public class SubTable
    {
        [PrimaryKey, AutoIncrement]
        public int SubId { get; set; }
        public string SubName { get; set; }
        public bool ShowDeg { get; set; }
        public int UserId { get; set; } //FORIGN KEY ref to UserAccountTable
        public string SubTeacherName { get; set; }
    }
    //Online
    public class DegreeTable
    {
        [PrimaryKey, AutoIncrement]
        public int DegId { get; set; } //foreignkey to Subject
        public int SubId { get; set; } //foreignkey to Subject
        public string StdName { get; set; } //foreignkey to UsertsAccount UserId type 3
        public float Deg { get; set; }
        public float MiddelDeg { get; set; }
        public float Total
        {
            get { return Deg + MiddelDeg; }
        }
    }
    //Online
    public class RequestJoinSubject
    {
        [PrimaryKey, AutoIncrement]
        public int ReqId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int SubId { get; set; }
        public DateTime RequestDate { get; set; }
    }
    
    //Online
    public class SubjectBooks
    {
        [PrimaryKey, AutoIncrement]
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int SubId { get; set; }
        public string BookFile { get; set; }
        public DateTime UploadDate { get; set; }
    }
    
    //Online
    public class SubjectPosts
    {
        [PrimaryKey, AutoIncrement]
        public int PostId { get; set; }
        public int SubId { get; set; }
        public string PostTitle { get; set; }
        public string PostDes { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime? DeadLineTime { get; set; }
        //public Byte[] PostDesFile { get; set; }
        public string PostFileLink { get; set; }
        
    }
    
    //Online
    public class SubjectAssignments{
        public int PostId { get; set; }
        public int StdId { get; set; }
        public string StdName { get; set; }
        public string AssignmentFile { get; set; }
        //public string FileType { get; set; }

    }
    

    public class SchedulerTask
    {
        [PrimaryKey, AutoIncrement]
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDes { get; set; }
        public DateTime TaskStartTime { get; set; }
        public DateTime TaskEndTime { get; set; }
        public string TaskColor { get; set; } // Property to store the selected color
        public int UserId { get; set; }
        
    }
}