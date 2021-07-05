using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using BlinkStickDotNet;

namespace July4th {
    static class Program {
        static readonly int MaxFactor = 10;
        [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Function is intended to run infinitely.")]
        static void Main() {
            var r = new Random();
            var factor = MaxFactor;
            while (true) {
                var device = BlinkStick.FindFirst();
                if (device.OpenDevice()) {
                    int index;
                    for (var i = 0; i < factor*2; i++) {
                        Console.Write($"{factor} ");
                        index = r.Next(0, 6);
                        device.SetColor(0, (byte)index, "red");
                        Thread.Sleep(20*r.Next(0,5));
                        device.SetColor(0, (byte)(index+1), "white");
                        Thread.Sleep(20*r.Next(0,5));
                        device.SetColor(0, (byte)(index+2), "blue");
                        device.SetColors(0, new List<byte>{0,0,0,0,0,0,0,0}.ToArray());
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    index = r.Next(0, 6);
                    device.SetColor(0, (byte)index, "white");
                    device.SetColor(0, (byte)(index+1), "white");
                    device.SetColor(0, (byte)(index+2), "white");
                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                    device.SetColors(0, new List<byte>{0,0,0,0,0,0,0,0}.ToArray());
                    device.CloseDevice();
                    factor = r.Next(1, MaxFactor);
                    Thread.Sleep(factor * 500);
                }
            }
        }
    }
}
