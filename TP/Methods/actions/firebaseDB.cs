﻿using Android.Net.Wifi.Aware;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Java.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Net.Http.SslCertificate;
using static Java.Util.Jar.Attributes;

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
                FirebaseResponse al = await client.GetAsync("User/UserSession");
                List<UserSessionTable> LUS = JsonConvert.DeserializeObject<List<UserSessionTable>>(al.Body.ToString());
                UserSessionTable result = LUS.FirstOrDefault(x => x.UserId == UserSession.UserId);
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
                FirebaseResponse al = await client.GetAsync("degree");
                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("degree");
                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("degree");
                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("degree");
                List<DegreeTable> LUS = JsonConvert.DeserializeObject<List<DegreeTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("request");
                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("request");
                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("request");
                List<RequestJoinSubject> LUS = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("sub");
                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("sub");
                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("assignment");
                List<SubjectAssignments> LUS = JsonConvert.DeserializeObject<List<SubjectAssignments>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("assignment");
                List<SubjectAssignments> LUS = JsonConvert.DeserializeObject<List<SubjectAssignments>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("Book");
                List<SubjectBooks> LUS = JsonConvert.DeserializeObject<List<SubjectBooks>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("post");
                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("post");
                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("post");
                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("sub");
                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(al.Body.ToString());
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
                FirebaseResponse al = await client.GetAsync("User/Account");
                List<UsersAccountTable> LUS = JsonConvert.DeserializeObject<List<UsersAccountTable>>(al.Body.ToString());
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
                var SetData = await client.SetAsync("degree/" + degreeTable.DegId, degreeTable);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
        public override async Task<int> insertRequestJoin(RequestJoinSubject request)
        {
           try
            {
                var setData = await client.SetAsync("request/" + request.ReqId, request);
                return 1;
            }
            catch { return 0; }
        }
        public override async Task<int> insertSub(SubTable subTable)
        {
            try
            {
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
        public override async Task<int> insertSubjectBook(SubjectBooks book)
        {
            try
            {
                var setData = await client.SetAsync("book/" + book.BookId, book);
                    return 1;
            }
            catch
            {
                return 0;
            }
        }
        public override async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            try
            {
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
                FirebaseResponse al = await client.GetAsync("User/Account");
                List<UsersAccountTable> LUS = JsonConvert.DeserializeObject<List<UsersAccountTable>>(al.Body.ToString());
                UsersAccountTable result = LUS.FirstOrDefault(e => e.Password == password && userid == e.UserId);
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
                FirebaseResponse al = await  client.GetAsync("sub");
                List<SubTable> LUS = JsonConvert.DeserializeObject<List<SubTable>>(al.Body.ToString());
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

                FirebaseResponse al = client.Get("User/Account");
                List<UsersAccountTable> LUS = JsonConvert.DeserializeObject<List<UsersAccountTable>>(al.Body.ToString());
                UsersAccountTable result = LUS.FirstOrDefault(e => e.Username == username && e.Password == password);
                return result;
                */

                //New Code:
                FirebaseResponse response = client.Get("User/Account");
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