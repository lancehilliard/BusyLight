using System;

namespace BusyLight.Core {
    public class DeviceChangerFactory {
        readonly ILightDevice _device;
        readonly string _deviceOnColor;
        readonly int _assumeMaxSeconds;

        public DeviceChangerFactory(ILightDevice device, string deviceOnColor, int assumeMaxSeconds) {
            _device = device;
            _deviceOnColor = deviceOnColor;
            _assumeMaxSeconds = assumeMaxSeconds;
        }

        public IDeviceChanger Create(TimeSpan timeSinceLastMessage) {
            var deviceShouldBeOn = timeSinceLastMessage.TotalSeconds <= _assumeMaxSeconds;
            var result = deviceShouldBeOn ? (IDeviceChanger)new OnDeviceChanger(_device, _deviceOnColor) : new OffDeviceChanger(_device);
            return result;
        }
    }
}