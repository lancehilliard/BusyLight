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
            foreach (var registryKey in registryKeys.Where(registryKey => !result)) {
                using (var rootSubKey = registryKey.OpenSubKey(RootSubKey))
                {
                    if (rootSubKey != null)
                    {
                        var subKeyNames = rootSubKey.GetSubKeyNames();
                        foreach (var subKeyName in subKeyNames)
                        {
                            var subKeyPath = $@"{RootSubKey}\{subKeyName}";
                            using (var subKey = Registry.CurrentUser.OpenSubKey(subKeyPath)) {
                                if (subKey != null) {
                                    var lastUsedFileTimeStop = Convert.ToInt64(subKey.GetValue("LastUsedTimeStop"));
                                    result = default(long).Equals(lastUsedFileTimeStop);
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