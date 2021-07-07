namespace BusyLight.Core {
    public class MicrophoneActivityPublisher {
        readonly IMicrophoneStatusChecker _microphoneStatusChecker;
        readonly IActivityPublisher _activityPublisher;
        public MicrophoneActivityPublisher(IMicrophoneStatusChecker microphoneStatusChecker, IActivityPublisher activityPublisher) {
            _microphoneStatusChecker = microphoneStatusChecker;
            _activityPublisher = activityPublisher;
        }

        public bool PublishMicrophoneActivity() {
            var activityPublished = false;
            var microphoneIsBeingUsed = _microphoneStatusChecker.IsMicrophoneBeingUsed();
            if (microphoneIsBeingUsed) {
                _activityPublisher.PublishMicrophoneUse();
                activityPublished = true;
            }
            return activityPublished;
        }
    }
}