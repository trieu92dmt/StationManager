﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPTraCuuCuocTau
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="SAPTraCuuCuocTau.ZWS_CRM07")]
    public interface ZWS_CRM07
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_CUOCTAU is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM07:ZCRM_CUOCTAURequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse1 ZCRM_CUOCTAU(SAPTraCuuCuocTau.ZCRM_CUOCTAURequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM07:ZCRM_CUOCTAURequest", ReplyAction="*")]
        System.Threading.Tasks.Task<SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse1> ZCRM_CUOCTAUAsync(SAPTraCuuCuocTau.ZCRM_CUOCTAURequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_CUOCTAU1
    {
        
        private string cANGDENField;
        
        private string fROM_DATEField;
        
        private string hANGTAUField;
        
        private string tO_DATEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string CANGDEN
        {
            get
            {
                return this.cANGDENField;
            }
            set
            {
                this.cANGDENField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string FROM_DATE
        {
            get
            {
                return this.fROM_DATEField;
            }
            set
            {
                this.fROM_DATEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string HANGTAU
        {
            get
            {
                return this.hANGTAUField;
            }
            set
            {
                this.hANGTAUField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string TO_DATE
        {
            get
            {
                return this.tO_DATEField;
            }
            set
            {
                this.tO_DATEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_CUOCTAU
    {
        
        private string hANGTAUField;
        
        private string cANGDENField;
        
        private string cURRENCYField;
        
        private string nGAYBLField;
        
        private string sOBLField;
        
        private string sOCONTField;
        
        private string sOINVField;
        
        private decimal tONGCUOCField;
        
        private string pOSTINGDATEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string HANGTAU
        {
            get
            {
                return this.hANGTAUField;
            }
            set
            {
                this.hANGTAUField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string CANGDEN
        {
            get
            {
                return this.cANGDENField;
            }
            set
            {
                this.cANGDENField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string CURRENCY
        {
            get
            {
                return this.cURRENCYField;
            }
            set
            {
                this.cURRENCYField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string NGAYBL
        {
            get
            {
                return this.nGAYBLField;
            }
            set
            {
                this.nGAYBLField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string SOBL
        {
            get
            {
                return this.sOBLField;
            }
            set
            {
                this.sOBLField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string SOCONT
        {
            get
            {
                return this.sOCONTField;
            }
            set
            {
                this.sOCONTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string SOINV
        {
            get
            {
                return this.sOINVField;
            }
            set
            {
                this.sOINVField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public decimal TONGCUOC
        {
            get
            {
                return this.tONGCUOCField;
            }
            set
            {
                this.tONGCUOCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string POSTINGDATE
        {
            get
            {
                return this.pOSTINGDATEField;
            }
            set
            {
                this.pOSTINGDATEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_CUOCTAUResponse
    {
        
        private ZCRM_CUOCTAU[] cUOCTAU_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_CUOCTAU[] CUOCTAU_DATAS
        {
            get
            {
                return this.cUOCTAU_DATASField;
            }
            set
            {
                this.cUOCTAU_DATASField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_CUOCTAURequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPTraCuuCuocTau.ZCRM_CUOCTAU1 ZCRM_CUOCTAU;
        
        public ZCRM_CUOCTAURequest()
        {
        }
        
        public ZCRM_CUOCTAURequest(SAPTraCuuCuocTau.ZCRM_CUOCTAU1 ZCRM_CUOCTAU)
        {
            this.ZCRM_CUOCTAU = ZCRM_CUOCTAU;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_CUOCTAUResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse ZCRM_CUOCTAUResponse;
        
        public ZCRM_CUOCTAUResponse1()
        {
        }
        
        public ZCRM_CUOCTAUResponse1(SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse ZCRM_CUOCTAUResponse)
        {
            this.ZCRM_CUOCTAUResponse = ZCRM_CUOCTAUResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface ZWS_CRM07Channel : SAPTraCuuCuocTau.ZWS_CRM07, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class ZWS_CRM07Client : System.ServiceModel.ClientBase<SAPTraCuuCuocTau.ZWS_CRM07>, SAPTraCuuCuocTau.ZWS_CRM07
    {
        
        public ZWS_CRM07Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse1 SAPTraCuuCuocTau.ZWS_CRM07.ZCRM_CUOCTAU(SAPTraCuuCuocTau.ZCRM_CUOCTAURequest request)
        {
            return base.Channel.ZCRM_CUOCTAU(request);
        }
        
        public SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse ZCRM_CUOCTAU(SAPTraCuuCuocTau.ZCRM_CUOCTAU1 ZCRM_CUOCTAU1)
        {
            SAPTraCuuCuocTau.ZCRM_CUOCTAURequest inValue = new SAPTraCuuCuocTau.ZCRM_CUOCTAURequest();
            inValue.ZCRM_CUOCTAU = ZCRM_CUOCTAU1;
            SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse1 retVal = ((SAPTraCuuCuocTau.ZWS_CRM07)(this)).ZCRM_CUOCTAU(inValue);
            return retVal.ZCRM_CUOCTAUResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse1> SAPTraCuuCuocTau.ZWS_CRM07.ZCRM_CUOCTAUAsync(SAPTraCuuCuocTau.ZCRM_CUOCTAURequest request)
        {
            return base.Channel.ZCRM_CUOCTAUAsync(request);
        }
        
        public System.Threading.Tasks.Task<SAPTraCuuCuocTau.ZCRM_CUOCTAUResponse1> ZCRM_CUOCTAUAsync(SAPTraCuuCuocTau.ZCRM_CUOCTAU1 ZCRM_CUOCTAU)
        {
            SAPTraCuuCuocTau.ZCRM_CUOCTAURequest inValue = new SAPTraCuuCuocTau.ZCRM_CUOCTAURequest();
            inValue.ZCRM_CUOCTAU = ZCRM_CUOCTAU;
            return ((SAPTraCuuCuocTau.ZWS_CRM07)(this)).ZCRM_CUOCTAUAsync(inValue);
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
