namespace BusyLight.Core {
    public interface IDeviceChanger {
        DeviceState Change();
    }

    public class OnDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        readonly IActiveColorGetter _activeColorGetter;


        public OnDeviceChanger(ILightDevice device, IActiveColorGetter activeColorGetter) {
            _device = device;
            _activeColorGetter = activeColorGetter;
        }

        public DeviceState Change() {
            var color = _activeColorGetter.Get();
            _device.SetQuadrantColor(color, Quadrant.First);
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