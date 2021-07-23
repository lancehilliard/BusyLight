namespace BusyLight.Core {
    public class MicrophoneActivityPublisher {
        readonly IMicrophoneStatusChecker _microphoneStatusChecker;
        readonly IActivityPublisher _activityPublisher;
        readonly ILogger _logger;
        public MicrophoneActivityPublisher(IMicrophoneStatusChecker microphoneStatusChecker, IActivityPublisher activityPublisher, ILogger logger) {
            _microphoneStatusChecker = microphoneStatusChecker;
            _activityPublisher = activityPublisher;
            _logger = logger;
        }

        public bool PublishMicrophoneActivity() {
            var activityPublished = false;
            var microphoneIsBeingUsed = _microphoneStatusChecker.IsMicrophoneBeingUsed();
            if (microphoneIsBeingUsed) {
                _logger.Log(Constants.ActivitySendMessage);
                _activityPublisher.PublishMicrophoneUse();
                activityPublished = true;
            }
            return activityPublished;
        }
    }
}