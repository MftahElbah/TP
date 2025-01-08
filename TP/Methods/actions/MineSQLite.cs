using Firebase;
using SQLite;

namespace TP.Methods.actions
{
    internal class MineSQLite : Database
    {
        public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

        private readonly SQLiteAsyncConnection _database;

        public MineSQLite()
        {
            _database = new SQLiteAsyncConnection(GetDatabasePath());

        }

        private string GetDatabasePath()
        {
            // Path for SQLite file
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(folderPath, "YourDatabaseName.db");
        }
        public override async void DatabaseStarted()
        {
            await _database.CreateTableAsync<UserSessionTable>();
            await _database.CreateTableAsync<SubTable>();
            await _database.CreateTableAsync<UsersAccountTable>();
            await _database.CreateTableAsync<RequestJoinSubject>();
            await _database.CreateTableAsync<DegreeTable>();
            await _database.CreateTableAsync<SubjectPosts>();
            await _database.CreateTableAsync<SchedulerTask>();
            //await _database.CreateTableAsync<SubjectBooks>();
            //await _database.CreateTableAsync<SubjectAssignments>();
            await SeedDatabase(); // Calls the method to seed the database with initial data if needed.

        }
        private async Task SeedDatabase(){
            var teacher = await _database.Table<UsersAccountTable>().ToListAsync();
            if (teacher.Count == 0)
            {
                var initialTeacher = new List<UsersAccountTable>
                {
                    new UsersAccountTable {UserId=111,Name= "test",Username = "t" , Password="1" , UserType=1 },
                    new UsersAccountTable {UserId=123,Name= "stest",Username = "s" , Password="1" , UserType=2 }
                };
                await _database.InsertAllAsync(initialTeacher); // Inserts the initial Teacher Account into the database.
            }
        }
        public override async Task<UsersAccountTable> getUserAccountById(int userId)
        {
            var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(u => u.UserId == userId);
            return user;
        }
        public override async Task<UserSessionTable> UserSessionChecker()
        {
            var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
            return session;
            
        }
        public override async Task<int> deleteSession()
        {
            var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
            if (session != null)
            {
                return await _database.DeleteAsync(session);
            }
            return 0;
        }

        public override async Task<UsersAccountTable> UserLoginChecker(string username, string password)
        {
            var IfUserExist = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == username && d.Password == password);
            return IfUserExist;
        }

        public override async Task<UsersAccountTable> loginSecction(string password, int userid)
        {
            var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(u => u.Password == password && u.UserId == userid );
            return user;
        }

        //  Subject Section 
 
        public override async Task<List<SubTable>> getSubTable()
        {
            var allSubjects = await _database.Table<SubTable>().ToListAsync();
            return allSubjects;
        }

        public override async Task<List<RequestJoinSubject>> getRequestJoinBySubIdAndUserId(int SubId)
        {
           var RequestJoin = await _database.Table<RequestJoinSubject>().Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToListAsync();
            return RequestJoin;
        }

        public override async Task<List<DegreeTable>> getDegreeTableBySubIdAndStdName(int SubId)
        {
           var DegreeTable = await _database.Table<DegreeTable>().Where(s => s.SubId == SubId && s.StdName == UserSession.Name).ToListAsync();
            return DegreeTable;
        }

        /*public override async Task<SubjectAssignments> getSubjectASsignmentByPostIdAndStdId(int postId)
        {
            var assignment = await _database.Table<SubjectAssignments>().FirstOrDefaultAsync(a => a.PostId == postId && a.StdName == UserSession.Name);
            return assignment;
        }*/
        public override async Task<List<SubTable>> getSubByUser()
        {
            var teacherSubjects = await _database.Table<SubTable>()
                                                    .Where(s => s.UserId == UserSession.UserId)
                                                    .ToListAsync();
            return teacherSubjects;
        }

