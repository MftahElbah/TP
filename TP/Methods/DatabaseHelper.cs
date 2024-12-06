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
            await _database.CreateTableAsync<DepTable>(); // Creates the DepTable if it doesn't exist.
            await _database.CreateTableAsync<BranchTable>(); // Creates the BranchTable if it doesn't exist.
            await _database.CreateTableAsync<StdTable>(); // Creates the StdTable if it doesn't exist.
            await _database.CreateTableAsync<SubTable>();
            await _database.CreateTableAsync<UsersAccountTable>();

            await SeedDatabase(); // Calls the method to seed the database with initial data if needed.

        }
        public async Task<List<SubTableView>> GetSubTableViewAsync()
        {
            try
            {
                string query = @"
            SELECT 
                s.SubId, 
                s.SubName, 
                d.DepName AS DepName, 
                b.BranchName AS BranchName, 
                s.SubClass
            FROM SubTable s
            INNER JOIN DepTable d ON s.SubDep = d.DepId
            INNER JOIN BranchTable b ON s.SubBranch = b.BranchId";

                return await _database.QueryAsync<SubTableView>(query);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"SQLiteException: {ex.Message}");
                return new List<SubTableView>(); // Return empty list on error.
            }
        }



        public async Task InitializeAsync()
        {
            await SeedDatabase(); // Calls the SeedDatabase method to populate initial data asynchronously.
        }

        private async Task SeedDatabase()
        {
            var departments = await _database.Table<DepTable>().ToListAsync(); // Fetches all departments from the database.
            if (departments.Count == 0)
            {
                // If no departments exist, seed the database with initial department data.
                var initialDepartments = new List<DepTable>
                {
                    new DepTable { DepName = "الاتجاه العام" },
                    new DepTable { DepName = "قسم الادارة" },
                    new DepTable { DepName = "قسم الكهرباء" },
                    new DepTable { DepName = "قسم الحاسوب" },
                    new DepTable { DepName = "قسم الميكانيكا" },
                    new DepTable { DepName = "قسم المعماري" },
                    new DepTable { DepName = "قسم المدني" }
                };
                await _database.InsertAllAsync(initialDepartments); // Inserts the initial departments into the database.
            }

            var branches = await _database.Table<BranchTable>().ToListAsync(); // Fetches all branches from the database.
            if (branches.Count == 0)
            {
                // If no branches exist, seed the database with initial branch data.
                var initialBranches = new List<BranchTable>
                {
                    new BranchTable { BranchName = "الاتجاه العام", DepName = "الاتجاه العام" },
                    new BranchTable { BranchName = "الإدارة", DepName = "قسم الادارة" },
                    new BranchTable { BranchName = "المحاسبة", DepName = "قسم الادارة" },
                    new BranchTable { BranchName = "الكهرباء", DepName = "قسم الكهرباء" },
                    new BranchTable { BranchName = "الحاسوب", DepName = "قسم الحاسوب" },
                    new BranchTable { BranchName = "تبرد وتكيف", DepName = "قسم الميكانيكا" },
                    new BranchTable { BranchName = "الميكانيكا", DepName = "قسم الميكانيكا" },
                    new BranchTable { BranchName = "المعماري", DepName = "قسم المعماري" },
                    new BranchTable { BranchName = "المدني", DepName = "قسم المدني" },
                    new BranchTable { BranchName = "تصميم داخلي", DepName = "قسم المدني" }
                };
                await _database.InsertAllAsync(initialBranches); // Inserts the initial branches into the database.
            }
            var teacher = await _database.Table<UsersAccountTable>().ToListAsync();
            if(teacher.Count == 0){
                var initialTeacher = new List<UsersAccountTable>
                {
                    new UsersAccountTable {UserId=1234,Name= "Test",Username = "Test" , Password="123" , UserType=2 }
                };
                await _database.InsertAllAsync(initialTeacher); // Inserts the initial Teacher Account into the database.
                }

        }

        public Task<List<DepTable>> GetDepartmentsAsync()
        {
            return _database.Table<DepTable>().ToListAsync(); // Fetches all departments from the database asynchronously.
        }

        public Task<List<BranchTable>> GetBranchesAsync()
        {
            return _database.Table<BranchTable>().ToListAsync(); // Fetches all branches from the database asynchronously.
        }

        public Task<List<StdTable>> GetStudentsAsync()
        {
            return _database.Table<StdTable>().ToListAsync(); // Fetches all students from the database asynchronously.
        }

        public async Task AddDepartmentAsync(string depName)
        {
            if (!string.IsNullOrWhiteSpace(depName))
            {
                var newDepartment = new DepTable { DepName = depName }; // Creates a new department instance.
                await _database.InsertAsync(newDepartment); // Inserts the new department into the database.
            }
        }

        public async Task UpdateDepartmentAsync(int depId, string depName)
        {
            var department = await _database.Table<DepTable>().FirstOrDefaultAsync(d => d.DepId == depId);
            // Retrieves the department with the specified ID.
            if (department != null)
            {
                string oldDepName = department.DepName; // Saves the old department name before updating.
                department.DepName = depName; // Updates the department name.
                await _database.UpdateAsync(department); // Saves the updated department in the database.

                // Updates any branches associated with the old department name.
                var branchesToUpdate = await _database.Table<BranchTable>().Where(b => b.DepName == oldDepName).ToListAsync();
                foreach (var branch in branchesToUpdate)
                {
                    branch.DepName = depName;
                    await _database.UpdateAsync(branch); // Updates the branches in the database.
                }
                var stdToUpdate = await _database.Table<StdTable>().Where(b => b.StdDep == oldDepName).ToListAsync();
                foreach (var std in stdToUpdate)
                {
                    std.StdDep = depName;
                    await _database.UpdateAsync(std); // Updates the branches in the database.
                }
            }
            else
            {
                throw new Exception($"Department with ID {depId} not found."); // Throws an exception if the department doesn't exist.
            }
        }

        public async Task UpdateBranchAsync(int branchId, string branchName, string depName)
        {
            var branch = await _database.Table<BranchTable>().FirstOrDefaultAsync(b => b.BranchId == branchId);
            // Retrieves the branch with the specified ID.
            if (branch != null)
            {
                string oldBranchName = branch.BranchName;
                var stdToUpdate = await _database.Table<StdTable>().Where(b => b.StdBranch == oldBranchName).ToListAsync();
                foreach (var std in stdToUpdate)
                {
                    std.StdBranch = branchName;
                    std.StdDep = depName;
                    await _database.UpdateAsync(std); // Updates the branches in the database.
                }

                branch.BranchName = branchName; // Updates the branch name.
                branch.DepName = depName; // Updates the department name for the branch.
                await _database.UpdateAsync(branch); // Saves the updated branch in the database.
            }
            else
            {
                throw new Exception($"Branch with ID {branchId} not found."); // Throws an exception if the branch doesn't exist.
            }
        }

        public async Task DeleteDepartmentAsync(int depId)
        {
            var department = await _database.Table<DepTable>().FirstOrDefaultAsync(d => d.DepId == depId);
            // Retrieves the department with the specified ID.
            if (department != null)
            {
                string oldDepName = department.DepName;
                // Deletes all branches associated with the department.
                var branchesToDelete = await _database.Table<BranchTable>().Where(b => b.DepName == oldDepName).ToListAsync();
                foreach (var branch in branchesToDelete)
                {
                    await _database.DeleteAsync(branch); // Deletes the branches from the database.
                }
                var stdToDelete = await _database.Table<StdTable>().Where(b => b.StdDep == oldDepName).ToListAsync();
                foreach (var std in stdToDelete)
                {
                    await _database.DeleteAsync(std); // Deletes the branches from the database.
                }

                await _database.DeleteAsync(department); // Deletes the department from the database.
            }
            else
            {
                throw new Exception($"Department with ID {depId} not found."); // Throws an exception if the department doesn't exist.
            }
        }

        public async Task DeleteBranchAsync(int branchId)
        {

            var branch = await _database.Table<BranchTable>().FirstOrDefaultAsync(b => b.BranchId == branchId);
            // Retrieves the branch with the specified ID.
            if (branch != null)
            {
                var stdToDelete = await _database.Table<StdTable>().Where(b => b.StdBranch == branch.BranchName).ToListAsync();
                foreach (var std in stdToDelete)
                {
                    await _database.DeleteAsync(std); // Deletes the branches from the database.
                }
                await _database.DeleteAsync(branch); // Deletes the branch from the database.
            }
            else
            {
                throw new Exception($"Branch with ID {branchId} not found."); // Throws an exception if the branch doesn't exist.
            }
        }

    }
}
