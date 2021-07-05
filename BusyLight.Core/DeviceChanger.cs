using BlinkStickDotNet;

namespace BusyLight.Core {
    public interface IDeviceChanger {
        void Change();
    }

    public class RedDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        public RedDeviceChanger(ILightDevice device) {
            _device = device;
        }

        public void Change() {
            _device.SetQuadrantColor(RgbColor.FromString("red"), Quadrant.First);
        }
    }

    public class OffDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        public OffDeviceChanger(ILightDevice device) {
            _device = device;
        }

        public void Change() {
            _device.TurnOffAllLights();
        }
    }
}