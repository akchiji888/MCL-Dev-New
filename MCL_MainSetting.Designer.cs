﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MCL_Dev {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class MCL_MainSetting : global::System.Configuration.ApplicationSettingsBase {
        
        private static MCL_MainSetting defaultInstance = ((MCL_MainSetting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MCL_MainSetting())));
        
        public static MCL_MainSetting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int MaxMemory {
            get {
                return ((int)(this["MaxMemory"]));
            }
            set {
                this["MaxMemory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int JavaSelectedItemIndex {
            get {
                return ((int)(this["JavaSelectedItemIndex"]));
            }
            set {
                this["JavaSelectedItemIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("-1")]
        public int GameComboSelectedIndex {
            get {
                return ((int)(this["GameComboSelectedIndex"]));
            }
            set {
                this["GameComboSelectedIndex"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ZanZhuMingDan {
            get {
                return ((string)(this["ZanZhuMingDan"]));
            }
            set {
                this["ZanZhuMingDan"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string gameAccountsList {
            get {
                return ((string)(this["gameAccountsList"]));
            }
            set {
                this["gameAccountsList"] = value;
            }
        }
    }
}