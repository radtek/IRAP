﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace IRAP.WebService.Client.WS_IRAPUES {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="IRAPUESBaseSoap", Namespace="http://tempuri.org/")]
    public partial class IRAPUESBase : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback IRAPUESOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetXMLSchemasOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public IRAPUESBase() {
            this.Url = global::IRAP.WebService.Client.Properties.Settings.Default.IRAP_WebService_Client_WS_IRAPUES_IRAPUESBase;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event IRAPUESCompletedEventHandler IRAPUESCompleted;
        
        /// <remarks/>
        public event GetXMLSchemasCompletedEventHandler GetXMLSchemasCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IRAPUES", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string IRAPUES(string ExCode) {
            object[] results = this.Invoke("IRAPUES", new object[] {
                        ExCode});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void IRAPUESAsync(string ExCode) {
            this.IRAPUESAsync(ExCode, null);
        }
        
        /// <remarks/>
        public void IRAPUESAsync(string ExCode, object userState) {
            if ((this.IRAPUESOperationCompleted == null)) {
                this.IRAPUESOperationCompleted = new System.Threading.SendOrPostCallback(this.OnIRAPUESOperationCompleted);
            }
            this.InvokeAsync("IRAPUES", new object[] {
                        ExCode}, this.IRAPUESOperationCompleted, userState);
        }
        
        private void OnIRAPUESOperationCompleted(object arg) {
            if ((this.IRAPUESCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.IRAPUESCompleted(this, new IRAPUESCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetXMLSchemas", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetXMLSchemas(string ExCode, string Category) {
            object[] results = this.Invoke("GetXMLSchemas", new object[] {
                        ExCode,
                        Category});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetXMLSchemasAsync(string ExCode, string Category) {
            this.GetXMLSchemasAsync(ExCode, Category, null);
        }
        
        /// <remarks/>
        public void GetXMLSchemasAsync(string ExCode, string Category, object userState) {
            if ((this.GetXMLSchemasOperationCompleted == null)) {
                this.GetXMLSchemasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetXMLSchemasOperationCompleted);
            }
            this.InvokeAsync("GetXMLSchemas", new object[] {
                        ExCode,
                        Category}, this.GetXMLSchemasOperationCompleted, userState);
        }
        
        private void OnGetXMLSchemasOperationCompleted(object arg) {
            if ((this.GetXMLSchemasCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetXMLSchemasCompleted(this, new GetXMLSchemasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void IRAPUESCompletedEventHandler(object sender, IRAPUESCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IRAPUESCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal IRAPUESCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void GetXMLSchemasCompletedEventHandler(object sender, GetXMLSchemasCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetXMLSchemasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetXMLSchemasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591