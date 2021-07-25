using System;
using System.Collections.Generic;
using System.Drawing;
using BlinkStickDotNet;

namespace BusyLight.Core {
    public interface ILightDevice : IDisposable {
        void SetQuadrantColor(Color color, Quadrant quadrant);
        void TurnOffQuadrant(Quadrant first);
        bool IsReady();
    }

    public class SquareLightDevice : QuadrantLightDevice, ILightDevice {
        static readonly Dictionary<Quadrant, byte[]> QuadrantIndexes = new() {
            {Quadrant.First, new byte[]{0,1}}
            , {Quadrant.Second, new byte[]{2,3}}
            , {Quadrant.Third, new byte[]{4,5}}
            , {Quadrant.Fourth, new byte[]{6,7}}
        };

        public SquareLightDevice(ILogger logger) : base(logger, QuadrantIndexes) { }
    }

    public class QuadrantLightDevice : LightDevice {
        readonly Dictionary<Quadrant, byte[]> _quadrantIndexes;

        protected QuadrantLightDevice(ILogger logger, Dictionary<Quadrant, byte[]> quadrantIndexes) : base(logger) {
            _quadrantIndexes = quadrantIndexes;
        }
        public void TurnOffQuadrant(Quadrant quadrant) {
            SetQuadrantColor(OffColor, quadrant);
        }
        
        public void SetQuadrantColor(Color color, Quadrant quadrant) {
            Do(x => {
                foreach (var index in _quadrantIndexes[quadrant]) {
                    x.SetColor(0, index, RgbColor.FromRgb(color.R, color.G, color.B));
                }
            });
        }
    }

    public class LightDevice : IDisposable {
        BlinkStick _device;
        readonly ILogger _logger;
        protected static readonly Color OffColor = Color.Black;

        protected LightDevice(ILogger logger) {
            _logger = logger;
        }

        BlinkStick Device {
            get {
                if (_device is null || !_device.Connected) {
                    _device = BlinkStick.FindFirst();
                    _device?.OpenDevice();
                }
                return _device;
            }
        }

        public bool IsReady() => Device is {Connected: true};

        protected void Do(Action<BlinkStick> action) {
            try {
                action(Device);
            }
            catch (Exception e) {
                _logger.Log($"BlinkStick error. Message: {e.Message}; Trace: {e.StackTrace}");
                _device?.CloseDevice();
                _device = null;
            }
        }

        public void Dispose() {
            _device?.CloseDevice();
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
