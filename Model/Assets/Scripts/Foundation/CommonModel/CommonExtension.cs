using System;
using System.Text;
using UnityEngine.Networking;

namespace Foundation.CommonModel
{
    public static class CommonExtension
    {  
        public static string EncodingText(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return UnityWebRequest.EscapeURL(text, Encoding.UTF8);
        }
        
        public static string EncodingText(this Guid guid)
        {
            return guid.ToString().EncodingText();
        }

        
    }
}