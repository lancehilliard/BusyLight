using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace BusyLight.Core {
    public interface IMicrophoneStatusChecker {
        bool IsMicrophoneBeingUsed();
    }

    public class MicrophoneRegistryStatusChecker : IMicrophoneStatusChecker {
        static readonly string RootSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone";
        public bool IsMicrophoneBeingUsed() {
            var result = false;
            var registryKeys = new List<RegistryKey>{Registry.LocalMachine,Registry.CurrentUser};
            foreach (var registryKey in registryKeys.Where(_ => !result)) {
                result = IsMicrophoneBeingUsedByChildren(registryKey, RootSubKey);
            }
            return result;
        }

        bool IsMicrophoneBeingUsedByChildren(RegistryKey registryKey, string parentSubKeyName) {
            var result = false;
            using (var parentSubKey = registryKey.OpenSubKey(parentSubKeyName, RegistryKeyPermissionCheck.ReadSubTree))
            {
                if (parentSubKey != null)
                {
                    var childSubKeyNames = parentSubKey.GetSubKeyNames();
                    foreach (var childSubKeyName in childSubKeyNames.Where(_=>!result))
                    {
                        var childSubKeyPath = $@"{parentSubKeyName}\{childSubKeyName}";
                        using (var childSubKey = Registry.CurrentUser.OpenSubKey(childSubKeyPath, RegistryKeyPermissionCheck.ReadSubTree)) {
                            if (childSubKey != null) {
                                var grandChildSubKeyNames = childSubKey.GetSubKeyNames();
                                if (grandChildSubKeyNames.Any()) {
                                    result = IsMicrophoneBeingUsedByChildren(registryKey, childSubKeyPath);
                                }
                                else {
                                    var value = childSubKey.GetValue("LastUsedTimeStop");
                                    result = value != null && default(long).Equals(Convert.ToInt64(value));
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}