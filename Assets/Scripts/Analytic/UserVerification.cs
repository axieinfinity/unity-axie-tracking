using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class UserVertification
{
    public static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return string.Empty;
    }

    public static void GetArgVertification(out string signature, out string message)
    {
        signature = GetArg("-signature");
        message = GetArg("-message");
    }

    private static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    public static JObject Vertification(string signature, string message)
    {
        if (string.IsNullOrEmpty(message)) return null;
        
        var messageData = StringToByteArray(message);
        var messageStr = System.Text.Encoding.UTF8.GetString(messageData);
        //var signatureData = StringToByteArray(signature);
        try
        {
            //This not support on net20, so just skip verify at client side
            // var rsa = RSA.Create();
            // rsa.ImportRSAPublicKey(DecodeOpenSSLPublicKey(rsaPub), out var bytesRead);
            // UnityEngine.Debug.Log($"bytesRead: ${bytesRead}");

            JObject userData = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(messageStr);
            if (userData != null)
            {
                UnityEngine.Debug.Log($"MH user data: {Newtonsoft.Json.JsonConvert.SerializeObject(userData)}");
                return userData;
            }
            else
            {
                return null;
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex);
            return null;
        }
    }
}