        public override async Task<SubTable> getSubBySubId(int subId)
        {
            var SubTable = await _database.Table<SubTable>()
                .Where(s => s.SubId == subId)
                .FirstOrDefaultAsync();
            return SubTable;
        }

        public override async Task<int> updateSubBySubId(SubTable sub)
        {
            int rows = await _database.UpdateAsync(sub);
            return rows;
        }

        public override async Task<List<DegreeTable>> getDegreeTablesBySubId(int SubId)
        {
            var allDegrees = await _database.Table<DegreeTable>().Where(i => i.SubId == SubId).ToListAsync();
            return allDegrees;
        }

        public override async Task<List<DegreeTable>> getDegreeBySessionName()
        {
            var teacherDegrees = await _database.Table<DegreeTable>()
                                                    .Where(s => s.StdName == UserSession.Name)
                                                    .ToListAsync();
            return teacherDegrees;
        }

        public override async Task<int> insertSession(UserSessionTable session)
        {
            int rows = await _database.InsertAsync(session);
            return rows;
        }

        public override async Task<int> insertRequestJoin(RequestJoinSubject request)
        {
            int rows = await _database.InsertAsync(request);
            return rows;
        }

        public override async Task<int> insertSub(SubTable subTable)
        {
            int rows = await _database.InsertAsync(subTable);
            return rows;

        }
        /*
        public override async Task<int> insertSubjectAssignment(SubjectAssignments assignment)
        {
            int rows = await _database.InsertAsync(assignment);
            return rows;
        }
        public override async Task<List<SubjectAssignments>> getSubjectAssignmentsByPost(int postId)
        {
            var assignments = await _database.Table<SubjectAssignments>()
            .Where(a => a.PostId == postId)
            .ToListAsync();
            return assignments;
        }
        */


        public override async Task<List<SubjectPosts>> getSubjectPosts()
        {
            var posts = await _database.Table<SubjectPosts>().ToListAsync();
            return posts;
        }

        public override async Task<SubjectPosts> getSubjectPost(int postId)
        {
            var post = await _database.Table<SubjectPosts>().FirstOrDefaultAsync(p => p.PostId == postId);
            return post;
        }

        public override async Task<int> deleteSubjectPost(int postId)
        {
            var post = await getSubjectPost(postId);
            int rows = await _database.DeleteAsync(post);
            return rows;
        }

        public override async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            int row = await _database.InsertAsync(subjectPosts);
            return row;
        }

        public override async Task<int> updateSubjectPost(SubjectPosts subjectPosts)
        {
            int row = await _database.UpdateAsync(subjectPosts);
            return row;
        }

        public override async Task<List<RequestJoinSubject>> getRequestJoinSubjectsBySubId(int subId)
        {
            var requests = await _database.Table<RequestJoinSubject>().Where(d => d.SubId == subId).ToListAsync();
            return requests;
        }

        public override async Task<List<RequestJoinSubject>> getREquestJoinSubjectBySubIdAndUserId(int SubId)
        {
            var RequestJoin = await _database.Table<RequestJoinSubject>().Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToListAsync();
            return RequestJoin;
        }

        public override async Task<int> insertDegree(DegreeTable degreeTable)
        {
            int rows = await _database.InsertAsync(degreeTable);
            return rows;
        }

        public override async Task<int> deleteRequestJoin(int requestId)
        {
            var req = await _database.Table<RequestJoinSubject>().FirstOrDefaultAsync(d => d.ReqId == requestId);
            int rows = await _database.DeleteAsync(req);
            return rows;

        }
