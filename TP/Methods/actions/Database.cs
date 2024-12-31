using Kotlin.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods.actions
{
    internal class Database
    {
        public static Database SelectedDatabase;
        public virtual void DatabaseStarted()
        {
        }
        public virtual async Task<UsersAccountTable> getUserAccountById(int userId)
        {
            return new UsersAccountTable();
        }
        public virtual async Task<UserSessionTable> UserSessionChecker()
        {
            return new UserSessionTable();
        }
        public virtual async Task<int> deleteSession()
        {
            return new int();
        }
        public virtual async Task<UsersAccountTable> UserLoginChecker(string username, string password)
        {
            return new UsersAccountTable();
        }
        public virtual async Task<UsersAccountTable> loginSecction(string password, int userid)
        {
            return new UsersAccountTable();
        }
        public virtual async Task<List<SubTable>> getSubTable()
        {
            return new List<SubTable>();
        }
        public virtual async Task<List<RequestJoinSubject>> getRequestJoinBySubIdAndUserId(int SubId)
        {
            return new List<RequestJoinSubject>();
        }
        public virtual async Task<List<DegreeTable>> getDegreeTableBySubIdAndStdName(int SubId)
        {
            return new List<DegreeTable>();
        }
        public virtual async Task<SubjectAssignments> getSubjectASsignmentByPostIdAndStdId(int postId)
        {
            return new SubjectAssignments();
        }
        public virtual async Task<List<SubTable>> getSubByUser()
        {
            return new List<SubTable>();
        }
        public virtual async Task<SubTable> getSubBySubId(int subId)
        {
            return new SubTable();
        }
        public virtual async Task<int> updateSubBySubId(SubTable sub)
        {
            return new int();
        }
        public virtual async Task<List<DegreeTable>> getDegreeTablesBySubId(int SubId)
        {
            return new List<DegreeTable>();
        }
        public virtual async Task<List<DegreeTable>> getDegreeBySessionName()
        {
            return new List<DegreeTable>();
        }
        public virtual async Task<int> insertSession(UserSessionTable session)
        {
            return new int();
        }
        public virtual async Task<int> insertRequestJoin(RequestJoinSubject request)
        {
            return new int();
        }
        public virtual async Task<int> insertSub(SubTable subTable)
        {
            return new int();
        }
        public virtual async Task<int> insertSubjectAssignment(SubjectAssignments assignment)
        {
            return new int();
        }
        public virtual async Task<List<SubjectAssignments>> getSubjectAssignmentsByPost(int postId)
        {
            return new List<SubjectAssignments>();
        }
        public virtual async Task<List<SubjectPosts>> getSubjectPosts()
        {
            return new List<SubjectPosts>();
        }
        public virtual async Task<SubjectPosts> getSubjectPost(int postId)
        {
            return new SubjectPosts();
        }
        public virtual async Task<int> deleteSubjectPost(int postId)
        {
            return new int();
        }
        public virtual async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            return new int();
        }
        public virtual async Task<int> updateSubjectPost(SubjectPosts subjectPosts)
        {
            return new int();
        }
        public virtual async Task<List<RequestJoinSubject>> getRequestJoinSubjectsBySubId(int subId)
        {
            return new List<RequestJoinSubject>();
        }
        public virtual async Task<List<RequestJoinSubject>> getREquestJoinSubjectBySubIdAndUserId(int SubId)
        {
            return new List<RequestJoinSubject>();
        }
        public virtual async Task<int> insertDegree(DegreeTable degreeTable)
        {
            return new int();
        }
        public virtual async Task<int> deleteRequestJoin(int requestId)
        {
            return new int();
        }
        public virtual async Task<List<SubjectBooks>> getSubjectBooksBySubId(int subId)
        {
            return new List<SubjectBooks>();
        }
        public virtual async Task<int> insertSubjectBook(SubjectBooks book)
        {
            return new int();
        }
        public virtual async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            return new List<SubjectPosts>();
        }
        public virtual async Task<int> deleteDegree(DegreeTable degreeTable)
        {
            return new int();
        }
        public virtual async Task<int> deleteSubjectBook(SubjectBooks book)
        {
            return new int();
        }
        public virtual async Task<int> deletePost(SubjectPosts post)
        {
            return new int();
        }
        public virtual async Task<int> deleteSub(SubTable sub)
        {
            return new int();
        }
        public virtual async Task<DegreeTable> getDegreeByStdNameAndSubId(string stdName, int subId)
        {
            return new DegreeTable();
        }
        public virtual async Task<List<SubTable>> searchSubTable(string searchText)
        {
            return new List<SubTable>();
        }
        public virtual async Task<int> updateDegree(DegreeTable degree)
        {
            return new int();
        }
    }
}
