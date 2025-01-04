using Android.App;
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
        public override async Task<int> deleteSub(SubTable sub)
        {
            try
            {
                var SetData = await client.DeleteAsync("sub/" + sub.SubId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
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
                    return null;
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                List<DegreeTable> result = LUS.Where(x => x.StdName == UserSession.Name).ToList();
                return result;
            }
            catch
            {
                return null;
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
                    return null;
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                List<DegreeTable> result = LUS.Where(s => s.SubId == SubId && s.StdName == UserSession.Name).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<DegreeTable>> getDegreeTablesBySubId(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("degree");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                List<DegreeTable> result = LUS.Where(s => s.SubId == SubId ).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<RequestJoinSubject>> getRequestJoinBySubIdAndUserId(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                List<RequestJoinSubject> result = LUS.Where(s => s.SubId == SubId ).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<RequestJoinSubject>> getREquestJoinSubjectBySubIdAndUserId(int SubId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                List<RequestJoinSubject> result = LUS.Where(s => s.SubId == SubId && s.UserId == UserSession.UserId).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<RequestJoinSubject>> getRequestJoinSubjectsBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("request");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                List<RequestJoinSubject> result = LUS.Where(s => s.SubId == subId ).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<SubTable> getSubBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("sub");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                if (LUS == null) return null;

                SubTable result = LUS.FirstOrDefault(s => s.SubId == subId);
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
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                if (LUS == null) return new List<SubTable>();


                List<SubTable> result = LUS.Where(s => s.UserId == UserSession.UserId).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<SubjectAssignments> getSubjectASsignmentByPostIdAndStdId(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("assignment");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubjectAssignments> LUS = JsonConvert.DeserializeObject<List<SubjectAssignments>>(response.Body.ToString());
                SubjectAssignments result = LUS.FirstOrDefault(s => s.PostId == postId && s.StdId == UserSession.UserId);
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubjectAssignments>> getSubjectAssignmentsByPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("assignment");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubjectAssignments> LUS = JsonConvert.DeserializeObject<List<SubjectAssignments>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectAssignments>();


                List<SubjectAssignments> result = LUS.Where(s => s.PostId == postId ).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubjectBooks>> getSubjectBooksBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("Book");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubjectBooks> LUS = JsonConvert.DeserializeObject<List<SubjectBooks>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectBooks>();


                List<SubjectBooks> result = LUS.Where(s => s.SubId == subId).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<SubjectPosts> getSubjectPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                SubjectPosts result = LUS.FirstOrDefault(s => s.PostId == postId);
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
                    return null;
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                List<SubjectPosts> result = LUS.ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                List<SubjectPosts> result = LUS.Where(e => e.SubId ==  subId).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<List<SubTable>> getSubTable()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("sub");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                if (LUS == null) return new List<SubTable>();


                List<SubTable> result = LUS.ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<UsersAccountTable> getUserAccountById(int userId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("User/Account");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<UsersAccountTable> LUS = JsonConvert.DeserializeObject<List<UsersAccountTable>>(response.Body.ToString());
                UsersAccountTable result = LUS.FirstOrDefault(e => e.UserId == userId);
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
                    return null;
                }

                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(response.Body.ToString());
                if (LUS == null) return new List<DegreeTable>();


                return LUS.ToList();

            }
            catch
            {
                return null;
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
                    return null;
                }

                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(response.Body.ToString());
                if (LUS == null) return new List<RequestJoinSubject>();


                return LUS.ToList();

            }
            catch
            {
                return null;
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
                    return null;
                }

                List<SubjectBooks> LUS = JsonConvert.DeserializeObject<List<SubjectBooks>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectBooks>();


                return LUS.ToList();

            }
            catch
            {
                return null;
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
                    return null;
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                return LUS.ToList();

            }
            catch
            {
                return null;
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
                FirebaseResponse response = await  client.GetAsync("sub");
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(response.Body.ToString());
                if (LUS == null) return new List<SubTable>();


                List<SubTable> result = LUS.Where(s => s.SubName.Contains(searchText) || s.SubTeacherName.Contains(searchText)).ToList();
                return result;

            }
            catch
            {
                return null;
            }
        }
        public override async Task<UsersAccountTable> UserLoginChecker(string username, string password)
        {
            
            try
            {
                /*
                Old Code:

                FirebaseResponse response = client.Get("User/Account");
                List<UsersAccountTable> LUS = JsonConvert.DeserializeObject<List<UsersAccountTable>>(response.Body.ToString());
                UsersAccountTable result = LUS.FirstOrDefault(e => e.Username == username && e.Password == password);
                return result;
                */

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

    }
}
