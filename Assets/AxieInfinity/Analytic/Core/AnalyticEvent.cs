using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Analytic
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EventTypes
    {
        [EnumMember(Value = "unknown")] Unknown,
        [EnumMember(Value = "identify")] Identify,
        [EnumMember(Value = "screen")] Screen,
        [EnumMember(Value = "track")] Track,
    }

    [Serializable]
    public class AnalyticEvent
    {
        public EventTypes type;
        public JObject data;


        public AnalyticEvent(EventTypes type, object data)
        {
            this.type = type;

            if(data is JObject)
            {
                this.data = data as JObject;
            }
            else
            {
                this.data = JObject.Parse(JsonConvert.SerializeObject(data));
            }
       
            if (!this.data.ContainsKey("event"))
            {
                this.data.Add(new JProperty("event", type.ToString().ToLower()));
            }
            switch (type)
            {
                case EventTypes.Identify:
                    break;
                case EventTypes.Screen:
                    if(this.data["screen"] == null) this.data["screen"] = this.data["event"];
                    break;
                case EventTypes.Track:
                    if(this.data["action"] == null) this.data["action"] = this.data["event"];
                    break;
            }
        }

        public void MergeData(object otherData)
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(data));
            var jObjectOther = JObject.Parse(JsonConvert.SerializeObject(otherData));
            jObject.Merge(jObjectOther);
            data = jObject;
        }
    }

    public class AnalyticRequest
    {
        public List<AnalyticEvent> events;

        public AnalyticRequest(List<AnalyticEvent> listEvents)
        {
            events = listEvents;
        }
    }

    public class AnalyticUtils
    {
        public static DateTime FromUnixTimeMS(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        public static long ToUnixTimeMS(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        public static long ToUnixTimeS(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static DateTime FromUnixTimeS(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static long GetUtcNowMS()
        {
            return ToUnixTimeMS(DateTime.UtcNow);
        }
    }

    public class HeartbeatStats 
    {
        [JsonProperty("last_beat_time")]
        public long lastBeatTimestamp;

        public long timestamp => AnalyticUtils.GetUtcNowMS();

        [JsonProperty("total_pressed")]
        public int totalPressed;

        [JsonProperty("last_checksum_timestamp")]
        public long lastChecksumTimestamp;

        [JsonProperty("summary_hash")]
        public string summaryHash;

        public void Init()
        {
            lastBeatTimestamp = AnalyticUtils.GetUtcNowMS();
            totalPressed = 0;
            lastChecksumTimestamp = 0;
            summaryHash = "";
        }

        public void OnNewHash(string summaryHash)
        {
            this.summaryHash = summaryHash;
            lastChecksumTimestamp = timestamp;
        }

        public void OnNewPressed()
        {
            totalPressed++;
        }

        public void EndBeat()
        {
            lastBeatTimestamp = timestamp;
            totalPressed = 0;
        }
    }
}
