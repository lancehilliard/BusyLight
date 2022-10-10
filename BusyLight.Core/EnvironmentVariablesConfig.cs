using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BusyLight.Core {
    public interface IConfig {
        string MessageQueueUrl { get; }
        int PublishIntervalSeconds { get; }
        Color ActiveColor { get; }
        int AssumeMaxSeconds { get; }
        IEnumerable<string> MicrophoneRegistryKeyIgnores { get; }
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
                    var value = Get();
                    result = string.IsNullOrWhiteSpace(value) ? GetDefaultColor() : ColorTranslator.FromHtml(value);
                }
                catch (Exception) {
                    result = GetDefaultColor();
                }
                return result;

                Color GetDefaultColor() => ColorTranslator.FromHtml("#FF0000");
            }
        }

        public int AssumeMaxSeconds {
            get {
                int result = int.TryParse(Get(), out result) ? result : 10;
                return result;
            }
        }

        public IEnumerable<string> MicrophoneRegistryKeyIgnores { 
            get {
                var value = Get();
                var result = string.IsNullOrWhiteSpace(value) ? Enumerable.Empty<string>() : value.Split(';');
                return result;
            }
        }

        protected virtual string Get([CallerMemberName]string name = "") {throw new NotImplementedException();}
    }

    public class EnvironmentVariablesConfig : Config {
        protected override string Get([CallerMemberName]string name = "") => Environment.GetEnvironmentVariable(GetEnvironmentVariableFullName(name));
        string GetEnvironmentVariableFullName(string name) => $"{Constants.ProductName}.{name}";
    }
}