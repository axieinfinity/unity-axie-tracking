using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Analytic
{
    public partial class AnalyticManager
    {
        public static string buildVersion => $"{Application.version}";
       
        public static string sessionId { get; private set; }
        public static long sessionOffset { get; private set; }
        public static bool initialized { get; private set; }
        public static string userId;
        public static string env;
        
        private static float lastRequestTime;

        private static AnalyticListData analyticListData = new AnalyticListData(AnalyticListData.analyticListData);
        public static AnalyticBehavior analyticBehavior;
     
        public static void IdentifyLocalUser()
        {
            if (AnalyticManager.initialized)
            {
                string userId = PlayerPrefs.GetString("userId");
                if (string.IsNullOrEmpty(userId))
                {
                    userId = System.Guid.NewGuid().ToString();
                    PlayerPrefs.SetString("userId", userId);
                }
                AnalyticManager.userId = userId;
#if UNITY_EDITOR
            AnalyticManager.env = "dev";
#else
                AnalyticManager.env = "staging";
#endif

                var jObject = new JObject();
                jObject.Add(new JProperty("device_name", SystemInfo.deviceModel));
                jObject.Add(new JProperty("device_id", SystemInfo.deviceUniqueIdentifier));
                jObject.Add(new JProperty("platform_name", Application.platform.ToString()));
                jObject.Add(new JProperty("platform_version", SystemInfo.operatingSystem));

                jObject.Add(new JProperty("system_memory_size", SystemInfo.systemMemorySize));
                jObject.Add(new JProperty("processor_count", SystemInfo.processorCount));
                jObject.Add(new JProperty("graphics_device", SystemInfo.graphicsDeviceName));
                jObject.Add(new JProperty("graphics_memory_size", SystemInfo.graphicsMemorySize));
                AnalyticManager.AddEvent(EventTypes.Identify, jObject);
            }
        }

        public static void IdentifyUserData(JObject userData)
        {
            if (AnalyticManager.initialized)
            {
                string userId = PlayerPrefs.GetString("userId");
                if (string.IsNullOrEmpty(userId))
                {
                    userId = System.Guid.NewGuid().ToString();
                    PlayerPrefs.SetString("userId", userId);
                }

                string roninAddress = "";
                try
                {
                    roninAddress = userData["roninAddress"].Value<string>();
                }
                catch (System.Exception ex) { }

                AnalyticManager.userId = userId;
#if UNITY_EDITOR
            AnalyticManager.env = "dev";
#else
                AnalyticManager.env = "staging";
#endif

                var jObject = new JObject();
                jObject.Add(new JProperty("ronin_address", roninAddress));
                jObject.Add(new JProperty("device_name", SystemInfo.deviceModel));
                jObject.Add(new JProperty("device_id", SystemInfo.deviceUniqueIdentifier));
                jObject.Add(new JProperty("platform_name", Application.platform.ToString()));
                jObject.Add(new JProperty("platform_version", SystemInfo.operatingSystem));

                jObject.Add(new JProperty("system_memory_size", SystemInfo.systemMemorySize));
                jObject.Add(new JProperty("processor_count", SystemInfo.processorCount));
                jObject.Add(new JProperty("graphics_device", SystemInfo.graphicsDeviceName));
                jObject.Add(new JProperty("graphics_memory_size", SystemInfo.graphicsMemorySize));
                AnalyticManager.AddEvent(EventTypes.Identify, jObject);
            }
        }

        public static void InitManager(string apiKey)
        {
            AnalyticManager.apiKey = apiKey;
            if (initialized) return;
            if (string.IsNullOrEmpty(endPoint) || string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("AnalyticManager invalid endpoint");
                return;
            }
            analyticListData = AnalyticListData.LoadFromDevice();
            sessionId = Guid.NewGuid().ToString();
            sessionOffset = 1;


            AnalyticManager.heartbeatStats = new HeartbeatStats();
            AnalyticManager.heartbeatStats.Init();

            lastRequestTime = float.MinValue;
            initialized = true;

            var behavior = new GameObject();
            behavior.name = "AnalyticBehavior";
            analyticBehavior = behavior.AddComponent<AnalyticBehavior>();

            analyticBehavior.StartCoroutine(CheckingTimer());
            Debug.Log("AnalyticManager StartSession");
        }

        public static void AddEvent(EventTypes type, object data)
        {
            var analyticEvent = new AnalyticEvent(type, data);
            AddEvent(analyticEvent);
        }

        private static void AddEvent(AnalyticEvent analyticEvent)
        {
            if (!initialized) return;
            analyticEvent.MergeData(GetCommonFields());
            analyticListData.listEventDatas.Add(analyticEvent);
            analyticListData.SaveToDevice();
        }

        private static object GetCommonFields()
        {
            var internetType = "unknown";
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                internetType = "wifi_or_cable";
            }
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                internetType = "data_network";
            }

            return new
            {
                
                uuid = Guid.NewGuid().ToString(),
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                session_id = sessionId,
                offset = sessionOffset++,
                user_id = userId,
                build_version = env + ":" + buildVersion,
                internet_type = internetType,
            };
        }
    }
}
