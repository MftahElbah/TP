using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;

namespace TP.Methods.actions
{
    internal class firebaseDB : Database
    {
        public IFirebaseConfig fc = new FirebaseConfig
        {
            AuthSecret = "7gviqqKuDYSHOM6kjHznZjS5u1VTgjAx3D1uq7X9",
            BasePath = "https://ctsapp-9de50-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        public IFirebaseClient client;
        //Code to warn console if class cannot connect when called.
        public void connection()
        {
            try
            {
                client = new FireSharp.FirebaseClient(fc);
            }
            catch (Exception)
            {
                Console.WriteLine("some ");
            }
        }

        public firebaseDB() {
            connection();
        }

        public override void DatabaseStarted()
        {
            connection();
        }


        // start of firebase work

        public override async Task<int> insertSession(UserSessionTable session)
        {
            try
            {
                UserSessionTable set = new UserSessionTable()
                {
                    UserId = session.UserId,
                    Password = session.Password
                };
                var SetData = await client.SetAsync("User/UserSession/" + set.UserId, set);
                await SecureStorage.SetAsync("userid", set.UserId.ToString());
                return 1;
            } 
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public override async Task<int> deleteSession()
        {
            try
            {
                SecureStorage.Remove("userid");
                var SetData = await client.DeleteAsync("User/UserSession/" + UserSession.UserId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public override async Task<UserSessionTable> UserSessionChecker()
        {
            try
            {
                FirebaseResponse response;
                string userid = await SecureStorage.GetAsync("userid");
                if (userid != null)
                { 
                    response = await client.GetAsync("User/UserSession/" + userid);
                }
                else
                {
                    return null;
                }
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                UserSessionTable LUS = JsonConvert.DeserializeObject<UserSessionTable>(response.Body.ToString());

                UserSessionTable result = LUS;
                    return result; 
                
            } catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }
        public override async Task<int> deleteDegree(DegreeTable degreeTable)
        {
            try
            {
                var SetData = await client.DeleteAsync("degree/" + degreeTable.DegId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public override async Task<int> deletePost(SubjectPosts post)
        {
            try
            {
                var SetData = await client.DeleteAsync("post/" + post.PostId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<int> deleteRequestJoin(int requestId)
        {
            try
            {
                var SetData = await client.DeleteAsync("request/" + requestId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<int> deleteSub(int subId)
        {
            try
            {
                // Delete the subject from "sub/"
                var subDeleteResponse = await client.DeleteAsync($"sub/{subId}");
                if (subDeleteResponse == null || subDeleteResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return 0; // Failed to delete the subject
                }

                int deletedCount = 1; // Count the deleted subject

                // Call separate methods to delete related data
                deletedCount += await DeleteDegreesBySubId(subId);
                deletedCount += await DeletePostsBySubId(subId);
                deletedCount += await DeleteRequestsBySubId(subId);

                return deletedCount; // Return total number of deleted records
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting subject and related data: {ex.Message}");
                return 0;
            }
        }

        private async Task<int> DeleteDegreesBySubId(int subId)
        {
            int deletedCount = 0;

            var degreeData = await client.GetAsync("degree/");
            if (degreeData != null && degreeData.Body != "null")
            {
                var degrees = JsonConvert.DeserializeObject<List<DegreeTable>>(degreeData.Body);
                foreach (var item in degrees)
                {
                    if (item.SubId == subId)
                    {
                        var degreeDelete = await client.DeleteAsync($"degree/{item.DegId}");
                        if (degreeDelete.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            deletedCount++;
                        }
                    }
                }
            }

            return deletedCount;
        }

        private async Task<int> DeletePostsBySubId(int subId)
        {
            int deletedCount = 0;

            var postData = await client.GetAsync("post/");
            if (postData != null && postData.Body != "null")
            {
                var posts = JsonConvert.DeserializeObject<List<SubjectPosts>>(postData.Body);
                foreach (var item in posts)
                {
                    if (item.SubId == subId)
                    {
                        var postDelete = await client.DeleteAsync($"post/{item.PostId}");
                        if (postDelete.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            deletedCount++;
                        }
                    }
                }
            }

            return deletedCount;
        }

        private async Task<int> DeleteRequestsBySubId(int subId)
        {
            int deletedCount = 0;

            var requestData = await client.GetAsync("request/");
            if (requestData != null && requestData.Body != "null")
            {
                var requests = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(requestData.Body);
                foreach (var item in requests)
                {
                    if(item != null)
                    {
                        if (item.SubId == subId)
                        {
                            var requestDelete = await client.DeleteAsync($"request/{item.ReqId}");
                            if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                    }
                }
            }

            return deletedCount;
        }
        /*public override async Task<int> deleteSubjectBook(SubjectBooks book)
        {
            try
            {
                var SetData = await client.DeleteAsync("Book/" + book.BookId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }*/
        public override async Task<int> deleteSubjectPost(int postId)
        {
            try
            {
                var SetData = await client.DeleteAsync("post/" + postId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<List<DegreeTable>> getDegreeBySessionName()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("degree");
                if (response == null || response.Body == "null")
                {
                    return new List<DegreeTable>();
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                List<DegreeTable> result = LUS.Where(x => x.StdName == UserSession.Name).ToList();
                return result;
            }
            catch
            {
                return new List<DegreeTable>();
            }
        }
        public override async Task<DegreeTable> getDegreeByStdNameAndSubId(string stdName, int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("degree");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return null;

                DegreeTable result = LUS.FirstOrDefault(x => x.StdName == stdName && x.SubId == subId);
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<DegreeTable>> getDegreeTableBySubIdAndStdName(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("degree");
                if (response == null || response.Body == "null")
                {
                    return new List<DegreeTable>() ;
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                List<DegreeTable> result = LUS.Where(s => s.SubId == SubId && s.StdName == UserSession.Name).ToList();
                return result;
            }
            catch
            {
                return new List<DegreeTable>();
            }
        }
        public override async Task<List<DegreeTable>> getDegreeTablesBySubId(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("degree");
                if (response == null || response.Body == "null")
                {
                    return new List<DegreeTable>();
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                List<DegreeTable> result = LUS.Where(s => s.SubId == SubId ).ToList();
                return result;
            }
            catch
            {
                return new List<DegreeTable>();
            }
        }
        public override async Task<List<RequestJoinSubject>> getRequestJoinBySubIdAndUserId(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return new List<RequestJoinSubject>();
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                List<RequestJoinSubject> result = LUS.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId ).ToList();
                return result;
            }
            catch
            {
                return new List<RequestJoinSubject>();
            }
        }
        public override async Task<List<RequestJoinSubject>> getREquestJoinSubjectBySubIdAndUserId(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return new List<RequestJoinSubject>();
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                List<RequestJoinSubject> result = LUS.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToList();
                return result;
            }
            catch
            {
                return new List<RequestJoinSubject>();
            }
        }
        public override async Task<List<RequestJoinSubject>> getRequestJoinSubjectsBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return new List<RequestJoinSubject>();
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                List<RequestJoinSubject> result = LUS.Where(s => s.SubId == subId ).ToList();
                return result;
            }
            catch
            {
                return new List<RequestJoinSubject>();
            }
        }
        public override async Task<SubTable> getSubBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("sub/" + subId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                SubTable LUS = JsonConvert.DeserializeObject<SubTable>(response.Body.ToString());
                if (LUS == null) return null;

                SubTable result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubTable>> getSubByUser()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("sub");
                if (response == null || string.IsNullOrWhiteSpace(response.Body) || response.Body == "null")
                {
                    return new List<SubTable>();
                }

                List<SubTable> LUS;
                try
                {
                    LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during deserialization: {ex.Message}");
                    return new List<SubTable>();
                }

                if (LUS == null)
                    return new List<SubTable>();

                List<SubTable> result = LUS
                    .Where(s => s != null && s.UserId == UserSession.UserId)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getSubByUser: {ex.Message}");
                return new List<SubTable>();
            }
        }

        /*public override async Task<SubjectAssignments> getSubjectASsignmentByPostIdAndStdId(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("assignment/" + postId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                SubjectAssignments LUS = JsonConvert.DeserializeObject<SubjectAssignments>(response.Body.ToString());
                SubjectAssignments result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }*/
        /*public override async Task<List<SubjectAssignments>> getSubjectAssignmentsByPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("assignment");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectAssignments>();
                }

                List<SubjectAssignments> LUS = JsonConvert.DeserializeObject<List<SubjectAssignments>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectAssignments>();


                List<SubjectAssignments> result = LUS.Where(s => s.PostId == postId ).ToList();
                return result;
            }
            catch
            {
                return new List<SubjectAssignments>();
            }
        }*/
        /*public override async Task<List<SubjectBooks>> getSubjectBooksBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("Book");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectBooks>();
                }

                List<SubjectBooks> LUS = JsonConvert.DeserializeObject<List<SubjectBooks>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectBooks>();


                List<SubjectBooks> result = LUS.Where(s => s.SubId == subId).ToList();
                return result;
            }
            catch
            {
                return new List<SubjectBooks>();
            }
        }*/
        public override async Task<SubjectPosts> getSubjectPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post/" + postId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                SubjectPosts LUS = JsonConvert.DeserializeObject<SubjectPosts>(response.Body.ToString());
                SubjectPosts result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubjectPosts>> getSubjectPosts()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                List<SubjectPosts> result = LUS.ToList();
                return result;
            }
            catch
            {
                return new List<SubjectPosts>();
            }
        }
        public override async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                List<SubjectPosts> result = LUS.Where(e => e.SubId ==  subId).ToList();
                return result;
            }
            catch
            {
                return new List<SubjectPosts>();
            }
        }
        public override async Task<List<SubTable>> getSubTable()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("sub");
                if (response == null || response.Body == "null")
                {
                    return new List<SubTable>();
                }

                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                if (LUS == null) return new List<SubTable>();


                List<SubTable> result = LUS.ToList();
                return result;
            }
            catch
            {
                return new List<SubTable>();
            }
        }
        public override async Task<UsersAccountTable> getUserAccountById(int userId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("User/Account/" + userId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                UsersAccountTable LUS = JsonConvert.DeserializeObject<UsersAccountTable>(response.Body.ToString());
                UsersAccountTable result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<int> insertDegree(DegreeTable degreeTable)
        {
            try
            {
                List<DegreeTable> dg = await getDegree();
                
                degreeTable.DegId = dg.Count;
                var SetData = await client.SetAsync("degree/" + degreeTable.DegId, degreeTable);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<List<DegreeTable>> getDegree()
        {

            try
            {
                FirebaseResponse response = await client.GetAsync("degree");
                if (response == null || response.Body == "null")
                {
                    return new List<DegreeTable>();
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                return LUS.ToList();

            }
            catch
            {
                return new List<DegreeTable>();
            }
        }

        public override async Task<int> insertRequestJoin(RequestJoinSubject request)
        {
           try
            {
                List<RequestJoinSubject> rj = await GetRequests();
                request.ReqId = rj.Count ;
                var setData = await client.SetAsync("request/" + request.ReqId, request);
                return 1;
            }
            catch { return 0; }
        }
        public override async Task<List<RequestJoinSubject>> GetRequests()
        {

            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return new List<RequestJoinSubject>();
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                return LUS.ToList();

            }
            catch
            {
                return new List<RequestJoinSubject>();
            }
        }
        public override async Task<int> insertSub(SubTable subTable)
        {
            try
            {
                List<SubTable> sbt = await getSubTable();
                subTable.SubId = sbt.Count ;
                var setData = await client.SetAsync("sub/" + subTable.SubId, subTable);
                return 1;
            }catch { return 0; }
        }
        /*public override async Task<int> insertSubjectAssignment(SubjectAssignments assignment)
        {
            try
            {

                var setData = await client.SetAsync("assignment/" + assignment.StdId.ToString() + assignment.PostId.ToString(), assignment);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }*/
        /*public override async Task<List<SubjectBooks>> getsubjectBooks()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("book");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectBooks>();
                }

                List<SubjectBooks> LUS = JsonConvert.DeserializeObject<List<SubjectBooks>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectBooks>();


                return LUS.ToList();

            }
            catch
            {
                return new List<SubjectBooks>();
            }
        }*/
        /*public override async Task<int> insertSubjectBook(SubjectBooks book)
        {
            try
            {
                List<SubjectBooks> sb = await getsubjectBooks();
                book.BookId = sb.Count ;
                var setData = await client.SetAsync("book/" + book.BookId, book);
                    return 1;
            }
            catch
            {
                return 0;
            }
        }*/
        public override async Task<List<SubjectPosts>> GetSubjectPosts()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                return LUS.ToList();

            }
            catch
            {
                return new List<SubjectPosts>();
            }
        }
        public override async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            try
            {
                List<SubjectPosts> sb = await getSubjectPosts();
                subjectPosts.PostId = sb.Count ;
                var setData = await client.SetAsync("post/" + subjectPosts.PostId, subjectPosts);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public override async Task<UsersAccountTable> loginSecction(string password, int userid)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("User/Account/" + userid);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                UsersAccountTable LUS = JsonConvert.DeserializeObject<UsersAccountTable>(response.Body.ToString());
                if ( LUS.Password != password)
                {
                    return null;
                }
                UsersAccountTable result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubTable>> searchSubTable(string searchText)
        {
            try
            {
                // Ensure searchText is non-null and non-empty
                if (string.IsNullOrEmpty(searchText))
                {
                    Console.WriteLine("Search text is null or empty.");
                    return new List<SubTable>();
                }

                // Fetch data from Firebase
                FirebaseResponse response = await client.GetAsync("sub");
                if (response == null || response.Body == "null")
                {
                    Console.WriteLine("Firebase response is null or empty.");
                    return new List<SubTable>();
                }

                // Log raw response for debugging
                Console.WriteLine($"Raw Firebase response: {response.Body}");

                // Deserialize the response into a list of SubTable objects
                var LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                if (LUS == null || !LUS.Any())
                {
                    Console.WriteLine("Deserialized data is null or empty.");
                    return new List<SubTable>();
                }

                // Log the number of entries fetched
                Console.WriteLine($"Fetched {LUS.Count} subjects from Firebase.");

                // Perform the search with case-insensitive comparison
                var result = LUS
                             .Where(s =>
                                 s != null && // Ensure the subject object is not null
                                 (
                                     (!string.IsNullOrEmpty(s.SubName) && s.SubName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(s.SubTeacherName) && s.SubTeacherName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                                 )
                             )
                             .ToList();

                // Log the number of results found
                Console.WriteLine($"Found {result.Count} matching subjects for search text: {searchText}");

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in searchSubTable: {ex.Message}");
                return new List<SubTable>();
            }
        }



        public override async Task<UsersAccountTable> UserLoginChecker(string username, string password)
        {
            
            try
            {
                //New Code:
                FirebaseResponse response = await client.GetAsync("User/Account");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                var usersDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(response.Body);
                var user = usersDict?.Values.FirstOrDefault(u => u.Username == username && u.Password == password);
                return user;
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }
        public override async Task<int> updateDegree(DegreeTable degreeTable)
        {
            try
            {
                var SetData = await client.UpdateAsync("degree/" + degreeTable.DegId, degreeTable);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<int> updateSubBySubId(SubTable sub)
        {
            try
            {
                var SetData = await client.UpdateAsync("sub/" + sub.SubId, sub);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<int> updateSubjectPost(SubjectPosts subjectPosts)
        {
            try
            {
                var SetData = await client.UpdateAsync("post/" + subjectPosts.PostId, subjectPosts);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public override async Task<List<SchedulerTask>> getTaskTable()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("tasks");
                if (response == null || response.Body == "null")
                {
                    return new List<SchedulerTask>();
                }

                List<SchedulerTask> LUS = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body.ToString());
                if (LUS == null) return new List<SchedulerTask>();

                return LUS.ToList();
                
            }
            catch
            {
                return new List<SchedulerTask>();
            }
        }
        public override async Task<List<SchedulerTask>> getTaskTableByUserId()
        {
            try
            {
                // Fetch tasks from Firebase
                FirebaseResponse response = await client.GetAsync("tasks");
                if (response == null || response.Body == "null")
                {
                    return new List<SchedulerTask>();
                }

                // Deserialize response to a list while ignoring null values
                List<SchedulerTask> tasksList = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body)?
                    .Where(task => task != null) // Remove null entries
                    .ToList();

                if (tasksList == null || tasksList.Count == 0)
                {
                    return new List<SchedulerTask>();
                }

                // Filter tasks by the current user
                List<SchedulerTask> result = tasksList
                    .Where(s => s.UserId == UserSession.UserId)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getTaskTableByUserId: {ex.Message}");
                return new List<SchedulerTask>();
            }
        }
        public override async Task<SchedulerTask> getTaskByID(int taskId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("tasks/" + taskId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                SchedulerTask LUS = JsonConvert.DeserializeObject<SchedulerTask>(response.Body.ToString());
                SchedulerTask result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<int> insertTask(SchedulerTask Task)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("tasks");
                if (response != null && response.Body != "null")
                {
                    var existingTasks = JsonConvert.DeserializeObject<Dictionary<string, SchedulerTask>>(response.Body);
                    if (existingTasks != null && existingTasks.Any())
                    {
                        int maxId = existingTasks.Values.Max(t => t.TaskId);
                        Task.TaskId = maxId + 1;
                    }
                    else
                    {
                        Task.TaskId = 1; // First TaskId
                    }
                }
                else
                {
                    Task.TaskId = 1; // First TaskId if no tasks exist
                }

                // Insert the new task
                await client.SetAsync($"tasks/{Task.TaskId}", Task);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in insertTask: {e.Message}");
                return 0;
            }
        } 
        public override async Task<int> updateTask(SchedulerTask Task) {
            try
            {
                var SetData = await client.UpdateAsync("tasks/" + Task.TaskId, Task);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<int> deleteTask(int taskid) {
            try
            {
                var SetData = await client.DeleteAsync("tasks/" + taskid);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<bool> TaskTimeConflict(DateTime newStartTime, DateTime newEndTime, string id)
        {
            try
            {
                // Fetch all tasks
                FirebaseResponse response = await client.GetAsync("tasks");
                if (response == null || response.Body == "null")
                {
                    return false;
                }

                // Deserialize tasks
                List<SchedulerTask> tasks = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body);
                List<SchedulerTask> userTasks = tasks.Where(t => t.UserId == UserSession.UserId).ToList();

                SchedulerTask conflict;
                if (string.IsNullOrEmpty(id))
                {
                    conflict = userTasks.FirstOrDefault(t =>
                        (newStartTime >= t.TaskStartTime && newStartTime < t.TaskEndTime) ||
                        (newEndTime > t.TaskStartTime && newEndTime <= t.TaskEndTime) ||
                        (newStartTime <= t.TaskStartTime && newEndTime >= t.TaskEndTime)
                    );
                }
                else
                {
                    int tid = int.Parse(id);
                    conflict = userTasks.FirstOrDefault(t =>
                        t.TaskId != tid && (
                            (newStartTime >= t.TaskStartTime && newStartTime < t.TaskEndTime) ||
                            (newEndTime > t.TaskStartTime && newEndTime <= t.TaskEndTime) ||
                            (newStartTime <= t.TaskStartTime && newEndTime >= t.TaskEndTime)
                        ));
                }

                return conflict != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in TaskTimeConflict: {ex.Message}");
                return false;
            }
        }

    }
}
