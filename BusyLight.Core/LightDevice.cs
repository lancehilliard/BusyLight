using System;
using System.Collections.Generic;
using BlinkStickDotNet;

namespace BusyLight.Core {
    public interface ILightDevice : IDisposable {
        void SetQuadrantColor(string color, Quadrant quadrant);
        void TurnOffQuadrant(Quadrant first);
    }

    public class SquareLightDevice : LightDevice, ILightDevice {
        static readonly Dictionary<Quadrant, byte[]> QuadrantIndexes = new() {
            {Quadrant.First, new byte[]{0,1}}
            , {Quadrant.Second, new byte[]{2,3}}
            , {Quadrant.Third, new byte[]{4,5}}
            , {Quadrant.Fourth, new byte[]{6,7}}
        };
        public SquareLightDevice(BlinkStick device) : base(device) {
        }

        public void TurnOffQuadrant(Quadrant quadrant) {
            SetQuadrantColor("black", quadrant);
        }

        public void SetQuadrantColor(string color, Quadrant quadrant) {
            Do(x => {
                foreach (var index in QuadrantIndexes[quadrant]) {
                    x.SetColor(0, index, RgbColor.FromString(color));
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
