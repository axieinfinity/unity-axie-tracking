using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
//using CryptoUtils;
using System.Linq;

public class CustomMenu
{
    [UnityEditor.MenuItem("Editor/Test Vertification")]
    static void TestVertification()
    {
        Debug.Log("Test Vertification...");
        string signature = "3044022061b9961d76ede6f38426220552a003186982ea5e48ebc7fdd0074e432689807e0220616f09f35b7c850e62a3c2de342e535d2d095d2f46bc4c62853dd001fb2bf185";
        string message = "7B22757365724944223A223C757569643E222C22726F6E696E41646472657373223A2230782E2E2E222C2265786368616E6765644174223A313639313734303530372C2267616D65536C7567223A2270726F6A6563742D74227D";
        UserVertification.Vertification(signature, message);
    }
}
