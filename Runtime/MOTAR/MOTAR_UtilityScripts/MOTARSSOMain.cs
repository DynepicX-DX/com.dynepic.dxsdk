using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DXCommunications
{
   
    public class MOTARSSOMain
    {
        //public static WebSocketServer wssv;
        //public static HttpServer hssv;

        public static bool isAuthenticated = false;
        public static string accessToken;
        public static string refreshToken;
        public static System.IntPtr hwnd;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, uint flag);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);
        public static async void StartServer(string[] args)
        {
            //wssv = new WebSocketServer("ws://127.0.0.1");
            ////wssv.SslConfiguration.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2();
            //wssv.AddWebSocketService<MOTARWSS>("/MOTAR");
            //wssv.Start();
            //hssv = new HttpServer("http://localhost");
            //hssv.SslConfiguration.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2();
            //hssv.AddWebSocketService<MOTARWSS>("/MOTAR");
            //hssv.Start();
            string[] prefixes = new string[] { "http://127.0.0.1/MOTAR/", "https://127.0.0.1/MOTAR/" };

            var result = Task.Run(async() => {
                hwnd = GetForegroundWindow();
                SimpleListenerExample(prefixes);
                //await Task.Delay(2000);

                //SetForegroundWindow(p);

                });

            await result;
        }
        public static void StopServer()
        {
            //wssv.Stop();
            //hssv.Stop();
        }
        
        public static void SimpleListenerExample(string[] prefixes)
        {
            isAuthenticated = false;
            var p = Process.GetCurrentProcess();
            
            if (!HttpListener.IsSupported)
            {
                //Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");
            // Note: The GetContext method blocks while waiting for a request.
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            accessToken = request.QueryString["access_token"];
            refreshToken = request.QueryString["refresh_token"];
           

            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY>You've been authanticated!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
          

            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
            listener.Stop();
            //MOTARStateMachineHandler.instance.MOTARRuntimeStateMachine.SetTrigger("LoginSuccess");
            isAuthenticated = true;

        }

      

        

      
    }
}
