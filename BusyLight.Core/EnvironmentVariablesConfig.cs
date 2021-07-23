using System;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace BusyLight.Core {
    public interface IConfig {
        string MessageQueueUrl { get; }
        int PublishIntervalSeconds { get; }
        Color ActiveColor { get; }
        int AssumeMaxSeconds { get; }
    }

    public abstract class Config : IConfig {
        public string MessageQueueUrl => Get();

        public int PublishIntervalSeconds {
            get {
                int result = int.TryParse(Get(), out result) ? result : 4;
                return result;
            }
        }

        public Color ActiveColor {
            get {
                Color result;
                try {
                    result = ColorTranslator.FromHtml(Get());
                }
                catch (Exception) {
                    result = ColorTranslator.FromHtml("#FF0000");
                }
                return result;
            }
        }

        public int AssumeMaxSeconds {
            get {
                int result = int.TryParse(Get(), out result) ? result : 15;
                return result;
            }
        }

        protected virtual string Get([CallerMemberName]string name = "") {
            throw new NotImplementedException();
        }
    }

    public class EnvironmentVariablesConfig : Config {
        protected override string Get([CallerMemberName]string name = "") => Environment.GetEnvironmentVariable(GetEnvironmentVariableFullName(name));
        string GetEnvironmentVariableFullName(string name) => $"{Constants.ProductName}.{name}";
    }
}