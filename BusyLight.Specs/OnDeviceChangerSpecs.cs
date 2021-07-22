using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusyLight.Core;

namespace BusyLight.Specs {
    public class OnDeviceChangerSpecs : Specification<OnDeviceChanger> {
        protected OnDeviceChangerSpecs() {
            ActiveColorGetterFake.Setup(x => x.Get()).Returns(ColorValue);
            Sut = new OnDeviceChanger(LightDeviceFake.Object, ActiveColorGetterFake.Object);
        }

        [TestClass]
        public class WhenOnDeviceChangerChangesTheDevice : OnDeviceChangerSpecs {
            public WhenOnDeviceChangerChangesTheDevice() => Sut.Change();

            [TestMethod]
            public void ShouldSetFirstQuadrantToGivenColor() => LightDeviceFake.Verify(x => x.SetQuadrantColor(ColorValue, Quadrant.First));
        }
    }
}
