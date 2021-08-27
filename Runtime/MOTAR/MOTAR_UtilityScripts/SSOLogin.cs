using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DXCommunications;


namespace DX
{
    public class SSOLogin : MonoBehaviour, IPointerClickHandler
    {
        private static DXConfiguration dxConfiguration;

        //public string URL;
        TextMeshProUGUI textMeshPro;
        // Start is called before the first frame update
        void Start()
        {

            MOTARSSOMain.StartServer(null);
            textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
           

            if (dxConfiguration == null)
                dxConfiguration = ScriptableObject.CreateInstance<DXConfiguration>();

            string URL = DXUrl.endpoint("oauth/signin", dxConfiguration.environment);

            string url = URL + "?client_id=#CLIENTID&response_type=implicit&state=12345&redirect_uri=http://127.0.0.1/MOTAR&app_login=false";
            url = url.Replace("#CLIENTID", dxConfiguration.clientID);
            Application.OpenURL(url);
            
        }
       
        public void OnApplicationQuit()
        {
            MOTARSSOMain.StopServer();
        }

        public void OnDestroy()
        {
            MOTARSSOMain.StopServer();
        }

        /* 
         * client_id
    REQUIRED
    string
    Your app's client ID, generated in MOTAR Studio.
    response_type
    REQUIRED
    string
    Determines what auth flow to use. Can be "code" or "implicit". "Code" auth flow will generate an auth-code that can be used to retrieve an auth token, while "implicit" will generate an auth and refresh token directly.
    redirect_uri
    REQUIRED
    string
    After logging in, the user will be redirected to this URI. Must be registered for your app in MOTAR Studio beforehand.
    state
    REQUIRED
    string
    This will be passed through the auth flow and to the redirect URI. Can be any arbitrary string.
    app_login
    OPTIONAL
    string
    Determines whether or not the OAuth screen should include the "Login via MOTAR App" button. Set to "true" if your client is an app to allow users to login if they have the MOTAR app.
         */
    }
}

