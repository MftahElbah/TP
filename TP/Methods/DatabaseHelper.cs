using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Net.Http.SslCertificate;


namespace TP.Methods
{
    public class DatabaseHelper
    {
        public readonly SQLiteAsyncConnection _database;
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");
        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<DepTable>().Wait();
            _database.CreateTableAsync<BranchTable>().Wait();
            
            SeedDatabase();

        }

        
        private async void SeedDatabase()

        {
            var departments = await _database.Table<DepTable>().ToListAsync();

            if (departments.Count == 0)

            {

                var initialDepartments = new List<DepTable>

                {

                    new DepTable { DepName = "الاتجاه العام"},
                    new DepTable { DepName = "قسم الادارة"},
                    new DepTable { DepName = "قسم الكهرباء"},
                    new DepTable { DepName = "قسم الحاسوب"},
                    new DepTable { DepName = "قسم الميكانيكا"},
                    new DepTable { DepName = "قسم المعماري"},
                    new DepTable { DepName = "قسم المدني"},

                };


                await _database.InsertAllAsync(initialDepartments);

            }


            var branches = await _database.Table<BranchTable>().ToListAsync();

            if (branches.Count == 0)

            {

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


                await _database.InsertAllAsync(initialBranches);

            }
        }


            public Task<List<DepTable>> GetDepartmentsAsync()
        {
            return _database.Table<DepTable>().ToListAsync();
        }

        public Task<List<BranchTable>> GetBranchesAsync()
        {
            return _database.Table<BranchTable>().ToListAsync();
        }

        public async Task AddDepartmentAsync(string depName)
        {
            if (!string.IsNullOrWhiteSpace(depName))
            {
                var newDepartment = new DepTable { DepName = depName };
                await _database.InsertAsync(newDepartment);
            }
        }

        public async Task UpdateDepartmentAsync(int depId, string depName)
        {
            // Retrieve the existing department by ID
            var department = await _database.Table<DepTable>().FirstOrDefaultAsync(d => d.DepId == depId);

            if (department != null)
            {
                string oldDepName = department.DepName; // Save the old department name

                // Update the department name
                department.DepName = depName;
                await _database.UpdateAsync(department);

                // Update all branches referencing the old department name
                var branchesToUpdate = await _database.Table<BranchTable>().Where(b => b.DepName == oldDepName).ToListAsync();
                foreach (var branch in branchesToUpdate)
                {
                    branch.DepName = depName;
                    await _database.UpdateAsync(branch);
                }
            }
            else
            {
                // Handle case where the department with the given ID doesn't exist
                throw new Exception($"Department with ID {depId} not found.");
            }
        }
        public async Task UpdateBranchAsync(int branchId, string branchName ,string depName)
        {
            // Retrieve the existing department by ID
            var branch = await _database.Table<BranchTable>().FirstOrDefaultAsync(d => d.BranchId == branchId);

            if (branch != null)
            {
                // Update the department name
                branch.BranchName= branchName;
                branch.DepName = depName;

                // Save changes to the database
                await _database.UpdateAsync(branch);
            }
            else
            {
                // Handle case where the department with the given ID doesn't exist
                throw new Exception($"Department with ID {branchId} not found.");
            }
        }



        public async Task DeleteDepartmentAsync(int depId)
        {
            var department = await _database.Table<DepTable>().FirstOrDefaultAsync(d => d.DepId == depId);
            if (department != null)
            {
                string oldDepName = department.DepName;
                var branchesToUpdate = await _database.Table<BranchTable>().Where(b => b.DepName == oldDepName).ToListAsync();
                foreach (var branch in branchesToUpdate)
                {
                    await _database.DeleteAsync(branch);
                }

                await _database.DeleteAsync(department);
            }
            else
            {
                // Handle case where the department with the given ID doesn't exist
                throw new Exception($"Department with ID {depId} not found.");
            }
            
        }

        public async Task DeleteBranchAsync(int branchId)
        {
            var branch = await _database.Table<BranchTable>().FirstOrDefaultAsync(d => d.BranchId == branchId);
            if (branch != null)
            {
                // Save changes to the database
                await _database.DeleteAsync(branch);
            }
            else
            {
                // Handle case where the branch with the given ID doesn't exist
                throw new Exception($"branch with ID {branchId} not found.");
            }
        }

        // Add methods for inserting, updating, and deleting records as needed
    }
}
