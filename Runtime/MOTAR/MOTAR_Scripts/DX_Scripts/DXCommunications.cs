//using DXDataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
namespace DXCommunications
{
    public enum DXEnvironment { Sandbox, Production, Development, FromConfiguration }
    public enum APIErrorCode
    {
        //    400
        GENERIC_BAD_REQUEST = 4000,
        PARAMETER_MISSING_FROM_REQUEST,
        DUPLICATE_RESOURCE_FOUND,
        VALIDATION_ERROR,
        INCORRECT_USER_TYPE,
        PARAMETERS_UNUSABLE,
        ACTION_IMPOSSIBLE,
        APP_ABOVE_DATA_LIMIT,
        TOKEN_ALREADY_USED,
        REQUESTED_CONTENT_HAS_BEEN_FLAGGED_FOR_MODERATION,


        //    401
        ACCESS_TOKEN_REFRESH_REQUIRED = 4010,
        INVALID_CREDENTIALS,
        API_KEY_INVALID,
        ACCOUNT_OR_RESOURCE_UNDER_MODERATION,
        INSUFFICIENT_PERMISSIONS_TO_ACCESS_RESOURCE,
        AUTHORIZATION_CODE_MALFORMED,
        AUTHORIZATION_MISSING_FROM_REQUEST,
        APP_DOES_NOT_HAVE_REQUESTED_SCOPE_REGISTERED,
        APP_DOES_NOT_HAVE_NOTIFICATIONS_ENABLED_FOR_THIS_DEVICE_TYPE,
        USER_HAS_NOT_GRANTED_CONSENT_FOR_THIS_APP,


        //    403
        USER_ATTEMPTED_ACTION_ON_DATA_THEY_ARE_NOT_AUTHORIZED_TO_ACCESS = 4032,
        AUTHORIZATION_REQUEST_IS_MISSING_PARAMETER,
        AUTHORIZATION_TOKEN_INVALID,
        ANONYMOUS_USER_ATTEMPTED_TO_ACCESS_RESOURCE_THAT_IS_NOT_AVAILABLE_TO_THEM,


        //    404
        REQUESTED_RESOURCE_WAS_NOT_FOUND = 4042,


        //    409
        CONFLICTS_WITH_EXISTING_RESOURCE = 4091,


        //    500
        UNCAUGHT_SERVER_ERROR = 5000,
        FAILED_TO_SAVE_DATA,
        FAILED_TO_UPDATE_DATA,
        INTERNAL_PROCESS_FAILED,
        INTERNAL_DATA_MISSING_OR_CORRUPTED,
        PARTNER_PROCESS_FAILED,


        //    505
        APP_VERSION_NO_LONGER_SUPPORTED = 5051
    }
   [Serializable]
    public class DXOfflineActivityQueueEntry
    {
        public DateTime queEntryTime;
        public string apiEndpoint;
        public string apiParametersJson;
    }

    
    public class DXUrl
    {
        public static DXConfiguration dXConfiguration;



        public DXUrl()
        {
            dXConfiguration = Resources.Load<DXConfiguration>("DX Configuration");

        }


        public static string Host(DXEnvironment environment = DXEnvironment.FromConfiguration)
        {
            DXEnvironment env = environment;
            if(env == DXEnvironment.FromConfiguration)
            {
                if(dXConfiguration == null)
                    dXConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();
                env = dXConfiguration.environment;
            }
            
            switch (env)
            {
                case DXEnvironment.Production:
                    return "https://api.motar.io";
                case DXEnvironment.Sandbox:
                    return "https://sandbox.motar.io";
                case DXEnvironment.Development:
                    return "https://api.motar-dev.com";

                default:
                    return "https://sandbox.motar.io";

            }
            
        }

        public static string endpoint(string EndOfEndpoint,DXEnvironment env = DXEnvironment.FromConfiguration)
        {
            return Host(env) + "/" + EndOfEndpoint;
        }


    }
    
    public class DXCommunicationLayer : ScriptableObject
    {
        public static Queue<DXOfflineActivityQueueEntry> DXOfflineUpdates;
        public static DXConfiguration dXConfiguration;
        public static string AccessToken
        {
            get => PlayerPrefs.GetString("DX-accessToken");
            set => PlayerPrefs.SetString("DX-accessToken", value);
        }

        public static string RefreshToken
        {
            get => PlayerPrefs.GetString("DX-refreshToken");
            set => PlayerPrefs.SetString("DX-refreshToken", value);
        }

