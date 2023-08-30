using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Analytic
{
    public partial class AnalyticManager
    {
        public const int REQUEST_INTERVAL = 10;
        public const int RETRY_NUMBER = 3;
        public const int EVENT_PER_REQUEST = 300;
        public static readonly string EN_POINT = "https://x.skymavis.com/track";
        public static string API_KEY;
        private static HeartbeatStats _heartbeatStats;
        private static string _currentName = "";

        private static readonly Dictionary<AnalyticRequest, int> dictRetryRequests = new Dictionary<AnalyticRequest, int>();

        private static IEnumerator CheckingTimer()
        {
            while (true)
            {
                if (initialized && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(API_KEY))
                {
                    if (Time.realtimeSinceStartup - lastRequestTime > REQUEST_INTERVAL)
                    {
                        lastRequestTime = Time.realtimeSinceStartup;
                        CheckSendAnalyticRequest();
                    }
                    OnCheckInput();
                }

                yield return null;
            }
        }

        private static void CheckSendAnalyticRequest()
        {
            if (analyticListData == null) return;
            if(analyticListData.listEventDatas.Count == 0)
            {
                //string heartbeatData = JsonConvert.SerializeObject(_heartbeatStats);

                //var jObject = new JObject();
                //var jHeartbeatData = JObject.Parse(heartbeatData);
                //jObject.Add("action_properties", jHeartbeatData);
                //jObject.Add(new JProperty("event", "heartbeat"));
                AddEvent(EventTypes.Track, new
                {
                    @event = "heartbeat",
                    action_properties = _heartbeatStats
                });
                _heartbeatStats.EndBeat();
            }

            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (activeScene != null && activeScene.name != _currentName)
            {
                _currentName = activeScene.name;
                AnalyticManager.AddEvent(EventTypes.Screen, AnalyticConst.NewScreenEvent("s_" + _currentName));
            }

            //Create new request
            DoRequest();
        }

        private static void Submit(AnalyticRequest data, Action<AsyncOperation> cb)
        {
            var request = new UnityWebRequest(EN_POINT, "POST"); // start as POST, then change later
            request.SetRequestHeader("Content-Type", "application/json");

            var apiBytes = Encoding.UTF8.GetBytes($"{API_KEY}:");
            string encodedText = Convert.ToBase64String(apiBytes, Base64FormattingOptions.None);
            request.SetRequestHeader("Authorization", "Basic " + encodedText);
            request.downloadHandler = new DownloadHandlerBuffer();
            var json = JsonConvert.SerializeObject(data, AnalyticListData.SerializerSettings);
            Debug.Log(json);
            var jsonBytes = new UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);


            var requestQueueOperation = WebRequestQueue.QueueRequest(request);
            if (requestQueueOperation.IsDone)
            {
                var requestOperation = requestQueueOperation.Result;
                if (requestOperation.isDone)
                    cb(requestOperation);
                else
                    requestOperation.completed += cb;
            }
            else
            {
                requestQueueOperation.OnComplete += asyncOperation =>
                {
                    var requestOperation = asyncOperation;
                    requestOperation.completed += cb;
                };
            }
        }

        private static IEnumerator RequestAfter(AnalyticRequest analyticRequest, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            DoRequest(analyticRequest);
        }

        private static void DoRequest(AnalyticRequest analyticRequest = null)
        {
            if (!initialized)
                return;
            analyticRequest ??= NewRequest();
            if (analyticRequest == null) return;

            Submit(analyticRequest, (op) =>
            {
                var webOp = op as UnityWebRequestAsyncOperation;
                var webReq = webOp.webRequest;

                if (webReq.responseCode != 200 || !string.IsNullOrEmpty(webReq.error))
                {
                    var tryCount = 0;
                    if (dictRetryRequests.ContainsKey(analyticRequest))
                    {
                        tryCount = dictRetryRequests[analyticRequest];
                        Debug.Log($"analytic retry {tryCount}. Error: {webReq.error} - responseCode: {webReq.responseCode}");
                    }

                    tryCount++;
                    if (tryCount < RETRY_NUMBER)
                    {
                        dictRetryRequests[analyticRequest] = tryCount;
                        analyticBehavior.StartCoroutine(RequestAfter(analyticRequest, Mathf.Pow(2, tryCount)));
                    }
                    else
                    {
                        Finish(analyticRequest);
                    }
                }
                else
                {
                    Finish(analyticRequest);
                }
            });
        }

        static AnalyticRequest NewRequest()
        {
            var listEvent = new List<AnalyticEvent>(analyticListData.listEventDatas);
            foreach (var request in dictRetryRequests)
            {
                foreach (var theEvent in request.Key.events)
                {
                    listEvent.Remove(theEvent);
                }
            }

            listEvent = listEvent.Take(EVENT_PER_REQUEST).ToList();
            if (listEvent.Count == 0)
                return null;
            return new AnalyticRequest(listEvent);
        }

        static void Finish(AnalyticRequest request)
        {
            dictRetryRequests.Remove(request);
            foreach (var theEvent in request.events)
            {
                analyticListData.listEventDatas.Remove(theEvent);
            }

            analyticListData.SaveToDevice();
            //Debug.Log($"[Analytic] Request Finish: {request.events.Count}");
        }
    }
}
