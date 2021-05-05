//using DXDataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Newtonsoft.Json;

namespace DXCommunications
{
    [Serializable]
    public class DXConfiguration : ScriptableObject
    {
        public string Name;
        public string Description;

        [Header("Enter Client ID and Client Secret")]
        public string clientID;
        public string clientSecret;

        //[FormerlySerializedAs("DXEnvironment"), Header("Select Development Environment")]
        public DXEnvironment environment;

        public DXConfiguration()
        {
            //var newConfig = Resources.Load("DX Configuration");
            //environment = DXEnvironment.Sandbox;
        }
        public void ReloadConfig()
        {
            OnEnable();
        }
        private void OnEnable()
        {
            //var oldconfig = Resources.Load<DX.DXConfiguration>("DX Configuration");
            //if (oldconfig != null)
            //{
            //    clientID = oldconfig.clientID;
            //    clientSecret = oldconfig.clientSecret;
            //    if (oldconfig.environment == (DX.DXEnvironment)DXEnvironment.Production)
            //        environment = DXEnvironment.Production;
            //    else
            //        environment = DXEnvironment.Sandbox;
            //}
            //else
            {
                //Debug.LogError("CALLING ENABLE FROM...");
                var newConfig = Resources.Load("DX Configuration") as DXConfiguration;
                if (newConfig != null)
                {
                    environment = DXEnvironment.Sandbox;
                    clientID = newConfig.clientID;
                    clientSecret = newConfig.clientSecret;
                    Name = newConfig.Name;
                    Description = newConfig.Description;
                }
            }

        }
        [Serializable] 
        public class OldDXConfiguration:ScriptableObject
            
        {
            [Header("Enter Client ID and Client Secret")]
            public string clientID;
            public string clientSecret;

            [FormerlySerializedAs("DXEnvironment"), Header("Select Development Environment")]
            public DXEnvironment environment;
        }

    }
}