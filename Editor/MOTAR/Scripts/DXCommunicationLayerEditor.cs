using System;
using Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using System.Text;

using System.Collections.Generic;



using UnityEditor.UIElements;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;


using System.Net.Http;

namespace DXCommunications
{
    public enum SortOrder { Name, Vendor, Earliest, Latest };
    public enum FormatFilter { All, FBX, Gltf, OBJ };
     
    public enum PolyFilter { All,Tiny,Small,Medium,Large,Enormous, Gigantic};
    public enum TextureFilter { All,Yes, No};
    public enum AnimationFilter { All, Yes, No};
    public enum SupportedEngineFilter { All, Yes, No };

    public class DXCommunicationLayerEditor : ScriptableObject

    {
       
        
        private static List<DXDeveloperCompany> companyList = new List<DXDeveloperCompany>();
        public static JArray appList = new JArray();
        public static JArray endUserList = new JArray();
        public static Dictionary<string, Texture2D> appIcons;
        public static Dictionary<string, Texture2D> SandboxUserProfileImages;
        public static string DevAccessToken;
        public static string DevRefreshToken;
        public static string DevSandboxAccessToken;
        public static string DevSandboxRefreshToken;
        public static DXAppLessonData thisApp;
        public static List<DXCourse> thisAppCourses;
        public static GetAllClassesResponse thisAppClasses;
        public static GetAllLessonsResponse thisAppLessons;
        public static List<DXEvent> thisAppEvents;

        //bCorrect, questions[questionIndex].question, questions[questionIndex].correctAnswerIndex, attemptedClassId, attemptedLessonId

