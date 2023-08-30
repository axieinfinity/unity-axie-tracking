using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Converters;

namespace Analytic
{
    [Serializable]
    public class AnalyticListData
    {
        public static string analyticListData = "analytic_data.json";

        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> {
                new StringEnumConverter()
            }
        };

        public string savePath;

        public List<AnalyticEvent> listEventDatas = new List<AnalyticEvent>();

        public AnalyticListData(string savePath)
        {
            this.savePath = savePath;
            this.listEventDatas = new List<AnalyticEvent>();
        }

        public void SaveToDevice()
        {
            var path = Path.Combine(Application.persistentDataPath, savePath);
            File.WriteAllText(path, JsonConvert.SerializeObject(this, SerializerSettings));
        }

        public static AnalyticListData LoadFromDevice()
        {
            try
            {
                var path = Path.Combine(Application.persistentDataPath, analyticListData);
                var json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<AnalyticListData>(json, SerializerSettings);

                // Double ensure ;)
                if (data != null && data.listEventDatas != null)
                    return data;
            }
            catch (Exception)
            {
                // Do nothing
            }

            return new AnalyticListData(analyticListData);
        }
    }
}
