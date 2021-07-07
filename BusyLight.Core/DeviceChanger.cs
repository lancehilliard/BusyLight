using BlinkStickDotNet;

namespace BusyLight.Core {
    public interface IDeviceChanger {
        void Change();
    }

    public class OnDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        readonly string _color;

        public OnDeviceChanger(ILightDevice device, string color) {
            _device = device;
            _color = color;
        }

        public void Change() {
            _device.SetQuadrantColor(_color, Quadrant.First);
        }
    }

    public class OffDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        public OffDeviceChanger(ILightDevice device) {
            _device = device;
        }

        public void Change() {
            _device.TurnOffQuadrant(Quadrant.First);
        }
    }
}