using System;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;

namespace Windows_OEM_Editor
{
    public class RegistryHelpers
    {
        private readonly RegistryKey _regKey;
        public RegistryHelpers()
        {
            _regKey = GetRegistryKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation");

            Debug.WriteLine(_regKey.Name);
            Debug.WriteLine(_regKey.ValueCount);
        }

        public string GetValue(string key, string @default)
        {
            var v = _regKey.GetValue(key);

            if(v == null) return @default;

            return (string) v;
        }

        public void SetValue(string key, string value)
        {
            _regKey.SetValue(
                key, value, RegistryValueKind.String);
        }

        public RegistryKey GetRegistryKey(string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                          Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath, true);
        }
    }
}
