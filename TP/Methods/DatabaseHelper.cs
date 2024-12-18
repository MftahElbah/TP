
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.Methods
{
    public class DatabaseHelper
    {
        public readonly SQLiteAsyncConnection _database;
        // Defines the path to the SQLite database in the local application data directory.

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            // Initializes the SQLiteAsyncConnection with the provided database path.
        }

        public async Task InitializeDatabaseAsync()
        {
            await _database.CreateTableAsync<SubTable>();
            await _database.CreateTableAsync<UsersAccountTable>();
            await _database.CreateTableAsync<RequestJoinSubject>();
            await _database.CreateTableAsync<DegreeTable>();
            await _database.CreateTableAsync<SubjectBooks>();
            await _database.CreateTableAsync<SubjectPosts>();
            await _database.CreateTableAsync<UserSessionTable>();
            await _database.CreateTableAsync<SubjectAssignments>();

            await SeedDatabase(); // Calls the method to seed the database with initial data if needed.

        }
                
        public async Task InitializeAsync()
        {
            await SeedDatabase(); // Calls the SeedDatabase method to populate initial data asynchronously.
        }

        private async Task SeedDatabase()
        {                      
            var teacher = await _database.Table<UsersAccountTable>().ToListAsync();
            if(teacher.Count == 0){
                var initialTeacher = new List<UsersAccountTable>
                {
                    new UsersAccountTable {UserId=111,Name= "test",Username = "t" , Password="1" , UserType=2 },
                    new UsersAccountTable {UserId=123,Name= "stest",Username = "s" , Password="1" , UserType=3 }
                };
                await _database.InsertAllAsync(initialTeacher); // Inserts the initial Teacher Account into the database.
            }
        }                
    }
}
