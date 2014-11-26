using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ProtoBuild.XBDMarkup;

namespace ProtoBuild.Types
{
    public class RegistryHelper
    {
    }
    internal class RegistryItem
    {
        private string _registryPath;
        private RegistryKey _regRoot;
        private RegistryKey _regKey;
        private IVariableReplacer _replacer;
        private bool _includeDefined;

        public object GetValue(string keyName)
        {
           
            if (IsNull(keyName))
                return null;
            return _regKey.GetValue(keyName);
        }
        public RegistryValueKind GetKind(string keyName)
        {
            return _regKey.GetValueKind(keyName);
        }
        public bool IsNull()
        {
            
            return _regKey == null;
        }
        public bool IsNull(string keyName)
        {

            return _regKey == null || _regKey.GetValue(keyName) == null;
        }
        public String RegistryPath
        {
            get
            {
                return _replacer.ReplaceVariables(_registryPath, 0, _includeDefined);
            }
        }
        public RegistryItem(string value, IVariableReplacer replacer, bool includeDefined = true)
        {
            this._includeDefined = includeDefined;
            this._replacer = replacer;
            string firstElement = value.Split(new char[] { '\\' })[0];
            switch (firstElement.ToUpper())
            {
                
                case "HKCU":
                case "HKEY_CURRENT_USER":
                    _regRoot = Registry.CurrentUser;
                    _registryPath = value.Substring(value.IndexOf('\\')+1);
                    break;
                case "HKCR":
                case "HKEY_CLASSES_ROOT":
                    _regRoot = Registry.ClassesRoot;
                    _registryPath = value.Substring(value.IndexOf('\\')+1);
                    break;
                case "HKU":
                case "HKEY_USERS":
                    _regRoot = Registry.Users;
                    _registryPath = value.Substring(value.IndexOf('\\')+1);
                    break;
                case "HKLM":
                case "HKEY_LOCAL_MACHINE":
                default:
                    _regRoot = Registry.LocalMachine;
                    _registryPath = value.Substring(value.IndexOf('\\') + 1);
                    break;
            }
            _regKey = _regRoot.OpenSubKey(_registryPath);
        }
    }
}
