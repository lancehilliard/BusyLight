using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace BusyLight.Core {
    public interface IActivityChecker {
        bool IsMicrophoneActive();
    }

    public class ActivityChecker : IActivityChecker {
        readonly string _databaseApiKey;
        public ActivityChecker(string databaseApiKey) {
            _databaseApiKey = databaseApiKey;
        }

        public bool IsMicrophoneActive() {
            var client = new RestClient("https://busylight-b8d1.restdb.io/rest/activities");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("x-apikey", _databaseApiKey);
            request.AddHeader("content-type", "application/json");
            var response = client.Get<List<Activity>>(request);
            var activity = response.Data.Single();
            var secondsSinceActivity = (DateTime.Now - activity.When).TotalSeconds;
            var result = secondsSinceActivity < 10;
            return result;
        }

        class Activity {
            public string Name { get; set; }
            public DateTime When { get; set; }
        }
    }
}