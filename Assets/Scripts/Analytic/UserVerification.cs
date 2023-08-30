using System.Security.Cryptography;
using System.Text;
using System;
using System.Diagnostics;
using UnityEngine;
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
        //Example
        //open -a test.app --args -message 7B22757365724944223A223C757569643E222C22726F6E696E41646472657373223A2230782E2E2E222C2265786368616E6765644174223A313639313734303530372C2267616D65536C7567223A2270726F6A6563742D74227D -signature 3044022061b9961d76ede6f38426220552a003186982ea5e48ebc7fdd0074e432689807e0220616f09f35b7c850e62a3c2de342e535d2d095d2f46bc4c62853dd001fb2bf185

        var messageData = StringToByteArray(message);
        var messageStr = System.Text.Encoding.UTF8.GetString(messageData);
        var signatureData = StringToByteArray(signature);
        UnityEngine.Debug.Log("Message string: " + messageStr);
        try
        {
            JObject userData = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(messageStr);
            if (userData != null)
            {
                UnityEngine.Debug.Log($"User data: {Newtonsoft.Json.JsonConvert.SerializeObject(userData)}");
                return userData;
            }
            else
            {
                UnityEngine.Debug.LogError("User data is NULL");
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