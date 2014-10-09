using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet
{
    public class Helpers
    {
        public static string ReadWebResponse(HttpWebResponse webResponse)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webResponse.GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            finally
            {
                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }
            }

            return responseData;
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Ceiling(diff.TotalSeconds);
        }

        public static string Encode(string input, string key)
        {
            var keyMaterial = System.Text.Encoding.UTF8.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(keyMaterial);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            byte[] res = myhmacsha1.ComputeHash(stream);
            var b64 = Convert.ToBase64String(res);
            return System.Uri.EscapeDataString(b64);
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
