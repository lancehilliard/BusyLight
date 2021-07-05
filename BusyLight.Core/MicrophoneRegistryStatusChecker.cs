using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace BusyLight.Core {
    public interface IMicrophoneStatusChecker {
        bool IsMicrophoneBeingUsed();
    }

    public class MicrophoneRegistryStatusChecker : IMicrophoneStatusChecker {
        static readonly string RootSubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged";
        public bool IsMicrophoneBeingUsed() {
            var result = false;
            var registryKeys = new List<RegistryKey>{Registry.LocalMachine,Registry.CurrentUser};
            foreach (var registryKey in registryKeys.Where(x => !result)) {
                using (var rootSubKey = registryKey.OpenSubKey(RootSubKey, RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if (rootSubKey != null)
                    {
                        var subKeyNames = rootSubKey.GetSubKeyNames();
                        foreach (var subKeyName in subKeyNames.Where(x=>!result))
                        {
                            var subKeyPath = $@"{RootSubKey}\{subKeyName}";
                            using (var subKey = Registry.CurrentUser.OpenSubKey(subKeyPath, RegistryKeyPermissionCheck.ReadSubTree)) {
                                if (subKey != null) {
                                    var value = subKey.GetValue("LastUsedTimeStop");
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