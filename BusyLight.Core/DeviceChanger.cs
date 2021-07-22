namespace BusyLight.Core {
    public interface IDeviceChanger {
        void Change();
    }

    public class OnDeviceChanger : IDeviceChanger {
        readonly ILightDevice _device;
        readonly IActiveColorGetter _activeColorGetter;


        public OnDeviceChanger(ILightDevice device, IActiveColorGetter activeColorGetter) {
            _device = device;
            _activeColorGetter = activeColorGetter;
        }

        public void Change() {
            var color = _activeColorGetter.Get();
            _device.SetQuadrantColor(color, Quadrant.First);
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