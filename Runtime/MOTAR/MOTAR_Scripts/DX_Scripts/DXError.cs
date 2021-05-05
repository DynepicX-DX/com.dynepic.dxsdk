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
    public class DXError : Exception
    {
        [SerializeField] private string error_code;

        public APIErrorCode ErrorCode
        {
            get => (APIErrorCode)Enum.Parse(typeof(APIErrorCode), error_code);
        }

        [SerializeField] private string error_description;

        public string ErrorDescription
        {
            get => error_description;
        }
    }
}