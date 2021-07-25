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
            var result = DeviceState.On;
            if (_device.IsReady()) {
                _device.SetQuadrantColor(_config.ActiveColor, Quadrant.First);
                result = DeviceState.WouldBeOn;
            }
            return result;
        }
    }

    public class OffDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        public OffDeviceChanger(ILightDevice device) {
            _device = device;
        }

        public DeviceState Change() {
            var result = DeviceState.Off;
            if (_device.IsReady()) {
                _device.TurnOffQuadrant(Quadrant.First);
                result = DeviceState.WouldBeOff;
            }
            return result;
        }
    }
}