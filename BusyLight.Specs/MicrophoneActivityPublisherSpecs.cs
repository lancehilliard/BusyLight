using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusyLight.Core;
using Moq;

namespace BusyLight.Specs {
    public class MicrophoneActivityPublisherSpecs : Specification<MicrophoneActivityPublisher, bool> {
        protected MicrophoneActivityPublisherSpecs() {
            Sut = new MicrophoneActivityPublisher(MicrophoneStatusCheckerFake.Object, ActivityPublisherFake.Object, LoggerFake.Object);
        }

        [TestClass]
        public class WhenMicrophoneIsBeingUsed : MicrophoneActivityPublisherSpecs {
            public WhenMicrophoneIsBeingUsed() {
                MicrophoneStatusCheckerFake.Setup(x => x.IsMicrophoneBeingUsed()).Returns(true);
                Result = Sut.PublishMicrophoneActivity();
            }

            [TestMethod]
            public void MicrophoneUseShouldGetPublished() => ActivityPublisherFake.Verify(x => x.PublishMicrophoneUse());

            [TestMethod]
            public void PublishShouldGetLogged() => LoggerFake.Verify(x => x.Log(Constants.ActivitySendMessage));

            [TestMethod]
            public void ShouldReturnThatActivityWasPublished() => Assert.AreEqual(true, Result);
        }

        [TestClass]
        public class WhenMicrophoneIsNotBeingUsed : MicrophoneActivityPublisherSpecs {
            public WhenMicrophoneIsNotBeingUsed() {
                MicrophoneStatusCheckerFake.Setup(x => x.IsMicrophoneBeingUsed()).Returns(false);
                Result = Sut.PublishMicrophoneActivity();
            }

            [TestMethod]
            public void MicrophoneUseShouldNotGetPublished() => ActivityPublisherFake.Verify(x => x.PublishMicrophoneUse(), Times.Never);

            [TestMethod]
            public void ShouldReturnThatNoActivityWasPublished() => Assert.AreEqual(false, Result);
        }
    }
}
