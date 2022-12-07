﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HopDongDK
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="HopDongDK.ZWS_CRM03")]
    public interface ZWS_CRM03
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_HOPDONGDAKY is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM03:ZCRM_HOPDONGDAKYRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        HopDongDK.ZCRM_HOPDONGDAKYResponse1 ZCRM_HOPDONGDAKY(HopDongDK.ZCRM_HOPDONGDAKYRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM03:ZCRM_HOPDONGDAKYRequest", ReplyAction="*")]
        System.Threading.Tasks.Task<HopDongDK.ZCRM_HOPDONGDAKYResponse1> ZCRM_HOPDONGDAKYAsync(HopDongDK.ZCRM_HOPDONGDAKYRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_HOPDONGDAKY1
    {
        
        private string fROM_PERIODField;
        
        private string pROFILEFOREIGNCODEField;
        
        private string sALESORGField;
        
        private string tO_PERIODField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string FROM_PERIOD
        {
            get
            {
                return this.fROM_PERIODField;
            }
            set
            {
                this.fROM_PERIODField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string PROFILEFOREIGNCODE
        {
            get
            {
                return this.pROFILEFOREIGNCODEField;
            }
            set
            {
                this.pROFILEFOREIGNCODEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string SALESORG
        {
            get
            {
                return this.sALESORGField;
            }
            set
            {
                this.sALESORGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string TO_PERIOD
        {
            get
            {
                return this.tO_PERIODField;
            }
            set
            {
                this.tO_PERIODField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_HOPDONGDAKY
    {
        
        private string sALESORGField;
        
        private string pERIODField;
        
        private string pROFILEFOREIGNCODEField;
        
        private int sOHOPDONGField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string SALESORG
        {
            get
            {
                return this.sALESORGField;
            }
            set
            {
                this.sALESORGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string PERIOD
        {
            get
            {
                return this.pERIODField;
            }
            set
            {
                this.pERIODField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string PROFILEFOREIGNCODE
        {
            get
            {
                return this.pROFILEFOREIGNCODEField;
            }
            set
            {
                this.pROFILEFOREIGNCODEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public int SOHOPDONG
        {
            get
            {
                return this.sOHOPDONGField;
            }
            set
            {
                this.sOHOPDONGField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_HOPDONGDAKYResponse
    {
        
        private ZCRM_HOPDONGDAKY[] hOPDONGDAKY_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_HOPDONGDAKY[] HOPDONGDAKY_DATAS
        {
            get
            {
                return this.hOPDONGDAKY_DATASField;
            }
            set
            {
                this.hOPDONGDAKY_DATASField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_HOPDONGDAKYRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public HopDongDK.ZCRM_HOPDONGDAKY1 ZCRM_HOPDONGDAKY;
        
        public ZCRM_HOPDONGDAKYRequest()
        {
        }
        
        public ZCRM_HOPDONGDAKYRequest(HopDongDK.ZCRM_HOPDONGDAKY1 ZCRM_HOPDONGDAKY)
        {
            this.ZCRM_HOPDONGDAKY = ZCRM_HOPDONGDAKY;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_HOPDONGDAKYResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public HopDongDK.ZCRM_HOPDONGDAKYResponse ZCRM_HOPDONGDAKYResponse;
        
        public ZCRM_HOPDONGDAKYResponse1()
        {
        }
        
        public ZCRM_HOPDONGDAKYResponse1(HopDongDK.ZCRM_HOPDONGDAKYResponse ZCRM_HOPDONGDAKYResponse)
        {
            this.ZCRM_HOPDONGDAKYResponse = ZCRM_HOPDONGDAKYResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface ZWS_CRM03Channel : HopDongDK.ZWS_CRM03, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class ZWS_CRM03Client : System.ServiceModel.ClientBase<HopDongDK.ZWS_CRM03>, HopDongDK.ZWS_CRM03
    {
        
        public ZWS_CRM03Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        HopDongDK.ZCRM_HOPDONGDAKYResponse1 HopDongDK.ZWS_CRM03.ZCRM_HOPDONGDAKY(HopDongDK.ZCRM_HOPDONGDAKYRequest request)
        {
            return base.Channel.ZCRM_HOPDONGDAKY(request);
        }
        
        public HopDongDK.ZCRM_HOPDONGDAKYResponse ZCRM_HOPDONGDAKY(HopDongDK.ZCRM_HOPDONGDAKY1 ZCRM_HOPDONGDAKY1)
        {
            HopDongDK.ZCRM_HOPDONGDAKYRequest inValue = new HopDongDK.ZCRM_HOPDONGDAKYRequest();
            inValue.ZCRM_HOPDONGDAKY = ZCRM_HOPDONGDAKY1;
            HopDongDK.ZCRM_HOPDONGDAKYResponse1 retVal = ((HopDongDK.ZWS_CRM03)(this)).ZCRM_HOPDONGDAKY(inValue);
            return retVal.ZCRM_HOPDONGDAKYResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<HopDongDK.ZCRM_HOPDONGDAKYResponse1> HopDongDK.ZWS_CRM03.ZCRM_HOPDONGDAKYAsync(HopDongDK.ZCRM_HOPDONGDAKYRequest request)
        {
            return base.Channel.ZCRM_HOPDONGDAKYAsync(request);
        }
        
        public System.Threading.Tasks.Task<HopDongDK.ZCRM_HOPDONGDAKYResponse1> ZCRM_HOPDONGDAKYAsync(HopDongDK.ZCRM_HOPDONGDAKY1 ZCRM_HOPDONGDAKY)
        {
            HopDongDK.ZCRM_HOPDONGDAKYRequest inValue = new HopDongDK.ZCRM_HOPDONGDAKYRequest();
            inValue.ZCRM_HOPDONGDAKY = ZCRM_HOPDONGDAKY;
            return ((HopDongDK.ZWS_CRM03)(this)).ZCRM_HOPDONGDAKYAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
    }
}
