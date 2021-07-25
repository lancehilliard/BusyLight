using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusyLight.Core;

namespace BusyLight.Specs {
    public class OffDeviceChangerSpecs : Specification<OffDeviceChanger> {
        protected OffDeviceChangerSpecs() {
            Sut = new OffDeviceChanger(LightDeviceFake.Object);
        }

        [TestClass]
        public class WhenOffDeviceChangerChangesTheDevice : OffDeviceChangerSpecs {
            public WhenOffDeviceChangerChangesTheDevice() {
                LightDeviceFake.Setup(x => x.IsReady()).Returns(true);
                Sut.Change();
            }

            [TestMethod]
            public void ShouldTurnOffFirstQuadrant() => LightDeviceFake.Verify(x => x.TurnOffQuadrant(Quadrant.First));
        }
    }
}
