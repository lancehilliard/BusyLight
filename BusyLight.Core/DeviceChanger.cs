namespace BusyLight.Core {
    public interface IDeviceChanger {
        DeviceState Change();
    }

    public class OnDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        readonly IConfig _config;

        public OnDeviceChanger(ILightDevice device, IConfig config) {
            _device = device;
            _config = config;
        }

        public DeviceState Change() {
            _device.SetQuadrantColor(_config.ActiveColor, Quadrant.First);
            return DeviceState.On;
        }
    }

    public class OffDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        public OffDeviceChanger(ILightDevice device) {
            _device = device;
        }

        public DeviceState Change() {
            _device.TurnOffQuadrant(Quadrant.First);
            return DeviceState.Off;
        }
    }
}