/*
        public override async Task<List<SubjectBooks>> getSubjectBooksBySubId(int subId)
        {
            var books = await _database.Table<SubjectBooks>().Where(b => b.SubId == subId).ToListAsync();
            return books;
        }   
        public override async Task<int> insertSubjectBook(SubjectBooks book)
        {
            int rows = await _database.InsertAsync(book);
            return rows;
        }
*/

        public override async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            var posts = await _database.Table<SubjectPosts>().Where(b => b.SubId == subId).ToListAsync();
            return posts;
        }


        public override async Task<int> deleteDegree(DegreeTable degreeTable)
        {
            int rows = await _database.DeleteAsync(degreeTable);
            return rows;
        }
        /*
        public override async Task<int> deleteSubjectBook(SubjectBooks book)
        {
            int rows = await _database.DeleteAsync(book);
            return rows;
        }
        */
        public override async Task<int> deletePost(SubjectPosts post)
        {
            int rows = await _database.DeleteAsync(post);
            return rows;
        }

        public override async Task<int> deleteSub(SubTable sub)
        {
            int rows = await _database.DeleteAsync(sub);
            return rows;
        }   

        public override async Task<DegreeTable> getDegreeByStdNameAndSubId(string stdName, int subId)
        {
            var degree = await _database.Table<DegreeTable>()
                .FirstOrDefaultAsync(d => d.StdName == stdName && d.SubId == subId);
            return degree;
        }

        public override async Task<List<SubTable>> searchSubTable(string searchText)
        {
            var subjects = await _database.Table<SubTable>()
                .Where(s => s.SubName.Contains(searchText) || s.SubTeacherName.Contains(searchText))
                .ToListAsync();
            return subjects;
        }

        public override async Task<int> updateDegree(DegreeTable degree)
        {
            int rows = await _database.UpdateAsync(degree);
            return rows;
        }
        public override async Task<List<SchedulerTask>> getTaskTableByUserId()
        {
            var allTasks = await _database.Table<SchedulerTask>().Where(t=> t.UserId == UserSession.UserId).ToListAsync();
            return allTasks;
        }

        public override async Task<SchedulerTask> getTaskByID(int taskid)
        {
            var task = await _database.Table<SchedulerTask>().FirstOrDefaultAsync(t => t.TaskId == taskid);
            return task;
        }
        public override async Task<int> insertTask(SchedulerTask Task){
            int rows = await _database.InsertAsync(Task);
            return rows;
        }
        public override async Task<int> updateTask(SchedulerTask Task)
        {
            int rows = await _database.UpdateAsync(Task);
            return rows;
        }
        public override async Task<bool> TaskTimeConflict(DateTime newStartTime, DateTime newEndTime, string id)
        {
            SchedulerTask conflict;
            if (string.IsNullOrEmpty(id))
            {
                conflict = await _database.Table<SchedulerTask>().FirstOrDefaultAsync(t =>
                t.UserId == UserSession.UserId &&(
                    (newStartTime >= t.TaskStartTime && newStartTime < t.TaskEndTime) ||
                    (newEndTime > t.TaskStartTime && newEndTime <= t.TaskEndTime) ||
                    (newStartTime <= t.TaskStartTime && newEndTime >= t.TaskEndTime)
                    ));
            }
            else
            {
                int tid = int.Parse(id);
                conflict = await _database.Table<SchedulerTask>().FirstOrDefaultAsync(t =>
                    t.UserId == UserSession.UserId && t.TaskId != tid && (
                        (newStartTime >= t.TaskStartTime && newStartTime < t.TaskEndTime) ||
                        (newEndTime > t.TaskStartTime && newEndTime <= t.TaskEndTime) ||
                        (newStartTime <= t.TaskStartTime && newEndTime >= t.TaskEndTime)
                    ));
            }

            return conflict != null;
        }

        public override async Task<int> deleteTask(int taskid)
        {
            var task = await getTaskByID(taskid);
            int rows = await _database.DeleteAsync(task);
            return rows;
        }
        /*public  async Task CheckSchedulerExist()
        {
            
                var tableExist = await _database.Table<SchedulerTask>().ToListAsync();

                // If the table is empty or doesn't exist, create the table
                if (tableExist == null || !tableExist.Any())
                {
                    // Create the table if it doesn't exist
                    await _database.CreateTableAsync<SchedulerTask>();
                }
        }*/


    }
}
