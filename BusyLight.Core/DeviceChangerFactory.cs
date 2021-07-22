using System;

namespace BusyLight.Core {
    public class DeviceChangerFactory {
        readonly ILightDevice _device;
        readonly IActiveColorGetter _activeColorGetter;
        readonly int _assumeMaxSeconds;

        public DeviceChangerFactory(ILightDevice device, IActiveColorGetter activeColorGetter, int assumeMaxSeconds) {
            _device = device;
            _activeColorGetter = activeColorGetter;
            _assumeMaxSeconds = assumeMaxSeconds;
        }

        public IDeviceChanger Create(TimeSpan timeSinceLastMessage) {
            var deviceShouldBeOn = timeSinceLastMessage.TotalSeconds <= _assumeMaxSeconds;
            var result = deviceShouldBeOn ? (IDeviceChanger)new OnDeviceChanger(_device, _activeColorGetter) : new OffDeviceChanger(_device);
            return result;
        }
    }
}