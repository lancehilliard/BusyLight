using System;
using System.Collections.Generic;
using BlinkStickDotNet;

namespace BusyLight.Core {
    public interface ILightDevice : IDisposable {
        void TurnOffAllLights();
        void SetQuadrantColor(RgbColor color, Quadrant quadrant);
    }

    public class SquareLightDevice : LightDevice, ILightDevice {
        static readonly Dictionary<Quadrant, byte[]> QuadrantIndexes = new Dictionary<Quadrant, byte[]> {
            {Quadrant.First, new byte[]{0,1}}
            , {Quadrant.Second, new byte[]{2,3}}
            , {Quadrant.Third, new byte[]{4,5}}
            , {Quadrant.Fourth, new byte[]{6,7}}
        };
        public SquareLightDevice(BlinkStick device) : base(device) {
        }

        public void TurnOffAllLights() {
            Do(x => x.SetColors(0, new List<byte>{0,0,0,0,0,0,0,0}.ToArray()));
        }

        public void SetQuadrantColor(RgbColor color, Quadrant quadrant) {
            Do(x => {
                foreach (var index in QuadrantIndexes[quadrant]) {
                    x.SetColor(0, index, color);
                }
            });
        }
    }

    public class LightDevice : IDisposable {
        readonly BlinkStick _device;

        protected LightDevice(BlinkStick device) {
            _device = device;
        }

        protected void Do(Action<BlinkStick> action) {
            if (_device.OpenDevice()) {
                action(_device);
                _device.CloseDevice();
            }
            else {
                throw new NotSupportedException("Light device not available.");
            }
        }

        public void Dispose() {
            _device?.Dispose();
        }
    }

    public enum Quadrant {
        First,
        Second,
        Third,
        Fourth
    }
}
