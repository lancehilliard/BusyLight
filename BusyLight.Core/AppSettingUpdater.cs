using System.Configuration;

namespace BusyLight.Core {
    public interface IAppSettingUpdater {
        void Update(string key, string value);
    }

    public class AppSettingUpdater : IAppSettingUpdater {
        readonly Configuration _config;
        public AppSettingUpdater(Configuration config) {
            _config = config;
        }

        public void Update(string key, string value) {
            var appSettings = _config.AppSettings.Settings;
            appSettings.Remove(key);
            appSettings.Add(key, value);
            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(_config.AppSettings.SectionInformation.Name);
        }
    }
}