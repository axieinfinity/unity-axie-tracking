using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Analytic
{

    public partial class AnalyticManager
    {
        private class AnalyticBehavior : MonoBehaviour
        {
        }

        public static string buildVersion => $"{Application.version}";

        public static string sessionId { get; private set; }
        public static long sessionOffset { get; private set; }
        public static bool initialized { get; private set; }
        public static string userId;
        public static string env;

        private static float lastRequestTime;

        private static AnalyticListData analyticListData = new AnalyticListData(AnalyticListData.analyticListData);
        private static AnalyticBehavior analyticBehavior;

        public static void IdentifyLocalUser()
        {
            JObject jObject = LoadLocalProfile();

            UserVertification.GetArgVertification(out var signature, out var message);
            var userData = UserVertification.Vertification(signature, message);
            JObject jUserProperties = new JObject();
            if (userData != null)
            {
                string roninAddress = (string)userData["roninAddress"];
                if (string.IsNullOrEmpty(roninAddress))
                {
                    jObject.Add(new JProperty("ronin_address", roninAddress));
                    jUserProperties.Add(new JProperty("ronin_address", roninAddress));
                }
            }
            jUserProperties.Add(new JProperty("system_memory_size", SystemInfo.systemMemorySize));
            jUserProperties.Add(new JProperty("processor_count", SystemInfo.processorCount));
            jUserProperties.Add(new JProperty("graphics_device", SystemInfo.graphicsDeviceName));
            jUserProperties.Add(new JProperty("graphics_memory_size", SystemInfo.graphicsMemorySize));
            jObject.Add(new JProperty("user_properties", jUserProperties));
            AnalyticManager.AddIdentifyEvent(jObject);
        }

        public static void IdentifyCustomUser(object userProperties)
        {
            JObject jUserProperties = null;
            if (userProperties is JObject)
            {
                jUserProperties = userProperties as JObject;
            }
            else
            {
                jUserProperties = JObject.Parse(JsonConvert.SerializeObject(userProperties));
            }
            JObject jObject = LoadLocalProfile();

            UserVertification.GetArgVertification(out var signature, out var message);
            var userData = UserVertification.Vertification(signature, message);
            if (userData != null)
            {
                string roninAddress = (string)userData["roninAddress"];
                if (string.IsNullOrEmpty(roninAddress))
                {
                    jObject.Add(new JProperty("ronin_address", roninAddress));
                }
            }

            if (jUserProperties != null)
            {
                string roninAddress = (string)jUserProperties["ronin_address"];
                if (!string.IsNullOrEmpty(roninAddress))
                {
                    jObject["ronin_address"] = roninAddress;
                }
                string userId = (string)jUserProperties["user_id"];
                if (!string.IsNullOrEmpty(userId))
                {
                    AnalyticManager.userId = userId;
                }
                jObject.Add(new JProperty("user_properties", jUserProperties));
            }

            StartNewSession(AnalyticManager.userId);
            AnalyticManager.AddIdentifyEvent(jObject);
        }

        private static JObject LoadLocalProfile()
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
            return jObject;
        }

        public static void InitManager(string apiKey)
        {
            AnalyticManager.API_KEY = apiKey;
            if (initialized) return;
            if (string.IsNullOrEmpty(EN_POINT) || string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("AnalyticManager invalid endpoint");
                return;
            }
            analyticListData = AnalyticListData.LoadFromDevice();
            sessionId = Guid.NewGuid().ToString();
            sessionOffset = 1;

            AnalyticManager._heartbeatStats = new HeartbeatStats();
            AnalyticManager._heartbeatStats.Init();

            lastRequestTime = float.MinValue;
            initialized = true;

            var behavior = new GameObject();
            behavior.name = "AnalyticBehavior";
            analyticBehavior = behavior.AddComponent<AnalyticBehavior>();
            UnityEngine.GameObject.DontDestroyOnLoad(behavior);

            analyticBehavior.StartCoroutine(CheckingTimer());
            Debug.Log("AnalyticManager StartSession");
        }

        private static void StartNewSession(string userId)
        {
            if (userId != PlayerPrefs.GetString("userId"))
            {
                PlayerPrefs.SetString("userId", userId);
                sessionId = Guid.NewGuid().ToString();
                sessionOffset = 1;

                AnalyticManager._heartbeatStats = new HeartbeatStats();
                AnalyticManager._heartbeatStats.Init();
            }
        }

        public static void AddIdentifyEvent(object data)
        {
            var analyticEvent = new AnalyticEvent(EventTypes.Identify, data);
            AddEvent(analyticEvent);
        }

        public static void AddScreenEvent(object data)
        {
            var analyticEvent = new AnalyticEvent(EventTypes.Screen, data);
            AddEvent(analyticEvent);
        }

        public static void AddTrackEvent(object data)
        {
            var analyticEvent = new AnalyticEvent(EventTypes.Track, data);
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
