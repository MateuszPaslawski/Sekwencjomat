﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPF_Sekwencjomat.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string VLC_DLL_PATH {
            get {
                return ((string)(this["VLC_DLL_PATH"]));
            }
            set {
                this["VLC_DLL_PATH"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100,100,800,450")]
        public global::System.Windows.Rect WINDOW_LOCATION {
            get {
                return ((global::System.Windows.Rect)(this["WINDOW_LOCATION"]));
            }
            set {
                this["WINDOW_LOCATION"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Normal")]
        public global::System.Windows.WindowState WINDOW_STATE {
            get {
                return ((global::System.Windows.WindowState)(this["WINDOW_STATE"]));
            }
            set {
                this["WINDOW_STATE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string REFVIDEO_PATH {
            get {
                return ((string)(this["REFVIDEO_PATH"]));
            }
            set {
                this["REFVIDEO_PATH"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> LIST_OF_FILES {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["LIST_OF_FILES"]));
            }
            set {
                this["LIST_OF_FILES"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ACR")]
        public string PLAYBACK_TECHNIQUE {
            get {
                return ((string)(this["PLAYBACK_TECHNIQUE"]));
            }
            set {
                this["PLAYBACK_TECHNIQUE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Random")]
        public string PLAYBACK_MODE {
            get {
                return ((string)(this["PLAYBACK_MODE"]));
            }
            set {
                this["PLAYBACK_MODE"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int RATING_DELAY {
            get {
                return ((int)(this["RATING_DELAY"]));
            }
            set {
                this["RATING_DELAY"] = value;
            }
        }
    }
}
