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
            try{
                FirebaseResponse response;
                string userid = await SecureStorage.GetAsync("userid");
                if (userid == null)
                { 
                    return null;
                }

                response = await client.GetAsync("User/UserSession/" + userid);
                
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                UserSessionTable LUS = JsonConvert.DeserializeObject<UserSessionTable>(response.Body.ToString());

                UserSessionTable result = LUS;
                    return result; 
                
            } 
            catch (Exception error){
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
                deletedCount += await DeleteBookssBySubId(subId);

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
                try
                {
                    // Attempt to deserialize as Dictionary
                    var degreesDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(degreeData.Body);
                    if (degreesDict != null)
                    {
                        foreach (var item in degreesDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var degreeDelete = await client.DeleteAsync($"degree/{item.Key}");
                                if (degreeDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                        return deletedCount;
                    }
                }
                catch
                {
                    // If deserialization as Dictionary fails, try as List
                    try
                    {
                        var degreesList = JsonConvert.DeserializeObject<List<DegreeTable>>(degreeData.Body);
                        if (degreesList != null)
                        {
                            foreach (var item in degreesList)
                            {

                                if (item != null && item.SubId == subId)
                                {
                                    var degreeDelete = await client.DeleteAsync($"degree/{item.DegId}");
                                    if (degreeDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        deletedCount++;
                                    }
                                }
                            
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during deletion: " + ex.Message);
                    }
                }
            }

            return deletedCount;
        }


        // Delete Posts by SubId (Handles both List and Dictionary)
        private async Task<int> DeletePostsBySubId(int subId)
        {
            int deletedCount = 0;
            int deletedPostCount = 0;

            var postData = await client.GetAsync("post/");
            if (postData != null && postData.Body != "null")
            {
                try
                {
                    // Attempt to deserialize as Dictionary
                    var postsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(postData.Body);
                    if (postsDict != null)
                    {
                        foreach (var item in postsDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                deletedPostCount += await DeleteAssignmentByPostId(item.Value.PostId);
                                var postDelete = await client.DeleteAsync($"post/{item.Value.PostId}");
                                if (postDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                        return deletedCount + deletedPostCount;
                    }
                }
                catch
                {
                    // Fallback to List
                    var postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(postData.Body);
                    foreach (var item in postsList)
                    {
                        if (item != null && item.SubId == subId)
                        {
                            deletedPostCount += await DeleteAssignmentByPostId(item.PostId);
                            var postDelete = await client.DeleteAsync($"post/{item.PostId}");
                            if (postDelete.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                    }
                }
            }

            return deletedCount + deletedPostCount;
        }

        // Delete Assignments by PostId (Handles both List and Dictionary)
        private async Task<int> DeleteAssignmentByPostId(int postId)
        {
            int deletedCount = 0;

            var requestData = await client.GetAsync("assignment/");
            if (requestData != null && requestData.Body != "null")
            {
                try
                {
                    var assignmentsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectAssignments>>(requestData.Body);
                    if (assignmentsDict != null)
                    {
                        foreach (var item in assignmentsDict)
                        {
                            if (item.Value != null && item.Value.PostId == postId)
                            {
                                var requestDelete = await client.DeleteAsync($"assignment/{item.Key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                        return deletedCount;
                    }
                }
                catch
                {
                    var assignmentsList = JsonConvert.DeserializeObject<List<SubjectAssignments>>(requestData.Body);
                    int index = 0;
                    foreach (var item in assignmentsList)
                    {
                        if (item != null && item.PostId == postId)
                        {
                            var requestDelete = await client.DeleteAsync($"assignment/{item.StdId + item.PostId}");
                            if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                        index++;
                    }
                }
            }

            return deletedCount;
        }

        // Delete Requests by SubId (Handles both List and Dictionary)
        private async Task<int> DeleteRequestsBySubId(int subId)
        {
            int deletedCount = 0;

            var requestData = await client.GetAsync("request/");
            if (requestData != null && requestData.Body != "null")
            {
                try
                {
                    var requestsDict = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(requestData.Body);
                    if (requestsDict != null)
                    {
                        foreach (var item in requestsDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var requestDelete = await client.DeleteAsync($"request/{item.Key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                        return deletedCount;
                    }
                }
                catch
                {
                    var requestsList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(requestData.Body);
                    int index = 0;
                    foreach (var item in requestsList)
                    {
                        if (item != null && item.SubId == subId)
                        {
                            var requestDelete = await client.DeleteAsync($"request/{item.ReqId}");
                            if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                        index++;
                    }
                }
            }

            return deletedCount;
        }

        // Delete Books by SubId (Handles both List and Dictionary)
        private async Task<int> DeleteBookssBySubId(int subId)
        {
            int deletedCount = 0;

            var requestData = await client.GetAsync("book/");
            if (requestData != null && requestData.Body != "null")
            {
                try
                {
                    var booksDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectBooks>>(requestData.Body);
                    if (booksDict != null)
                    {
                        foreach (var item in booksDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var bookDelete = await client.DeleteAsync($"book/{item.Key}");
                                if (bookDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                        return deletedCount;
                    }
                }
                catch
                {
                    var booksList = JsonConvert.DeserializeObject<List<SubjectBooks>>(requestData.Body);
                    int index = 0;
                    foreach (var item in booksList)
                    {
                        if (item != null && item.SubId == subId)
                        {
                            var bookDelete = await client.DeleteAsync($"book/{item.BookId}");
                            if (bookDelete.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                        index++;
                    }
                }
            }

            return deletedCount;
        }


        public override async Task<int> deleteSubjectBook(SubjectBooks book)
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
        }
        public override async Task<int> deleteSubjectPost(int postId)
        {
            try
            {
                await DeleteAssignmentByPostId(postId);
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

                List<DegreeTable> result = new List<DegreeTable>();

                // Check if data is in Dictionary format
                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var degreeDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(response.Body.ToString());

                    // Filter results based on the current user session
                    foreach (var item in degreeDict)
                    {
                        if (item.Value != null && item.Value.StdName == UserSession.Name)
                        {
                            result.Add(item.Value);
                        }
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var degreeList = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());

                    // Filter results based on the current user session
                    foreach (var item in degreeList)
                    {
                        if (item != null && item.StdName == UserSession.Name)
                        {
                            result.Add(item);
                        }
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
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

                DegreeTable result = null;

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var degreeDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(response.Body.ToString());
                    result = degreeDict.Values.FirstOrDefault(x => x.StdName == stdName && x.SubId == subId);
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var degreeList = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                    result = degreeList.FirstOrDefault(x => x.StdName == stdName && x.SubId == subId);
                }

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
                    return new List<DegreeTable>();
                }

                List<DegreeTable> result = new List<DegreeTable>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var degreeDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(response.Body.ToString());
                    result = degreeDict.Values.Where(s => s.StdName == UserSession.Name && s.SubId == SubId).ToList();
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var degreeList = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                    result = degreeList.Where(s => s.StdName == UserSession.Name && s.SubId == SubId).ToList();
                }

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

                List<DegreeTable> result = new List<DegreeTable>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var degreeDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(response.Body.ToString());
                    result = degreeDict.Values.Where(s => s.SubId == SubId).ToList();
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var degreeList = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                    result = degreeList.Where(s => s.SubId == SubId).ToList();
                }

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

                List<RequestJoinSubject> result = new List<RequestJoinSubject>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(response.Body.ToString());
                    result = requestDict.Values.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToList();
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var requestList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                    result = requestList.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToList();
                }

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

                List<RequestJoinSubject> result = new List<RequestJoinSubject>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(response.Body.ToString());
                    result = requestDict.Values.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToList();
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var requestList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                    result = requestList.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToList();
                }

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

                List<RequestJoinSubject> result = new List<RequestJoinSubject>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var requestDict = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(response.Body.ToString());
                    result = requestDict.Values.Where(s => s.SubId == subId).ToList();
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var requestList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                    result = requestList.Where(s => s.SubId == subId).ToList();
                }

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

                SubTable result = null;

                // Deserialize directly into a SubTable (not list or dict)
                result = JsonConvert.DeserializeObject<SubTable>(response.Body.ToString());

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

                List<SubTable> result = new List<SubTable>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var subDict = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(response.Body.ToString());
                    result = subDict.Values.Where(s => s != null && s.UserId == UserSession.UserId).ToList();
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var subList = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                    result = subList.Where(s => s != null && s.UserId == UserSession.UserId).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getSubByUser: {ex.Message}");
                return new List<SubTable>();
            }
        }


        public override async Task<bool> getSubjectAssignmentByPostIdAndStdId(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("assignment");
                if (response == null || response.Body == "null")
                {
                    return false; // No assignments found
                }

                bool matchFound = false;

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var assignments = JsonConvert.DeserializeObject<Dictionary<string, SubjectAssignments>>(response.Body.ToString());
                    if (assignments != null)
                    {
                        matchFound = assignments.Values.Any(assignment => assignment != null && assignment.PostId == postId && assignment.StdId == UserSession.UserId);
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var assignmentsList = JsonConvert.DeserializeObject<List<SubjectAssignments>>(response.Body.ToString());
                    if (assignmentsList != null)
                    {
                        matchFound = assignmentsList.Any(assignment => assignment != null && assignment.PostId == postId && assignment.StdId == UserSession.UserId);
                    }
                }

                return matchFound;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getSubjectAssignmentByPostIdAndStdId: {ex.Message}");
                return false;
            }
        }

        public override async Task<List<SubjectAssignments>> getSubjectAssignmentsByPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("assignment");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectAssignments>();
                }

                List<SubjectAssignments> result = new List<SubjectAssignments>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var data = JsonConvert.DeserializeObject<Dictionary<string, SubjectAssignments>>(response.Body.ToString());
                    if (data != null)
                    {
                        result = data.Values.Where(s => s != null && s.PostId == postId).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var dataList = JsonConvert.DeserializeObject<List<SubjectAssignments>>(response.Body.ToString());
                    if (dataList != null)
                    {
                        result = dataList.Where(s => s != null && s.PostId == postId).ToList();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getSubjectAssignmentsByPost: {ex.Message}");
                return new List<SubjectAssignments>();
            }
        }

        public override async Task<List<SubjectBooks>> getSubjectBooksBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("book");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectBooks>();
                }

                List<SubjectBooks> result = new List<SubjectBooks>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var data = JsonConvert.DeserializeObject<Dictionary<string, SubjectBooks>>(response.Body.ToString());
                    if (data != null)
                    {
                        result = data.Values.Where(s => s != null && s.SubId == subId).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var dataList = JsonConvert.DeserializeObject<List<SubjectBooks>>(response.Body.ToString());
                    if (dataList != null)
                    {
                        result = dataList.Where(s => s != null && s.SubId == subId).ToList();
                    }
                }

                return result;
            }
            catch
            {
                return new List<SubjectBooks>();
            }
        }

        public override async Task<SubjectPosts> getSubjectPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post/" + postId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                SubjectPosts result = null;

                // Deserialize as single object (not list or dict)
                result = JsonConvert.DeserializeObject<SubjectPosts>(response.Body.ToString());

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

                List<SubjectPosts> postsList;

                // Auto-detect if data is Array or Object
                if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List if data is in array format
                    postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                }
                else
                {
                    // Deserialize as Dictionary if data is in object format
                    var postsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body.ToString());
                    postsList = postsDict.Values.ToList();
                }

                return postsList.Where(x => x != null).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

                List<SubjectPosts> postsList;

                // Try deserializing as a list
                if (response.Body.Trim().StartsWith("["))
                {
                    postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                }
                else
                {
                    var postsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body.ToString());
                    postsList = postsDict.Values.ToList();
                }

                if (postsList == null) return new List<SubjectPosts>();

                // Filter posts by SubId
                return postsList.Where(e => e != null && e.SubId == subId).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<SubjectPosts>();
            }
        }


        public override async Task<List<SubjectPosts>> getGeneralPosts()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> result = new List<SubjectPosts>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (if response is in key-value format)
                    var posts = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body.ToString());
                    if (posts != null)
                    {
                        result = posts.Values.Where(e => e != null && e.SubId == -1 && e.PostDate < DateTime.Now).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                    if (postsList != null)
                    {
                        result = postsList.Where(e => e != null && e.SubId == -1 && e.PostDate < DateTime.Now).ToList();
                    }
                }

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

                List<SubTable> result = new List<SubTable>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var subTables = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(response.Body.ToString());
                    if (subTables != null)
                    {
                        result = subTables.Values.Where(l => l != null).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List
                    var subTablesList = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                    if (subTablesList != null)
                    {
                        result = subTablesList.Where(l => l != null).ToList();
                    }
                }

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

                UsersAccountTable result = null;

                // Deserialize as single object
                result = JsonConvert.DeserializeObject<UsersAccountTable>(response.Body.ToString());
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

                List<DegreeTable> result = new List<DegreeTable>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var degreeData = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(response.Body.ToString());
                    if (degreeData != null)
                    {
                        result = degreeData.Values.Where(d => d != null).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var degreeList = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                    if (degreeList != null)
                    {
                        result = degreeList.Where(d => d != null).ToList();
                    }
                }

                return result;
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

                List<RequestJoinSubject> result = new List<RequestJoinSubject>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var requestData = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(response.Body.ToString());
                    if (requestData != null)
                    {
                        result = requestData.Values.Where(r => r != null).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var requestList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                    if (requestList != null)
                    {
                        result = requestList.Where(r => r != null).ToList();
                    }
                }

                return result;
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
        public override async Task<int> insertSubjectAssignment(SubjectAssignments assignment)
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
        }
        public override async Task<List<SubjectBooks>> getsubjectBooks()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("book");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectBooks>();
                }

                List<SubjectBooks> result = new List<SubjectBooks>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var bookData = JsonConvert.DeserializeObject<Dictionary<string, SubjectBooks>>(response.Body.ToString());
                    if (bookData != null)
                    {
                        result = bookData.Values.Where(b => b != null).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var bookList = JsonConvert.DeserializeObject<List<SubjectBooks>>(response.Body.ToString());
                    if (bookList != null)
                    {
                        result = bookList.Where(b => b != null).ToList();
                    }
                }

                return result;
            }
            catch
            {
                return new List<SubjectBooks>();
            }
        }

        public override async Task<int> insertSubjectBook(SubjectBooks book)
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
        }
        public override async Task<List<SubjectPosts>> GetSubjectPosts()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> result = new List<SubjectPosts>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var postData = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body.ToString());
                    if (postData != null)
                    {
                        result = postData.Values.Where(p => p != null).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var postList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                    if (postList != null)
                    {
                        result = postList.Where(p => p != null).ToList();
                    }
                }

                return result;
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
                int id;
                List<SubjectPosts> sb = await getSubjectPosts();
                if(sb.Count == 0)
                    id = 1;
                else
                {
                    id = sb.Max(s => s?.PostId ?? 0) + 1;
                }
                subjectPosts.PostId = id;
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
                if (LUS.Password != password)
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
                if (string.IsNullOrEmpty(searchText))
                {
                    return new List<SubTable>();
                }

                FirebaseResponse response = await client.GetAsync("sub");
                if (response == null || response.Body == "null")
                {
                    return new List<SubTable>();
                }

                List<SubTable> result = new List<SubTable>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var subData = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(response.Body.ToString());
                    if (subData != null)
                    {
                        result = subData.Values.Where(s => s != null &&
                            (!string.IsNullOrEmpty(s.SubName) && s.SubName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(s.SubTeacherName) && s.SubTeacherName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var subList = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                    if (subList != null)
                    {
                        result = subList.Where(s => s != null &&
                            (!string.IsNullOrEmpty(s.SubName) && s.SubName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrEmpty(s.SubTeacherName) && s.SubTeacherName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                    }
                }

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
                FirebaseResponse response = await client.GetAsync("User/Account");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                UsersAccountTable result = null;

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var usersDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(response.Body);
                    result = usersDict?.Values.FirstOrDefault(u => u != null && u.Username == username && u.Password == password);
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var usersList = JsonConvert.DeserializeObject<List<UsersAccountTable>>(response.Body);
                    result = usersList?.FirstOrDefault(u => u != null && u.Username == username && u.Password == password);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UserLoginChecker: {ex.Message}");
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

                List<SchedulerTask> result = new List<SchedulerTask>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var taskData = JsonConvert.DeserializeObject<Dictionary<string, SchedulerTask>>(response.Body.ToString());
                    if (taskData != null)
                    {
                        result = taskData.Values.Where(t => t != null).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var taskList = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body.ToString());
                    if (taskList != null)
                    {
                        result = taskList.Where(t => t != null).ToList();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getTaskTable: {ex.Message}");
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

                List<SchedulerTask> result = new List<SchedulerTask>();

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var tasksData = JsonConvert.DeserializeObject<Dictionary<string, SchedulerTask>>(response.Body.ToString());
                    if (tasksData != null)
                    {
                        result = tasksData.Values
                            .Where(task => task != null && task.UserId == UserSession.UserId)
                            .ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var tasksList = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body.ToString());
                    if (tasksList != null)
                    {
                        result = tasksList
                            .Where(task => task != null && task.UserId == UserSession.UserId)
                            .ToList();
                    }
                }

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

                SchedulerTask result = null;

                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var taskData = JsonConvert.DeserializeObject<Dictionary<string, SchedulerTask>>(response.Body.ToString());
                    if (taskData != null && taskData.ContainsKey(taskId.ToString()))
                    {
                        result = taskData[taskId.ToString()];
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var taskList = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body.ToString());
                    if (taskList != null)
                    {
                        result = taskList.FirstOrDefault(t => t.TaskId == taskId);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getTaskByID: {ex.Message}");
                return null;
            }
        }

        public override async Task<int> insertTask(SchedulerTask Task)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("tasks");
                List<SchedulerTask> existingTasks = new List<SchedulerTask>();

                if (response != null && response.Body != "null")
                {
                    // Deserialize as a List instead of Dictionary
                    existingTasks = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body);

                    if (existingTasks != null && existingTasks.Any())
                    {

                        int maxId = existingTasks.Where(t => t != null).Max(t => t.TaskId);
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

                // Initialize a list to hold the tasks
                List<SchedulerTask> userTasks = new List<SchedulerTask>();

                // Check if the response body starts with '{' (dictionary) or '[' (list)
                if (response.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as Dictionary (key-value format)
                    var tasksDict = JsonConvert.DeserializeObject<Dictionary<string, SchedulerTask>>(response.Body.ToString());
                    if (tasksDict != null)
                    {
                        userTasks = tasksDict.Values.Where(t => t != null && t.UserId == UserSession.UserId).ToList();
                    }
                }
                else if (response.Body.Trim().StartsWith("["))
                {
                    // Deserialize as List (array format)
                    var tasksList = JsonConvert.DeserializeObject<List<SchedulerTask>>(response.Body.ToString());
                    if (tasksList != null)
                    {
                        userTasks = tasksList.Where(t => t != null && t.UserId == UserSession.UserId).ToList();
                    }
                }

                // Helper function to check time conflict
                bool IsTimeConflict(SchedulerTask task)
                {
                    return (newStartTime >= task.TaskStartTime && newStartTime < task.TaskEndTime) ||
                           (newEndTime > task.TaskStartTime && newEndTime <= task.TaskEndTime) ||
                           (newStartTime <= task.TaskStartTime && newEndTime >= task.TaskEndTime);
                }

                // Check for time conflict in the filtered user tasks
                SchedulerTask conflict = null;
                if (string.IsNullOrEmpty(id))
                {
                    conflict = userTasks.FirstOrDefault(t => IsTimeConflict(t));
                }
                else
                {
                    int tid = int.Parse(id);
                    conflict = userTasks.FirstOrDefault(t =>
                        t.TaskId != tid && IsTimeConflict(t));
                }

                // Return true if conflict exists, else false
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
