using BlinkStickDotNet;

namespace BusyLight.Core {
    public interface IDeviceChanger {
        void Change();
    }

    public class RedDeviceChanger : DeviceChanger {
        public RedDeviceChanger(BlinkStick device) : base(device) {}

        protected override void ChangeDevice(BlinkStick device) {
            device.SetColor(RgbColor.FromString("red"));
        }
    }

    public class OffDeviceChanger : DeviceChanger {
        public OffDeviceChanger(BlinkStick device) : base(device) {}

        protected override void ChangeDevice(BlinkStick device) {
            device.TurnOff();
        }
    }

    public abstract class DeviceChanger : IDeviceChanger {
        readonly BlinkStick _device;
        protected DeviceChanger(BlinkStick device) {
            _device = device;
        }

        public void Change() {
            if (_device != null) {
                if (_device.OpenDevice()) {
                    ChangeDevice(_device);
                    _device.CloseDevice();
                }
            }
        }

        protected virtual void ChangeDevice(BlinkStick device) {}
    }
}