        //public static DXClass thisClass;
        //public static DXLesson thisLesson;
        //public static List<DXAssessmentQuestion> thisQuestionList;

        public static DXAppLessonData thisApp;

        private static DXProfile loggedOnUserProfile;

        

        public static void DXLoginRequest(string handle, string password, Action<DXProfile> completion)
        {
            MOTARStateMachineHandler.instance.StartCoroutine(coDXLoginRequest(handle, password, completion));
        }

        public static void DXSSOLoginRequest(string accessToken,string refreshToken,Action<DXProfile> completion)
        {
            MOTARStateMachineHandler.instance.StartCoroutine(coDXSSOLoginRequest(accessToken,refreshToken,completion));
        }

        public static IEnumerator DXRefreshToken()
        {

            
            DXConfiguration dxConfig = ScriptableObject.CreateInstance<DXConfiguration>();
            var body = new Dictionary<string, object>
            {
                ["refresh_token"] = RefreshToken,
                ["client_id"] = dxConfig.clientID,
                ["client_secret"] = dxConfig.clientSecret,
                ["grant_type"] = "refresh_token"

            };
            RefreshToken = "";
            AccessToken = "";
            string json = JsonConvert.SerializeObject(body);


            var data = Encoding.UTF8.GetBytes(json);

            // var encoded = Encoding.UTF8.GetString(data);

            string url = DXUrl.endpoint("oauth/token", DXEnvironment.Sandbox);// "https://api.motar.io/oauth/token";
           

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

                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        var headers = webRequest.GetResponseHeaders();
                        if (headers.ContainsKey("accesstoken"))
                            headers["access_token"] = headers["accesstoken"];
                        if (headers.ContainsKey("refreshtoken"))
                            headers["refresh_token"] = headers["refreshtoken"];
                        headers.TryGetValue("access_token", out string accessToken);
                      
                        headers.TryGetValue("refresh_token", out string refreshToken);
                        
                        AccessToken = accessToken;
                        RefreshToken = refreshToken;

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);

