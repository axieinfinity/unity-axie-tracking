using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Analytic
{
    internal class WebRequestQueueOperation
    {
        public UnityWebRequestAsyncOperation Result;
        public Action<UnityWebRequestAsyncOperation> OnComplete;

        public bool IsDone
        {
            get { return Result != null; }
        }

        internal UnityWebRequest _webRequest;

        public WebRequestQueueOperation(UnityWebRequest request)
        {
            _webRequest = request;
        }

        internal void Complete(UnityWebRequestAsyncOperation asyncOp)
        {
            Result = asyncOp;
            OnComplete?.Invoke(Result);
        }
    }

    internal static class WebRequestQueue
    {
        private static int s_MaxRequest = 10;
        internal static Queue<WebRequestQueueOperation> s_QueuedOperations = new Queue<WebRequestQueueOperation>();
        internal static List<UnityWebRequestAsyncOperation> s_ActiveRequests = new List<UnityWebRequestAsyncOperation>();

        public static WebRequestQueueOperation QueueRequest(UnityWebRequest request)
        {
            WebRequestQueueOperation queueOperation = new WebRequestQueueOperation(request);
            if (s_ActiveRequests.Count < s_MaxRequest)
            {
                var requestAsyncOp = request.SendWebRequest();
                requestAsyncOp.completed += OnWebAsyncOpComplete;
                s_ActiveRequests.Add(requestAsyncOp);
                queueOperation.Complete(requestAsyncOp);
            }
            else
                s_QueuedOperations.Enqueue(queueOperation);

            return queueOperation;
        }

        private static void OnWebAsyncOpComplete(AsyncOperation operation)
        {
            s_ActiveRequests.Remove((operation as UnityWebRequestAsyncOperation));

            if (s_QueuedOperations.Count > 0)
            {
                var nextQueuedOperation = s_QueuedOperations.Dequeue();
                var requestAsyncOp = nextQueuedOperation._webRequest.SendWebRequest();
                requestAsyncOp.completed += OnWebAsyncOpComplete;
                s_ActiveRequests.Add(requestAsyncOp);
                nextQueuedOperation.Complete(requestAsyncOp);
            }
        }
    }
}
