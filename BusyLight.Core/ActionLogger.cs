using System;

namespace BusyLight.Core {
    public interface ILogger {
        void Log(string message);
    }

    public class ActionLogger : ILogger {
        readonly Action<string> _loggingAction;
        public ActionLogger(Action<string> loggingAction) {
            _loggingAction = loggingAction;
        }

        public void Log(string message) {
            _loggingAction(message);
        }
    }
}