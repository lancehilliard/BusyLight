using System;
using RestSharp;

namespace BusyLight.Core {
    public class ActivityLogger {
        readonly string _databaseApiKey;
        readonly string _restBaseUrl;
        readonly string _microphoneActivityRecordId;

        public ActivityLogger(string databaseApiKey, string restBaseUrl, string microphoneActivityRecordId) {
            _databaseApiKey = databaseApiKey;
            _restBaseUrl = restBaseUrl;
            _microphoneActivityRecordId = microphoneActivityRecordId;
        }

        public void LogMicrophoneUse() {
            var updateClient = new RestClient($"{_restBaseUrl}/activities/{_microphoneActivityRecordId}");
            var updateRequest = new RestRequest(Method.PUT);
            updateRequest.AddHeader("cache-control", "no-cache");
            updateRequest.AddHeader("x-apikey", _databaseApiKey);
            updateRequest.AddHeader("content-type", "application/json");
            updateRequest.AddParameter("application/json", $@"{{""name"":""microphone"",""when"":""{DateTime.UtcNow}""}}", ParameterType.RequestBody);
            updateClient.Execute(updateRequest);
        }
    }
}