        public static IEnumerator MOTARDeveloperAuthenticationAndSetupFromMOTARDeveloperID(string handle, string password,Action<bool> completion,
                int? page = null,
                int? limit = null)
        {

          
      
            var body = new Dictionary<string, object>
            {
                ["handle"] = handle,
                ["password"] = password
            };
            string json = JsonConvert.SerializeObject(body);


            var data = Encoding.UTF8.GetBytes(json);

            // var encoded = Encoding.UTF8.GetString(data);

            //string url = "https://api.motar.io/sdk/v1/auth/login";
            string url = DXUrl.endpoint("sdk/v1/auth/login", DXEnvironment.Production);
           
            using (UnityWebRequest webRequest = new UnityWebRequest(url))
            {
                webRequest.method = UnityWebRequest.kHttpVerbPOST;

                
                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.uploadHandler.contentType = "application/json";
                // webRequest.uploadHandler.contentType = "application/json";
                //webRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        completion(false);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        completion(false);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        var headers = webRequest.GetResponseHeaders();
                        headers.TryGetValue("accesstoken", out string accessToken);
                        headers.TryGetValue("refreshtoken", out string refreshToken);
                        DevAccessToken = accessToken;
                        DevRefreshToken = refreshToken;
                        yield return MOTARDeveloperEnumerateCompanies();
                        if (completion != null)
                            completion(true);
                        
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        completion(false);
                        break;
                }
            }

            

        }
        public static IEnumerator MOTARDeveloperRefreshDeveloperSession()
        {



            var body = new Dictionary<string, object>
            {
                ["refreshToken"] = DevRefreshToken
                
            };
            string json = JsonConvert.SerializeObject(body);


            var data = Encoding.UTF8.GetBytes(json);

            // var encoded = Encoding.UTF8.GetString(data);

            //string url = "https://api.motar.io/sdk/v1/auth/refresh";
            string url = DXUrl.endpoint("sdk/v1/auth/refresh",DXEnvironment.Production);
            DevAccessToken = "";
            DevRefreshToken = "";

            using (UnityWebRequest webRequest = new UnityWebRequest(url))
            {
                webRequest.method = UnityWebRequest.kHttpVerbPOST;


                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.uploadHandler.contentType = "application/json";
                // webRequest.uploadHandler.contentType = "application/json";
                //webRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);

                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        var headers = webRequest.GetResponseHeaders();
                        headers.TryGetValue("accesstoken", out string accessToken);
                        headers.TryGetValue("refreshtoken", out string refreshToken);
                        DevAccessToken = accessToken;
                        DevRefreshToken = refreshToken;
                        
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        
                        break;
                }
            }



        }

        public static IEnumerator MOTARDeveloperHubListings()
        {
            //string url = "https://api.motar.io/sdk/v1/listing/list";
            string url =   DXUrl.endpoint("sdk/v1/listing/list",DXEnvironment.Development);
            //string searchURL = "https://api.motar.io/sdk/v1/listing/search/model";
            using (UnityWebRequest webRequest = new UnityWebRequest(url))
            {
                webRequest.method = UnityWebRequest.kHttpVerbGET;
                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);

                // webRequest.uploadHandler = new UploadHandlerRaw(data);
                //  webRequest.uploadHandler.contentType = "application/json";
                // webRequest.uploadHandler.contentType = "application/json";
                //webRequest.uploadHandler.contentType = "application/x-www-form-urlencoded";
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        //completion(false);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                      //  completion(false);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                      // completion(false);
                        break;
                }
            }

        }

        public static IEnumerator MOTARDeveloperSandboxUserImpersonation(string userId, string clientId,string appId, Action<bool> completion,
                int? page = null,
                int? limit = null)
        {

            Debug.LogWarning("Impersonationg userid:" + userId + " and app:" + appId);
            var body = new Dictionary<string, object>
            {
                ["userId"] = userId,
                ["appId"] = appId


            };
            string json = JsonConvert.SerializeObject(body);

            var data = Encoding.UTF8.GetBytes(json);

            //DXConfiguration dXConfiguration = (DXConfiguration)CreateInstance(typeof(DXConfiguration));
            //string url = "https://api.motar.io/sdk/v1/auth/login";
            //string url = "https://api.motar.io/sdk/v1/auth/sandbox/login";
            string url = DXUrl.endpoint("sdk/v1/auth/sandbox/login",DXEnvironment.Production);

            using (UnityWebRequest webRequest = new UnityWebRequest(url))
            {
                webRequest.method = UnityWebRequest.kHttpVerbPOST;
                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.uploadHandler.contentType = "application/json";
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                Debug.LogWarning("ACCESS TOKEN WAS:" + DevAccessToken);
                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        if (webRequest.downloadHandler.text.ToLower().Contains("refresh"))
                        {
                            

                            yield return MOTARDeveloperRefreshDeveloperSession();
                            if(DevAccessToken != "")
                                EditorCoroutineUtility.StartCoroutine(DXCommunicationLayerEditor.MOTARDeveloperSandboxUserImpersonation(userId, clientId, appId, completion), MOTARSetupWindow.myWindow);
                            
                        }
                        else
                            completion(false);
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        completion(false);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        var headers = webRequest.GetResponseHeaders();
                        //headers.TryGetValue("access_token", out string accessToken);
                        //headers.TryGetValue("refresh_token", out string refreshToken);
                        json = webRequest.downloadHandler.text;

                        var Impersonated = (JObject)JsonConvert.DeserializeObject(json);
                        string returnedUserID = (string)Impersonated["userid"];
                        
                        
                        DevSandboxAccessToken = (string)Impersonated["accessToken"];
                        
                        DevSandboxRefreshToken = (string)Impersonated["refreshToken"];
                        yield return MOTARDeveloperClassInfoFromLoggedOnSandboxUser();
                        if (completion != null)
                            completion(true);
                        
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        completion(false);
                        break;
                }
            }



        }
        public static IEnumerator MOTARDeveloperClearImpersonatedStudentClassData(string userId)
        {

            Debug.LogWarning("clearing data for "+userId);
            var body = new Dictionary<string, object>
            {
                ["userId"] = userId
              


            };
            string json = JsonConvert.SerializeObject(body);

            var data = Encoding.UTF8.GetBytes(json);

            //DXConfiguration dXConfiguration = (DXConfiguration)CreateInstance(typeof(DXConfiguration));
            //string url = "https://api.motar.io/sdk/v1//login";
            //string url = "https://api.motar.io/sdk/v1/training/clear-scores";
            string url = DXUrl.endpoint("sdk/v1/training/clear-scores",DXEnvironment.Production);
            using (UnityWebRequest webRequest = new UnityWebRequest(url))
            {
                webRequest.method = UnityWebRequest.kHttpVerbPOST;
                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.uploadHandler.contentType = "application/json";
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                Debug.LogWarning("ACCESS TOKEN WAS:" + DevAccessToken);
                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);

                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:

                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        var headers = webRequest.GetResponseHeaders();
                        //headers.TryGetValue("access_token", out string accessToken);
                        //headers.TryGetValue("refresh_token", out string refreshToken);
                       

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        //completion(false);
                        break;
                }
            }



        }
        //public static IEnumerator MOTARDeveloperAuthenticationAndSetupFromSandboxUser(string handle, string password, Action<bool> completion,
        //        int? page = null,
        //        int? limit = null)
        //{

        //    var body = new Dictionary<string, object>
        //    {
        //        ["userId"] = handle,
        //        ["password"] = password
        //    };
        //    string json = JsonConvert.SerializeObject(body);

        //    var data = Encoding.UTF8.GetBytes(json);

        //    DXConfiguration dXConfiguration = (DXConfiguration)CreateInstance(typeof(DXConfiguration));
        //    //string url = "https://api.motar.io/sdk/v1/auth/login";
        //    string url = DXUrl.endpoint("auth/basic");

        //    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        //    {
        //        webRequest.method = UnityWebRequest.kHttpVerbPOST;
        //        webRequest.uploadHandler = new UploadHandlerRaw(data);
        //        webRequest.uploadHandler.contentType = "application/json";
        //        webRequest.downloadHandler = new DownloadHandlerBuffer();

        //        // Request and wait for the desired page.
        //        yield return webRequest.SendWebRequest();


        //        switch (webRequest.result)
        //        {
        //            case UnityWebRequest.Result.ConnectionError:
        //            case UnityWebRequest.Result.DataProcessingError:
        //                Debug.LogError(": Error: " + webRequest.error);
        //                completion(false);
        //                break;
        //            case UnityWebRequest.Result.ProtocolError:
        //                Debug.LogError(": HTTP Error: " + webRequest.error);
        //                completion(false);
        //                break;
        //            case UnityWebRequest.Result.Success:
        //                Debug.Log("received login RESPONSE at " + System.DateTime.Now);
        //                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
        //                var headers = webRequest.GetResponseHeaders();
        //                headers.TryGetValue("access_token", out string accessToken);
        //                headers.TryGetValue("refresh_token", out string refreshToken);
        //                DXCommunicationLayerEditorAccessToken = accessToken;
        //                DevSandboxAccessToken = accessToken;
        //                DXCommunicationLayerEditorRefreshToken = refreshToken;
        //                DevSandboxRefreshToken = refreshToken;
        //                yield return MOTARDeveloperClassInfoFromLoggedOnSandboxUser();
        //                if (completion != null)
        //                    completion(true);
        //                break;

        //            default:
        //                Exception error = null;
        //                error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
        //                completion(false);
        //                break;
        //        }
        //    }



        //}
        //public static IEnumerator MOTARDeveloperClassAndCourseInfoFromLoggedOnTestUser()
        //{


        //    using (UnityWebRequest webRequest = UnityWebRequest.Get(DXUrl.endpoint("edu/v1/class/list")))
        //    {


        //        webRequest.SetRequestHeader("Authorization", "Bearer " + DXCommunicationLayerEditorAccessToken);
        //        // Request and wait for the desired page.
        //        yield return webRequest.SendWebRequest();


        //        switch (webRequest.result)
        //        {
        //            case UnityWebRequest.Result.ConnectionError:
        //            case UnityWebRequest.Result.DataProcessingError:
        //                Debug.LogError(": Error: " + webRequest.error);
        //                break;
        //            case UnityWebRequest.Result.ProtocolError:
        //                Debug.LogError(": HTTP Error: " + webRequest.error);
        //                break;
        //            case UnityWebRequest.Result.Success:
        //                Debug.Log("received login RESPONSE at " + System.DateTime.Now);
        //                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
        //                string json = webRequest.downloadHandler.text;

        //                //JObject test = (JObject)JsonConvert.DeserializeObject(json);

        //                //foreach (JToken jt in test.Children())
        //                //{
        //                //    Debug.LogWarning("jt:Type" + jt.Type);
        //                //}
        //                //var testDetail = test["docs"];

        //                //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
        //                var result = JsonConvert.DeserializeObject<DXCommunications.GetAllClassesResponse>(json);
        //                if (thisApp == null)
        //                    thisApp = ScriptableObject.CreateInstance<DXAppLessonData>();                        thisApp.dxClass = result.docs.Find(x => x.course != null && x.name.ToLower().Contains("demo"));
        //                yield return MOTARDeveloperLessonInfoFromCourse();

        //                break;

        //            default:
        //                Exception error = null;
        //                error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
        //                break;
        //        }
        //    }



        //}
        public static IEnumerator MOTARDeveloperClassInfoFromLoggedOnSandboxUser()
        {

            string url = DXUrl.endpoint("edu/v1/class/list?limit=100");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {

                Debug.LogWarning("URL for class list was:" + url);
                webRequest.SetRequestHeader("Authorization", "Bearer " + DXCommunicationLayerEditor.DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        var result = JsonConvert.DeserializeObject<DXCommunications.GetAllClassesResponse>(json);
                        MOTARDeveloperCourseInfoFromClass(result);
                        thisAppClasses = result;
                        //if (thisApp == null)
                        //    thisApp = ScriptableObject.CreateInstance<DXAppLessonData>(); thisApp.dxClass = result.docs.Find(x => x.course != null && x.name.ToLower().Contains("demo"));
                        //yield return MOTARDeveloperLessonInfoFromCourse();

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }
        }

        public static void MOTARDeveloperCourseInfoFromClass(GetAllClassesResponse response)
        {
            List<DXCourse> courses = new List<DXCourse>();

            foreach (DXClass c in response.docs)
            {
                DXCourse course = c.course;
                if (course.classes == null) course.classes = new List<DXClass>();
                DXCourse existingCourse = courses.Find(x => x.courseId == course.courseId);

                if (existingCourse != null)
                    existingCourse.classes.Add(c);
                else
                {
                    course.classes.Add(c);
                    courses.Add(course);
                }
            }

            thisAppCourses = courses;

            String s = "";
            foreach (DXCourse course in thisAppCourses)
            {
                s += "Course Name: " + course.name + " | Classes: " + getClasses(course);
            }
            Debug.Log(s);
        }

        private static String getClasses(DXCourse course)
        {
            String s = "";
            foreach (DXClass c in course.classes)
            {
                s += c.name + " , ";
            }
            return s;
        }

        public static IEnumerator MOTARDeveloperEnumerateCompanyApps(DXDeveloperCompany dCompany)
        {

            //string url = "https://api.motar.io/sdk/v1/app/list?companyId=" + dCompany.id;
            string url = DXUrl.endpoint("sdk/v1/app/list?companyId=" + dCompany.id, DXEnvironment.Production);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;
                       // dynamic testme = JsonUtility.FromJson<dynamic>(json);
                        appList = (JArray) JsonConvert.DeserializeObject(json);
                        yield return LoadAppImages();
                        //  MOTARSetupWindow.instance.PopulateAppList();
                        //  yield return MOTARDeveloperLessonInfoFromApp();

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }
        public static IEnumerator MOTARDeveloperEnumerateCompanySandboxUsers(DXDeveloperCompany dCompany)
        {
            string url = DXUrl.endpoint("sdk/v1/user/list?companyId=" + dCompany.id, DXEnvironment.Production);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        endUserList = (JArray)JsonConvert.DeserializeObject(json);
                        yield return LoadSandboxUserImages();
                        //  MOTARSetupWindow.instance.PopulateAppList();
                        //  yield return MOTARDeveloperLessonInfoFromApp();

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }
        public static IEnumerator LoadAppImages()
        {
            if (appIcons == null)
                appIcons = new Dictionary<string, Texture2D>();
            else if (appIcons.Count > 0)
                appIcons.Clear();

            foreach(var x in appList)
            {
                string AppName = (string)x["name"];
                string icon = (string)x["icon"];
                if(!String.IsNullOrEmpty(icon))
                    yield return MOTARDeveloperImageRequest(appIcons,AppName,icon);


            }
        }
        public static IEnumerator LoadSandboxUserImages()
        {
            if (SandboxUserProfileImages == null)
                SandboxUserProfileImages = new Dictionary<string, Texture2D>();
            else if (SandboxUserProfileImages.Count > 0)
                SandboxUserProfileImages.Clear();

            foreach (var x in endUserList)
            {
                string profileID = (string)x["profilePic"];
                string userID = (string)x["userId"];
                if (!String.IsNullOrEmpty(userID) && !String.IsNullOrEmpty(profileID))
                    yield return MOTARDeveloperImageRequest(SandboxUserProfileImages, userID, profileID);


            }
        }
        public static IEnumerator MOTARDeveloperEnumerateCompanies()
        {
            string orgFilter = "";
            //string url = "https://api.motar.io/sdk/v1/company/list";
            string url = DXUrl.endpoint("sdk/v1/company/list",DXEnvironment.Production);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        companyList = JsonConvert.DeserializeObject<List<DXDeveloperCompany>>(json);
                        
                        MOTAROrgWindow.PopulateDXCompanies(companyList);

                        //if(companyList != null && companyList.Count > 0)
                        //{
                        //    DXDeveloperCompany dxCompany = companyList[0];
                        //    DXDeveloperCompany demoDxCompany = null; // companyList.Find(x => x.name == "Boeing");
                        //    if(orgFilter != "" && orgFilter != null)
                        //        demoDxCompany = companyList.Find(x => x.name == orgFilter);
                        //    if (demoDxCompany != null)
                        //        dxCompany = demoDxCompany;
                        //    yield return MOTARDeveloperEnumerateCompanyApps(dxCompany);
                        //    yield return MOTARDeveloperEnumerateCompanySandboxUsers(dxCompany);
                        //}
                        
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }
        public static IEnumerator MOTARDeveloperImageRequest(Dictionary<string,Texture2D> map, string dicKey,string imageKey)
        {
            //     private static string profileBase = Host + "/user/v1";
            //internal static string userProfile = profileBase + "/my/profile";
            //internal static string friendProfiles = profileBase + "/my/friends";
            //internal static string myProfilePicture = profileBase + "/my/profile/picture";
            //internal static string myCoverPhoto = profileBase + "/my/profile/cover";


            //string url = "https://api.motar.io/sdk/v1/image/" + imageKey;
            string url = DXUrl.endpoint("sdk/v1/image/" + imageKey,DXEnvironment.Production);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {

                webRequest.downloadHandler = new DownloadHandlerBuffer();
               
                webRequest.SetRequestHeader("Authorization", "Bearer " + DevAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        //Debug.Log("received binary api RESPONSE at " + System.DateTime.Now);

                        try
                        {
                            var data = webRequest.downloadHandler.data;
                            var base64 = Convert.ToBase64String(data);
                            var base64Data = Convert.FromBase64String(base64);
                            Texture2D tx = new Texture2D(128, 128);
                            tx.LoadImage(data);
                            map.Add(dicKey, tx);
                           
                            
                        }
                        catch (Exception err)
                        {
                            Debug.LogError(err.Message + " NO BINARY");
                        }

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }

        public static IEnumerator MOTARDeveloperLessonInfoFromCourse()
        {
            string url = DXUrl.endpoint("edu/v1/lesson/course") + "?courseId=" + thisApp.dxClass.course.courseId;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        //var result = JsonConvert.DeserializeObject<GetAllLessonsResponse>(json);
                        var result = JsonUtility.FromJson<GetAllLessonsResponse>(json);
                        thisApp.dxLesson = result.docs[0];
                        yield return MOTARDeveloperQuestionsFromLesson();
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }
        public static IEnumerator MOTARDeveloperLessonInfoFromCourseFromSandboxUser(DXClass dxClass, Action<GetAllLessonsResponse> completion)
        {
            string url = DXUrl.endpoint("edu/v1/lesson/course") + "?courseId=" + dxClass.course.courseId;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error + " for " + url);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        //var result = JsonConvert.DeserializeObject<GetAllLessonsResponse>(json);
                        var result = JsonUtility.FromJson<GetAllLessonsResponse>(json);
                        thisAppLessons = result;

                        //thisApp.dxLesson = result.docs[0];
                        //yield return MOTARDeveloperQuestionsFromLesson();
                        if (completion != null)
                            completion(result);
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }
        }

        public static IEnumerator MOTARDeveloperLessonInfoFromApp()
        {
            string url = DXUrl.endpoint("edu/v1/lesson/app");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        //var result = JsonConvert.DeserializeObject<GetAllLessonsResponse>(json);
                        var result = JsonUtility.FromJson<GetAllLessonsResponse>(json);
                        thisApp.dxLesson = result.docs[0];
                        yield return MOTARDeveloperQuestionsFromLesson();
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }
        public static IEnumerator MOTARDeveloperQuestionsFromLessonFromSandboxUser(DXLesson dXLesson,Action<List<DXAssessmentQuestion>> completion)
        {


            string url = DXUrl.endpoint("edu/v1/assessment") + "?assessmentId=" + dXLesson.media;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error + " for " + url);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        //var result = JsonConvert.DeserializeObject<GetAllLessonsResponse>(json);
                        var result = JsonConvert.DeserializeObject<List<DXAssessmentQuestion>>(json);
                        //thisApp.dxQuestionsList = result;
                        if (completion != null)
                            completion(result);
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }
        public static IEnumerator MOTARDeveloperQuestionsFromLesson()
        {


            string url = DXUrl.endpoint("edu/v1/assessment") + "?assessmentId=" + thisApp.dxLesson.media;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        //var result = JsonConvert.DeserializeObject<GetAllLessonsResponse>(json);
                        var result = JsonConvert.DeserializeObject<List<DXAssessmentQuestion>>(json);
                        thisApp.dxQuestionsList = result;

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }
        }

        public static IEnumerator MOTARDeveloperEventSetFromLessonFromSandboxUser(DXLesson dXLesson, Action<List<DXEvent>> completion)
        {
            string url = DXUrl.endpoint("edu/v1/event-set") + "?eventSetId=" + dXLesson.lessonId;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {


                webRequest.SetRequestHeader("Authorization", "Bearer " + DevSandboxAccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        completion(null);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);
                        completion(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        string json = webRequest.downloadHandler.text;

                        //JObject test = (JObject)JsonConvert.DeserializeObject(json);

                        //foreach (JToken jt in test.Children())
                        //{
                        //    Debug.LogWarning("jt:Type" + jt.Type);
                        //}
                        //var testDetail = test["docs"];

                        //var result = JsonUtility.FromJson<GetAllClassesResponse>(json);
                        //var result = JsonConvert.DeserializeObject<GetAllLessonsResponse>(json);
                        var result = JsonConvert.DeserializeObject<List<DXEvent>>(json);
                        thisAppEvents = result;
                        if (completion != null)
                            completion(result);

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        completion(null);
                        break;
                }
            }
        }

        private static int List<T>()
        {
            throw new NotImplementedException();
        }
        private static string QueryString(IDictionary<string, object> dict)
        {
            var list = new List<string>();
            foreach (var item in dict)
            {
                if (item.Value != null)
                {
                    list.Add(item.Key + "=" + item.Value);
                }
            }
            return string.Join("&", list);
        }

        public static List<DXDeveloperCompany> GetDXDeveloperCompanies()
        {
            return companyList;
        }
    }

}
