﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AlertTrader.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.0.1.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int timerMinutesToCheckEmail {
            get {
                return ((int)(this["timerMinutesToCheckEmail"]));
            }
            set {
                this["timerMinutesToCheckEmail"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("toto@titi.com")]
        public string email {
            get {
                return ((string)(this["email"]));
            }
            set {
                this["email"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Bonjour")]
        public string password {
            get {
                return ((string)(this["password"]));
            }
            set {
                this["password"] = value;
            }
        }

        public string BitfinexApiKey { get; internal set; }
        public string OneBrokerApiToken { get; internal set; }
        public string BittrexApiKey { get; internal set; }
        public string BittrexApiSecret { get; internal set; }
        public string BitfinexApiSecret { get; internal set; }
        public string BaseCurrency { get; internal set; }
        public string Market { get; internal set; }
        public bool UsingFixedAmmount { get; internal set; }
        public object FixedAmmount { get; internal set; }
        public decimal CapitalPercentageInEachOrder { get; internal set; }
        public int LimitSpreadPercentage { get; internal set; }
        public string Leverage { get; internal set; }
        public string PoloniexApiSecret { get; internal set; }
        public string PoloniexApiKey { get; internal set; }
        public string KrakenApiKey { get; internal set; }
        public string KrakenApiSecret { get; internal set; }
    }
}
