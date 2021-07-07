using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusyLight.Core;

namespace BusyLight.Specs {
    public class OnDeviceChangerSpecs : Specification<OnDeviceChanger> {
        protected OnDeviceChangerSpecs() => Sut = new OnDeviceChanger(LightDeviceFake.Object, ColorNameValue);

        [TestClass]
        public class WhenOnDeviceChangerChangesTheDevice : OnDeviceChangerSpecs {
            public WhenOnDeviceChangerChangesTheDevice() => Sut.Change();

            [TestMethod]
            public void ShouldSetFirstQuadrantToGivenColor() => LightDeviceFake.Verify(x => x.SetQuadrantColor(ColorNameValue, Quadrant.First));
        }
    }
}