                        break;
                }
            }



        }
        public static IEnumerator DXGetAuthTokenFromMOTARLaunch(string code,Action<DXProfile> completion)
        {

          
            DXConfiguration dxConfig = ScriptableObject.CreateInstance<DXConfiguration>();
            var body = new Dictionary<string, object>
            {
                ["code"] = code,
                ["client_id"] = dxConfig.clientID,
                ["client_secret"] = dxConfig.clientSecret,
                ["grant_type"] = "authorization_code"

            };
            RefreshToken = "";
            AccessToken = "";
            string json = JsonConvert.SerializeObject(body);


            var data = Encoding.UTF8.GetBytes(json);

            // var encoded = Encoding.UTF8.GetString(data);

            string url = DXUrl.endpoint("oauth/token");// "https://api.motar.io/oauth/token";


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
                        //var headers = webRequest.GetResponseHeaders();
                        // headers.TryGetValue("access_token", out string accessToken);
                        // headers.TryGetValue("refresh_token", out string refreshToken);
                        var tokens = (JToken)JsonConvert.DeserializeObject(webRequest.downloadHandler.text);

                        AccessToken = (string)tokens["access_token"]; ;
                        RefreshToken = (string)tokens["refresh_token"]; ; ;
                       // Debug.LogError("access became:" + AccessToken);
                      //  Debug.LogError("updated tokens from CODE");
                        yield return DXProfileRquest(completion);
                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);

                        break;
                }
            }



        }
        public static IEnumerator DXBinaryAPIRequest(string api, Action<byte[]> completion)
        {
            //     private static string profileBase = Host + "/user/v1";
            //internal static string userProfile = profileBase + "/my/profile";
            //internal static string friendProfiles = profileBase + "/my/friends";
            //internal static string myProfilePicture = profileBase + "/my/profile/picture";
            //internal static string myCoverPhoto = profileBase + "/my/profile/cover";

            DXConfiguration dxConfig = ScriptableObject.CreateInstance<DXConfiguration>();

            using (UnityWebRequest webRequest = UnityWebRequest.Get(DXUrl.endpoint(api)))
            {

                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("clientId", dxConfig.clientID);
                Debug.LogWarning("in new image: auth token:" + AccessToken);
                webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                
                //if (AccessToken != "")
                //{
                //    MOTARStateMachineHandler.instance.StartCoroutine(DXBinaryAPIRequest(api, completion));
                //}

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError(": HTTP Error: " + webRequest.downloadHandler.text);

                        if (webRequest.downloadHandler.text.ToLower().Contains("refresh"))
                        {


                            yield return DXRefreshToken();
                            if (AccessToken != "")
                            {
                                MOTARStateMachineHandler.instance.StartCoroutine(DXBinaryAPIRequest(api, completion));
                            }

                        }
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received binary api RESPONSE at " + System.DateTime.Now);
                       // yield return DXRefreshToken();
                        try
                        {
                            var data = webRequest.downloadHandler.data;
                            var base64 = Convert.ToBase64String(data);
                            var base64Data = Convert.FromBase64String(base64);
                            completion(base64Data);
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

       
        private static IEnumerator coDXLoginRequest(string handle, string password, Action<DXProfile> completion)
        {
            Debug.Log("received USER login request at " + System.DateTime.Now);
            string toConvert = ($"{handle}:{password}");

            byte[] bytes = Encoding.UTF8.GetBytes(toConvert);
            var authToken = Convert.ToBase64String(bytes);

            var body = new Dictionary<string, object>
            {
                ["handle"] = handle,
                ["password"] = password
            };

            DXUrl dXUrl = new DXUrl();
            if(dXConfiguration == null)
                dXConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();
           // dXConfiguration.ReloadConfig();

            string url = DXUrl.endpoint("auth/basic");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Authorization", $"Basic {authToken}");
                webRequest.SetRequestHeader("clientId", dXConfiguration.clientID);

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
                        completion(null);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                        var headers = webRequest.GetResponseHeaders();
                        headers.TryGetValue("access_token", out string accessToken);
                        headers.TryGetValue("refresh_token", out string refreshToken);
                        AccessToken = accessToken;
                        RefreshToken = refreshToken;
                        yield return DXProfileRquest(completion);

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        completion(null);
                        break;
                }
            }
        }
        public static IEnumerator coDXSSOLoginRequest(string accessToken, string refreshToken, Action<DXProfile> completion)
        {
                   
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            yield return DXProfileRquest(completion);
           
        }
        public static void AddXApiStatement(string verb,string studentID,string sObjectBody)
        {
            // Don't forget to use the DXCommunications namespace

            //Note HEADER parameters are used for authentication and are set automatically by the API

            Dictionary<string, object> body = new Dictionary<string, object>();

            body["timestamp"] = DateTime.Now.ToString();
            body["actor"] = studentID;
            body["verb"] = verb;
            body["object"] = sObjectBody;

            string sBody = JsonConvert.SerializeObject(body);
            // Don't forget to use the DXCommunications namespace

            //Note HEADER parameters are used for authentication and are set automatically by the API

            string api = "edu/v1/xapi/statement";
           
            //if (DXCommunicationLayer.DXOfflineUpdates == null)
            //    DXCommunicationLayer.DXOfflineUpdates = new Queue<DXOfflineActivityQueueEntry>();
            //DXOfflineActivityQueueEntry doae = new DXOfflineActivityQueueEntry();
            //doae.queEntryTime = System.DateTime.Now;
            //doae.apiEndpoint = api;
            //doae.apiParametersJson = sBody;
            //DXCommunicationLayer.DXOfflineUpdates.Enqueue(doae);


            MOTARStateMachineHandler.instance.StartCoroutine(DXPostAPIRequest<DXLessonProgress>(api, sBody, null));
        }
        public static void UpdateStudentAssessmentAnswers(bool correct, string answer, int questionIndex, string classID, string lessonID, Action<DXLessonProgress> completion)
        {
            var body = new UpdateProgressBody
            {
                correct = correct,
                answer = answer,
                questionIndex = questionIndex,
                classId = classID,
                lessonId = lessonID,
                studentId = ""
            };

            string api = "edu/v1/assessment/progress";
            string sBody = JsonConvert.SerializeObject(body);
            //if (DXCommunicationLayer.DXOfflineUpdates == null)
            //    DXCommunicationLayer.DXOfflineUpdates = new Queue<DXOfflineActivityQueueEntry>();
            //DXOfflineActivityQueueEntry doae = new DXOfflineActivityQueueEntry();
            //doae.queEntryTime = System.DateTime.Now;
            //doae.apiEndpoint = api;
            //doae.apiParametersJson = sBody;
            //DXCommunicationLayer.DXOfflineUpdates.Enqueue(doae);

           
            MOTARStateMachineHandler.instance.StartCoroutine(DXPostAPIRequest<DXLessonProgress>(api, sBody, completion));
        }
        public static void UpdateStudentsProgress(string classId, string lessonId, bool pass, Action<DXLessonProgress> completion,
            string studentId = null,
            bool? complete = null,
            string answers = null,
            int? score = null,
            string startTime = null,
            string endTime = null)
        {
            var body = new UpdateStudentsProgressBody
            {
                classId = classId,
                lessonId = lessonId,
                pass = pass,
                studentId = studentId,
                answers = answers,
                startTime = startTime,
                endTime = endTime
            };

            string api = "edu/v1/lesson/progress";
            string sBody = JsonConvert.SerializeObject(body);
            //if (DXCommunicationLayer.DXOfflineUpdates == null)
            //    DXCommunicationLayer.DXOfflineUpdates = new Queue<DXOfflineActivityQueueEntry>();
            //DXOfflineActivityQueueEntry doae = new DXOfflineActivityQueueEntry();
            //doae.queEntryTime = System.DateTime.Now;
            //doae.apiEndpoint = api;
            //doae.apiParametersJson = sBody;
            //DXCommunicationLayer.DXOfflineUpdates.Enqueue(doae);
            MOTARStateMachineHandler.instance.StartCoroutine(DXPostAPIRequest<DXLessonProgress>(api, sBody, completion));
        }
        public static IEnumerator DXPostAPIRequest<T>(string api, string json, Action<T> completion)
        {
            if (DXCommunicationLayer.DXOfflineUpdates == null)
                DXCommunicationLayer.DXOfflineUpdates = new Queue<DXOfflineActivityQueueEntry>();
            DXOfflineActivityQueueEntry doae = new DXOfflineActivityQueueEntry();
            doae.queEntryTime = System.DateTime.Now;
            doae.apiEndpoint = api;
            doae.apiParametersJson = json;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {


                
                DXCommunicationLayer.DXOfflineUpdates.Enqueue(doae);
            }
            else
            {

                if (dXConfiguration == null)
                    dXConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();



                var data = Encoding.UTF8.GetBytes(json);
                UnityWebRequest webRequest = new UnityWebRequest(DXUrl.endpoint(api));
                webRequest.method = UnityWebRequest.kHttpVerbPOST;
                webRequest.uploadHandler = new UploadHandlerRaw(data);
                webRequest.uploadHandler.contentType = "application/json";
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("clientId", dXConfiguration.clientID);
                webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);

                yield return webRequest.SendWebRequest();


                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError(": Connection Error: " + webRequest.error);
                        DXCommunicationLayer.DXOfflineUpdates.Enqueue(doae);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(": Data Processing Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(": HTTP Error: " + webRequest.error);
                        Debug.LogError("DETAILS:" + webRequest.downloadHandler.text);

                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("received login RESPONSE at " + System.DateTime.Now);
                        Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);




                        string outJson = webRequest.downloadHandler.text;


                        var OutCompletion = JsonUtility.FromJson<T>(outJson);
                        if (completion != null)
                            completion(OutCompletion);


                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }

            }
          



        }
        //public static IEnumerator DXPostAPIRequest(string api, string json, Action<string> completion)
        //{


        //    if (dXConfiguration == null)
        //        dXConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();



        //    var data = Encoding.UTF8.GetBytes(json);
        //    UnityWebRequest webRequest = new UnityWebRequest(DXUrl.endpoint(api));
        //    webRequest.method = UnityWebRequest.kHttpVerbPOST;
        //    webRequest.uploadHandler = new UploadHandlerRaw(data);
        //    webRequest.uploadHandler.contentType = "application/json";
        //    webRequest.downloadHandler = new DownloadHandlerBuffer();
        //    webRequest.SetRequestHeader("clientId", dXConfiguration.clientID);
        //    webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);

        //    yield return webRequest.SendWebRequest();


        //    switch (webRequest.result)
        //    {
        //        case UnityWebRequest.Result.ConnectionError:
        //        case UnityWebRequest.Result.DataProcessingError:
        //            Debug.LogError(": Error: " + webRequest.error);
        //            break;
        //        case UnityWebRequest.Result.ProtocolError:
        //            Debug.LogError(": HTTP Error: " + webRequest.error);

        //            break;
        //        case UnityWebRequest.Result.Success:
        //            Debug.Log("received login RESPONSE at " + System.DateTime.Now);
        //            Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);




        //            string outJson = webRequest.downloadHandler.text;


                    
        //            completion(outJson);


        //            break;

        //        default:
        //            Exception error = null;
        //            error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
        //            break;
        //    }




        //}
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


        public static IEnumerator DXAPIRequest(string api, string method,Action<object> completion, Dictionary<string, object> queryParameters = null, Dictionary<string, object> body = null,Texture2D txImage=null )
        {
            

            string url = DXUrl.endpoint(api);
            if(queryParameters != null)
            {
                url += "?"+QueryString(queryParameters);
            }
            string bodyJson = "";
            UnityWebRequest webRequest = null;
            object apiResponse = null;

            switch(method)
            {
                case "GET":
                    webRequest = UnityWebRequest.Get(url);
                    break;

                case "POST":
                    bodyJson = JsonConvert.SerializeObject(body);
                    var dataPost = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(bodyJson));
                    webRequest = UnityWebRequest.Post(url, dataPost);
                    break;

                case "PUT":

                    bodyJson = JsonConvert.SerializeObject(body);
                    var data = Encoding.UTF8.GetBytes(bodyJson);
                    webRequest = UnityWebRequest.Put(url, data);
                    break;

            }

            using (webRequest)
            //using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                if (dXConfiguration == null)
                    dXConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();

                webRequest.SetRequestHeader("clientId", dXConfiguration.clientID);
                webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                Debug.Log("received " + api + " RESPONSE at " + System.DateTime.Now);

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("API:" + webRequest.url + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("API:" + webRequest.url + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        try
                        {
                            string jsonOut = webRequest.downloadHandler.text;
                            if (jsonOut[0] == '{')
                            {
                                apiResponse = JsonConvert.DeserializeObject(jsonOut);
                            }
                            else if (txImage != null)
                            {
                                var base64 = Convert.ToBase64String(webRequest.downloadHandler.data);
                                var base64Data = Convert.FromBase64String(base64);
                                if (txImage == null)
                                    txImage = new Texture2D(128, 128);
                                txImage.LoadImage(base64Data);
                                apiResponse = txImage;
                            }
                            else
                                apiResponse = webRequest.downloadHandler.data;
                        }
                        catch
                        {
                            apiResponse = webRequest.downloadHandler.data;
                        }



                        

                        //string json = webRequest.downloadHandler.text;


                        // completion(webRequest.downloadHandler);

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }

                if (completion != null)
                    completion(apiResponse);

                
            }



        }
        public static IEnumerator DXProfileRquest(Action<DXProfile> completion)
        {
            //     private static string profileBase = Host + "/user/v1";
            //internal static string userProfile = profileBase + "/my/profile";
            //internal static string friendProfiles = profileBase + "/my/friends";
            //internal static string myProfilePicture = profileBase + "/my/profile/picture";
            //internal static string myCoverPhoto = profileBase + "/my/profile/cover";

            string url = DXUrl.endpoint("user/v1/my/profile");
            //Debug.LogError("getting profile from " + url);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {

                //Debug.LogError("Set auth token to " + AccessToken);
                webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
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
                        var result = JsonConvert.DeserializeObject<DXProfile>(json);
                        loggedOnUserProfile = result;
                        yield return DXProfileFriendsRequest();
                        completion(loggedOnUserProfile);

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }



        }

        public static IEnumerator DXProfileFriendsRequest() {
            //     private static string profileBase = Host + "/user/v1";
            //internal static string userProfile = profileBase + "/my/profile";
            //internal static string friendProfiles = profileBase + "/my/friends";
            //internal static string myProfilePicture = profileBase + "/my/profile/picture";
            //internal static string myCoverPhoto = profileBase + "/my/profile/cover";

            string url = DXUrl.endpoint("user/v1/my/friends");
            //Debug.LogError("getting profile from " + url);
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {

                //Debug.LogError("Set auth token to " + AccessToken);
                webRequest.SetRequestHeader("Authorization", "Bearer " + AccessToken);
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();


                switch (webRequest.result) {
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

                        loggedOnUserProfile.friends = JsonConvert.DeserializeObject<List<DXProfile>>(json);

                        break;

                    default:
                        Exception error = null;
                        error = JsonUtility.FromJson<DXError>(webRequest.downloadHandler.text);
                        break;
                }
            }
        }

    }


}
