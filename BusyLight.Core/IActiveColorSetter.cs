using System.Drawing;

namespace BusyLight.Core {
    public interface IActiveColorSetter {
        public void Set(Color color);
    }

    public class ActiveColorSetter : IActiveColorSetter {
        readonly IAppSettingUpdater _appSettingUpdater;
        public ActiveColorSetter(IAppSettingUpdater appSettingUpdater) {
            _appSettingUpdater = appSettingUpdater;
        }

        public void Set(Color color) {
            _appSettingUpdater.Update("ActiveColor", ColorTranslator.ToHtml(color));
        }
    }
}