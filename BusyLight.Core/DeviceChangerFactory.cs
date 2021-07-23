using System;

namespace BusyLight.Core {
    public class DeviceChangerFactory {
        readonly ILightDevice _device;
        readonly IConfig _config;

        public DeviceChangerFactory(ILightDevice device, IConfig config) {
            _device = device;
            _config = config;
        }

        public IDeviceChanger Create(TimeSpan timeSinceLastMessage) {
            var deviceShouldBeOn = timeSinceLastMessage.TotalSeconds <= _config.AssumeMaxSeconds;
            var result = deviceShouldBeOn ? (IDeviceChanger)new OnDeviceChanger(_device, _config) : new OffDeviceChanger(_device);
            return result;
        }
    }
}