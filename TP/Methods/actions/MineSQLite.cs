using Javax.Security.Auth;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods.actions
{
    internal class MineSQLite
    {
        public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

        private readonly SQLiteAsyncConnection _database;

        public MineSQLite()
        {
            _database = new SQLiteAsyncConnection(dbPath);

        }

        public async Task<UsersAccountTable> getUserAccountById(int userId)
        {
            var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(u => u.UserId == userId);
            return user;
        }
        public async Task<UserSessionTable> UserSessionChecker()
        {
            var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
            return session;
            
        }
        public async Task<int> deleteSession()
        {
            var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
            if (session != null)
            {
                return await _database.DeleteAsync(session);
            }
            return 0;
        }

        public async Task<UsersAccountTable> UserLoginChecker(string username, string password)
        {
            var IfUserExist = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == username && d.Password == password);
            return IfUserExist;
        }

        public async Task<UsersAccountTable> loginSecction(string password, int userid)
        {
            var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(u => u.Password == password && u.UserId == userid );
            return user;
        }

        //  Subject Section 
 
        public async Task<List<SubTable>> getSubTable()
        {
            var allSubjects = await _database.Table<SubTable>().ToListAsync();
            return allSubjects;
        }

        public async Task<List<RequestJoinSubject>> getRequestJoinBySubIdAndUserId(int SubId)
        {
           var RequestJoin = await _database.Table<RequestJoinSubject>().Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToListAsync();
            return RequestJoin;
        }

        public async Task<List<DegreeTable>> getDegreeTableBySubIdAndStdName(int SubId)
        {
           var DegreeTable = await _database.Table<DegreeTable>().Where(s => s.SubId == SubId && s.StdName == UserSession.Name).ToListAsync();
            return DegreeTable;
        }

        public async Task<SubjectAssignments> getSubjectASsignmentByPostIdAndStdId(int postId)
        {
            var assignment = await _database.Table<SubjectAssignments>().FirstOrDefaultAsync(a => a.PostId == postId && a.StdName == UserSession.Name);
            return assignment;
        }
        public async Task<List<SubTable>> getSubByUser()
        {
            var teacherSubjects = await _database.Table<SubTable>()
                                                    .Where(s => s.UserId == UserSession.UserId)
                                                    .ToListAsync();
            return teacherSubjects;
        }

        public async Task<SubTable> getSubBySubId(int subId)
        {
            var SubTable = await _database.Table<SubTable>()
                .Where(s => s.SubId == subId)
                .FirstOrDefaultAsync();
            return SubTable;
        }

        public async Task<int> updateSubBySubId(SubTable sub)
        {
            int rows = await _database.UpdateAsync(sub);
            return rows;
        }

        public async Task<List<DegreeTable>> getDegreeTablesBySubId(int SubId)
        {
            var allDegrees = await _database.Table<DegreeTable>().Where(i => i.SubId == SubId).ToListAsync();
            return allDegrees;
        }

        public async Task<List<DegreeTable>> getDegreeBySessionName()
        {
            var teacherDegrees = await _database.Table<DegreeTable>()
                                                    .Where(s => s.StdName == UserSession.Name)
                                                    .ToListAsync();
            return teacherDegrees;
        }

        public async Task<int> insertSession(UserSessionTable session)
        {
            int rows = await _database.InsertAsync(session);
            return rows;
        }

        public async Task<int> insertRequestJoin(RequestJoinSubject request)
        {
            int rows = await _database.InsertAsync(request);
            return rows;
        }

        public async Task<int> insertSub(SubTable subTable)
        {
            int rows = await _database.InsertAsync(subTable);
            return rows;

        }

        public async Task<int> insertSubjectAssignment(SubjectAssignments assignment)
        {
            int rows = await _database.InsertAsync(assignment);
            return rows;
        }

        public async Task<List<SubjectAssignments>> getSubjectAssignmentsByPost(int postId)
        {
            var assignments = await _database.Table<SubjectAssignments>()
            .Where(a => a.PostId == postId)
            .ToListAsync();
            return assignments;
        }


        public async Task<List<SubjectPosts>> getSubjectPosts()
        {
            var posts = await _database.Table<SubjectPosts>().ToListAsync();
            return posts;
        }

        public async Task<SubjectPosts> getSubjectPost(int postId)
        {
            var post = await _database.Table<SubjectPosts>().FirstOrDefaultAsync(p => p.PostId == postId);
            return post;
        }

        public async Task<int> deleteSubjectPost(int postId)
        {
            var post = await getSubjectPost(postId);
            int rows = await _database.DeleteAsync(post);
            return rows;
        }

        public async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            int row = await _database.InsertAsync(subjectPosts);
            return row;
        }

        public async Task<int> updateSubjectPost(SubjectPosts subjectPosts)
        {
            int row = await _database.UpdateAsync(subjectPosts);
            return row;
        }

        public async Task<List<RequestJoinSubject>> getRequestJoinSubjectsBySubId(int subId)
        {
            var requests = await _database.Table<RequestJoinSubject>().Where(d => d.SubId == subId).ToListAsync();
            return requests;
        }

        public async Task<List<RequestJoinSubject>> getREquestJoinSubjectBySubIdAndUserId(int SubId)
        {
            var RequestJoin = await _database.Table<RequestJoinSubject>().Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToListAsync();
            return RequestJoin;
        }

        public async Task<int> insertDegree(DegreeTable degreeTable)
        {
            int rows = await _database.InsertAsync(degreeTable);
            return rows;
        }

        public async Task<int> deleteRequestJoin(int requestId)
        {
            var req = await _database.Table<RequestJoinSubject>().FirstOrDefaultAsync(d => d.ReqId == requestId);
            int rows = await _database.DeleteAsync(req);
            return rows;

        }

        public async Task<List<SubjectBooks>> getSubjectBooksBySubId(int subId)
        {
            var books = await _database.Table<SubjectBooks>().Where(b => b.SubId == subId).ToListAsync();
            return books;
        }   
        public async Task<int> insertSubjectBook(SubjectBooks book)
        {
            int rows = await _database.InsertAsync(book);
            return rows;
        }


        public async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            var posts = await _database.Table<SubjectPosts>().Where(b => b.SubId == subId).ToListAsync();
            return posts;
        }


        public async Task<int> deleteDegree(DegreeTable degreeTable)
        {
            int rows = await _database.DeleteAsync(degreeTable);
            return rows;
        }

        public async Task<int> deleteSubjectBook(SubjectBooks book)
        {
            int rows = await _database.DeleteAsync(book);
            return rows;
        }

        public async Task<int> deletePost(SubjectPosts post)
        {
            int rows = await _database.DeleteAsync(post);
            return rows;
        }

        public async Task<int> deleteSub(SubTable sub)
        {
            int rows = await _database.DeleteAsync(sub);
            return rows;
        }   

        public async Task<DegreeTable> getDegreeByStdNameAndSubId(string stdName, int subId)
        {
            var degree = await _database.Table<DegreeTable>()
                .FirstOrDefaultAsync(d => d.StdName == stdName && d.SubId == subId);
            return degree;
        }

        public async Task<List<SubTable>> searchSubTable(string searchText)
        {
            var subjects = await _database.Table<SubTable>()
                .Where(s => s.SubName.Contains(searchText) || s.SubTeacherName.Contains(searchText))
                .ToListAsync();
            return subjects;
        }

        public async Task<int> updateDegree(DegreeTable degree)
        {
            int rows = await _database.UpdateAsync(degree);
            return rows;
        }


    }
}
