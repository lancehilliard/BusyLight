using System;
using RestSharp;

namespace BusyLight.Core {
    public class ActivityLogger {
        readonly string _databaseApiKey;
        public ActivityLogger(string databaseApiKey) {
            _databaseApiKey = databaseApiKey;
        }

        public void LogMicrophoneUse() {
            var updateClient = new RestClient("https://busylight-b8d1.restdb.io/rest/activities/60dfdff3a46667610000390b");
            var updateRequest = new RestRequest(Method.PUT);
            updateRequest.AddHeader("cache-control", "no-cache");
            updateRequest.AddHeader("x-apikey", _databaseApiKey);
            updateRequest.AddHeader("content-type", "application/json");
            updateRequest.AddParameter("application/json", $@"{{""name"":""microphone"",""when"":""{DateTime.UtcNow}""}}", ParameterType.RequestBody);
            updateClient.Execute(updateRequest);
        }
    }
}