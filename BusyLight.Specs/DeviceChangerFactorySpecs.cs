using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusyLight.Core;

namespace BusyLight.Specs {
    public class DeviceChangerFactorySpecs : Specification<DeviceChangerFactory, IDeviceChanger> {
        protected DeviceChangerFactorySpecs() {
            Sut = new DeviceChangerFactory(LightDeviceFake.Object, ColorNameValue, AssumeMaxSecondsValue);
        }

        [TestClass]
        public class WhenTimeSinceLastMessageIsLessThanAssumeMaxSeconds : DeviceChangerFactorySpecs {
            public WhenTimeSinceLastMessageIsLessThanAssumeMaxSeconds() => Result = Sut.Create(TimeSpan.FromSeconds(AssumeMaxSecondsValue-1));

            [TestMethod]
            public void ShouldReturnImplementationThatTurnsOnTheDevice() => Assert.IsInstanceOfType(Result, typeof(OnDeviceChanger));
        }

        [TestClass]
        public class WhenTimeSinceLastMessageIsEqualToAssumeMaxSeconds : DeviceChangerFactorySpecs {
            public WhenTimeSinceLastMessageIsEqualToAssumeMaxSeconds() => Result = Sut.Create(TimeSpan.FromSeconds(AssumeMaxSecondsValue));

            [TestMethod]
            public void ShouldReturnImplementationThatTurnsOnTheDevice() => Assert.IsInstanceOfType(Result, typeof(OnDeviceChanger));
        }

        [TestClass]
        public class WhenTimeSinceLastMessageIsGreaterThanAssumeMaxSeconds : DeviceChangerFactorySpecs {
            public WhenTimeSinceLastMessageIsGreaterThanAssumeMaxSeconds() => Result = Sut.Create(TimeSpan.FromSeconds(AssumeMaxSecondsValue+1));

            [TestMethod]
            public void ShouldReturnImplementationThatTurnsOffTheDevice() => Assert.IsInstanceOfType(Result, typeof(OffDeviceChanger));
        }
    }
}
