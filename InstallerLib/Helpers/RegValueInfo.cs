using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Helpers
{
    class RegValueInfo
    {
        public RegistryValueKind RegistryValueKind { get; }
        public string Name { get; }
        public object Value { get; }

        private RegValueInfo()
        {

        }

        private RegValueInfo(string name, object value, RegistryValueKind registryValueKind)
        {
            Name = name;
            Value = value;
            RegistryValueKind = registryValueKind;
        }

        public static IEnumerable<RegValueInfo> Get(RegistryKey key)
        {
            var res = new List<RegValueInfo>();
            return key.GetValueNames().Select(x => new RegValueInfo(x, key.GetValue(x), key.GetValueKind(x))).ToList();
        }

        public static void Set(IEnumerable<RegValueInfo> info, RegistryKey key)
        {
            foreach (var valInfo in info)
                key.SetValue(valInfo.Name, valInfo.Value, valInfo.RegistryValueKind);
        }
    }

}
