using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusyLight.Core;

namespace BusyLight.Specs {
    public class OnDeviceChangerSpecs : Specification<OnDeviceChanger> {
        protected OnDeviceChangerSpecs() {
            ConfigFake.Setup(x => x.ActiveColor).Returns(ColorValue);
            Sut = new OnDeviceChanger(LightDeviceFake.Object, ConfigFake.Object);
        }

        [TestClass]
        public class WhenOnDeviceChangerChangesTheDevice : OnDeviceChangerSpecs {
            public WhenOnDeviceChangerChangesTheDevice() {
                LightDeviceFake.Setup(x => x.IsReady()).Returns(true);
                Sut.Change();
            }

            [TestMethod]
            public void ShouldSetFirstQuadrantToGivenColor() => LightDeviceFake.Verify(x => x.SetQuadrantColor(ColorValue, Quadrant.First));
        }
    }
}
