using System;
using System.Configuration;
using System.Drawing;

namespace BusyLight.Core {
    public interface IActiveColorGetter {
        public Color Get();
    }

    public class ActiveColorGetter : IActiveColorGetter {
        readonly Configuration _config;
        public ActiveColorGetter(Configuration config) {
            _config = config;
        }

        public Color Get() {
            Color result;
            try {
                var appSettings = _config.AppSettings.Settings;
                var activeColor = appSettings["ActiveColor"].Value;
                result = ColorTranslator.FromHtml(activeColor);
            }
            catch (Exception) {
                result = ColorTranslator.FromHtml("#FF0000");
            }
            return result;
        }
    }
}