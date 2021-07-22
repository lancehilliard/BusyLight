using System.Drawing;
using BusyLight.Core;
using Moq;

namespace BusyLight.Specs {
    public class Specification<TSut> {
        protected TSut Sut;

        protected readonly Mock<ILightDevice> LightDeviceFake = new();
        protected readonly Mock<IActiveColorGetter> ActiveColorGetterFake = new();
        protected readonly Mock<IMicrophoneStatusChecker> MicrophoneStatusCheckerFake = new();
        protected readonly Mock<IActivityPublisher> ActivityPublisherFake = new();

        protected readonly Color ColorValue = Color.Red;
        protected readonly int AssumeMaxSecondsValue = 42;
    }

    public class Specification<TSut, TResult> : Specification<TSut> {
        protected TResult Result;
    }
}