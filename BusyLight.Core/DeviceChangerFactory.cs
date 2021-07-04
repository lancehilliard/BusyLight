using BlinkStickDotNet;

namespace BusyLight.Core {
    public class DeviceChangerFactory {
        readonly IActivityChecker _activityChecker;
        public DeviceChangerFactory(IActivityChecker activityChecker) {
            _activityChecker = activityChecker;
        }

        public IDeviceChanger Create(BlinkStick device) {
            var isMicrophoneActive = _activityChecker.IsMicrophoneActive();
            var result = isMicrophoneActive ? (IDeviceChanger)new RedDeviceChanger(device) : new OffDeviceChanger(device);
            return result;
        }
    }
}