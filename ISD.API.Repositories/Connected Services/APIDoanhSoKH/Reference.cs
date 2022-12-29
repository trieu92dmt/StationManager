﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIDoanhSoKH
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="APIDoanhSoKH.ZWS_CRM02")]
    public interface ZWS_CRM02
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_DOANHSOKH is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM02:ZCRM_DOANHSOKHRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        APIDoanhSoKH.ZCRM_DOANHSOKHResponse1 ZCRM_DOANHSOKH(APIDoanhSoKH.ZCRM_DOANHSOKHRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM02:ZCRM_DOANHSOKHRequest", ReplyAction="*")]
        System.Threading.Tasks.Task<APIDoanhSoKH.ZCRM_DOANHSOKHResponse1> ZCRM_DOANHSOKHAsync(APIDoanhSoKH.ZCRM_DOANHSOKHRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHSOKH1
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
    public partial class ZCRM_DOANHSOKH
    {
        
        private string sALESORGField;
        
        private string pERIODField;
        
        private string pROFILEFOREIGNCODEField;
        
        private string pRODUCTHIERACHYField;
        
        private decimal dOANHSOField;
        
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
        public string PRODUCTHIERACHY
        {
            get
            {
                return this.pRODUCTHIERACHYField;
            }
            set
            {
                this.pRODUCTHIERACHYField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public decimal DOANHSO
        {
            get
            {
                return this.dOANHSOField;
            }
            set
            {
                this.dOANHSOField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHSOKHResponse
    {
        
        private ZCRM_DOANHSOKH[] dOANHSO_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_DOANHSOKH[] DOANHSO_DATAS
        {
            get
            {
                return this.dOANHSO_DATASField;
            }
            set
            {
                this.dOANHSO_DATASField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DOANHSOKHRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public APIDoanhSoKH.ZCRM_DOANHSOKH1 ZCRM_DOANHSOKH;
        
        public ZCRM_DOANHSOKHRequest()
        {
        }
        
        public ZCRM_DOANHSOKHRequest(APIDoanhSoKH.ZCRM_DOANHSOKH1 ZCRM_DOANHSOKH)
        {
            this.ZCRM_DOANHSOKH = ZCRM_DOANHSOKH;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DOANHSOKHResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public APIDoanhSoKH.ZCRM_DOANHSOKHResponse ZCRM_DOANHSOKHResponse;
        
        public ZCRM_DOANHSOKHResponse1()
        {
        }
        
        public ZCRM_DOANHSOKHResponse1(APIDoanhSoKH.ZCRM_DOANHSOKHResponse ZCRM_DOANHSOKHResponse)
        {
            this.ZCRM_DOANHSOKHResponse = ZCRM_DOANHSOKHResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface ZWS_CRM02Channel : APIDoanhSoKH.ZWS_CRM02, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class ZWS_CRM02Client : System.ServiceModel.ClientBase<APIDoanhSoKH.ZWS_CRM02>, APIDoanhSoKH.ZWS_CRM02
    {
        
        public ZWS_CRM02Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        APIDoanhSoKH.ZCRM_DOANHSOKHResponse1 APIDoanhSoKH.ZWS_CRM02.ZCRM_DOANHSOKH(APIDoanhSoKH.ZCRM_DOANHSOKHRequest request)
        {
            return base.Channel.ZCRM_DOANHSOKH(request);
        }
        
        public APIDoanhSoKH.ZCRM_DOANHSOKHResponse ZCRM_DOANHSOKH(APIDoanhSoKH.ZCRM_DOANHSOKH1 ZCRM_DOANHSOKH1)
        {
            APIDoanhSoKH.ZCRM_DOANHSOKHRequest inValue = new APIDoanhSoKH.ZCRM_DOANHSOKHRequest();
            inValue.ZCRM_DOANHSOKH = ZCRM_DOANHSOKH1;
            APIDoanhSoKH.ZCRM_DOANHSOKHResponse1 retVal = ((APIDoanhSoKH.ZWS_CRM02)(this)).ZCRM_DOANHSOKH(inValue);
            return retVal.ZCRM_DOANHSOKHResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<APIDoanhSoKH.ZCRM_DOANHSOKHResponse1> APIDoanhSoKH.ZWS_CRM02.ZCRM_DOANHSOKHAsync(APIDoanhSoKH.ZCRM_DOANHSOKHRequest request)
        {
            return base.Channel.ZCRM_DOANHSOKHAsync(request);
        }
        
        public System.Threading.Tasks.Task<APIDoanhSoKH.ZCRM_DOANHSOKHResponse1> ZCRM_DOANHSOKHAsync(APIDoanhSoKH.ZCRM_DOANHSOKH1 ZCRM_DOANHSOKH)
        {
            APIDoanhSoKH.ZCRM_DOANHSOKHRequest inValue = new APIDoanhSoKH.ZCRM_DOANHSOKHRequest();
            inValue.ZCRM_DOANHSOKH = ZCRM_DOANHSOKH;
            return ((APIDoanhSoKH.ZWS_CRM02)(this)).ZCRM_DOANHSOKHAsync(inValue);
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