namespace BusyLight.Core {
    public class DeviceChangerFactory {
        readonly IActivityChecker _activityChecker;
        readonly string _microphoneActiveColor;

        public DeviceChangerFactory(IActivityChecker activityChecker, string microphoneActiveColor) {
            _activityChecker = activityChecker;
            _microphoneActiveColor = microphoneActiveColor;
        }

        public IDeviceChanger Create(ILightDevice device) {
            var isMicrophoneActive = _activityChecker.IsMicrophoneActive();
            var result = isMicrophoneActive ? (IDeviceChanger)new OnDeviceChanger(device, _microphoneActiveColor) : new OffDeviceChanger(device);
            return result;
        }
    }
}