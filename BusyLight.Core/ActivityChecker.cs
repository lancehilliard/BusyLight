using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RestSharp;

namespace BusyLight.Core {
    public interface IActivityChecker {
        bool IsMicrophoneActive();
    }

    public class ActivityChecker : IActivityChecker {
        readonly string _databaseApiKey;
        readonly string _restBaseUrl;

        public ActivityChecker(string databaseApiKey, string restBaseUrl) {
            _databaseApiKey = databaseApiKey;
            _restBaseUrl = restBaseUrl;
        }

        public bool IsMicrophoneActive() {
            var url = $@"{_restBaseUrl}/activities?q={{""name"":""microphone""}}";
            var client = new RestClient(url);
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

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Class is instantiated during deserialization.")]
        class Activity {
            [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Value is set during deserialization.")]
            public DateTime When { get; set; }
        }
    }
}