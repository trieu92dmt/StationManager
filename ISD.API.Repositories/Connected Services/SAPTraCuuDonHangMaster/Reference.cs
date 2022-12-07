﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPTraCuuDonHangMaster
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="SAPTraCuuDonHangMaster.ZWS_CRM17")]
    public interface ZWS_CRM17
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_DONHANG_HEADER is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM17:ZCRM_DONHANG_HEADERRequest", ReplyAction="urn:sap-com:document:sap:rfc:functions:ZWS_CRM17:ZCRM_DONHANG_HEADERResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse1 ZCRM_DONHANG_HEADER(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM17:ZCRM_DONHANG_HEADERRequest", ReplyAction="urn:sap-com:document:sap:rfc:functions:ZWS_CRM17:ZCRM_DONHANG_HEADERResponse")]
        System.Threading.Tasks.Task<SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse1> ZCRM_DONHANG_HEADERAsync(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DONHANG_HEADER1
    {
        
        private string fROM_DATEField;
        
        private string pROFILEFOREIGNCODEField;
        
        private string sALESDOCUMENTField;
        
        private string sTATUSField;
        
        private string tO_DATEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
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
        public string SALESDOCUMENT
        {
            get
            {
                return this.sALESDOCUMENTField;
            }
            set
            {
                this.sALESDOCUMENTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string STATUS
        {
            get
            {
                return this.sTATUSField;
            }
            set
            {
                this.sTATUSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DONHANG_HEADER
    {
        
        private string sALESDOCUMENTField;
        
        private string sTATUSField;
        
        private string sALESEMPLOYEEField;
        
        private string pROFILEFOREIGNCODEField;
        
        private string cUSTOMER_NAMEField;
        
        private string dOCUMENTDATEField;
        
        private string pONUMBERField;
        
        private string pO_DATEField;
        
        private string sALESORGField;
        
        private decimal tHANHTIEN_USDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string SALESDOCUMENT
        {
            get
            {
                return this.sALESDOCUMENTField;
            }
            set
            {
                this.sALESDOCUMENTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string STATUS
        {
            get
            {
                return this.sTATUSField;
            }
            set
            {
                this.sTATUSField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string SALESEMPLOYEE
        {
            get
            {
                return this.sALESEMPLOYEEField;
            }
            set
            {
                this.sALESEMPLOYEEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string CUSTOMER_NAME
        {
            get
            {
                return this.cUSTOMER_NAMEField;
            }
            set
            {
                this.cUSTOMER_NAMEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string DOCUMENTDATE
        {
            get
            {
                return this.dOCUMENTDATEField;
            }
            set
            {
                this.dOCUMENTDATEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string PONUMBER
        {
            get
            {
                return this.pONUMBERField;
            }
            set
            {
                this.pONUMBERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string PO_DATE
        {
            get
            {
                return this.pO_DATEField;
            }
            set
            {
                this.pO_DATEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public decimal THANHTIEN_USD
        {
            get
            {
                return this.tHANHTIEN_USDField;
            }
            set
            {
                this.tHANHTIEN_USDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DONHANG_HEADERResponse
    {
        
        private ZCRM_DONHANG_HEADER[] dONHANG_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_DONHANG_HEADER[] DONHANG_DATAS
        {
            get
            {
                return this.dONHANG_DATASField;
            }
            set
            {
                this.dONHANG_DATASField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DONHANG_HEADERRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADER1 ZCRM_DONHANG_HEADER;
        
        public ZCRM_DONHANG_HEADERRequest()
        {
        }
        
        public ZCRM_DONHANG_HEADERRequest(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADER1 ZCRM_DONHANG_HEADER)
        {
            this.ZCRM_DONHANG_HEADER = ZCRM_DONHANG_HEADER;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DONHANG_HEADERResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse ZCRM_DONHANG_HEADERResponse;
        
        public ZCRM_DONHANG_HEADERResponse1()
        {
        }
        
        public ZCRM_DONHANG_HEADERResponse1(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse ZCRM_DONHANG_HEADERResponse)
        {
            this.ZCRM_DONHANG_HEADERResponse = ZCRM_DONHANG_HEADERResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public interface ZWS_CRM17Channel : SAPTraCuuDonHangMaster.ZWS_CRM17, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public partial class ZWS_CRM17Client : System.ServiceModel.ClientBase<SAPTraCuuDonHangMaster.ZWS_CRM17>, SAPTraCuuDonHangMaster.ZWS_CRM17
    {
        
        public ZWS_CRM17Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse1 SAPTraCuuDonHangMaster.ZWS_CRM17.ZCRM_DONHANG_HEADER(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest request)
        {
            return base.Channel.ZCRM_DONHANG_HEADER(request);
        }
        
        public SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse ZCRM_DONHANG_HEADER(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADER1 ZCRM_DONHANG_HEADER1)
        {
            SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest inValue = new SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest();
            inValue.ZCRM_DONHANG_HEADER = ZCRM_DONHANG_HEADER1;
            SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse1 retVal = ((SAPTraCuuDonHangMaster.ZWS_CRM17)(this)).ZCRM_DONHANG_HEADER(inValue);
            return retVal.ZCRM_DONHANG_HEADERResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse1> SAPTraCuuDonHangMaster.ZWS_CRM17.ZCRM_DONHANG_HEADERAsync(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest request)
        {
            return base.Channel.ZCRM_DONHANG_HEADERAsync(request);
        }
        
        public System.Threading.Tasks.Task<SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERResponse1> ZCRM_DONHANG_HEADERAsync(SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADER1 ZCRM_DONHANG_HEADER)
        {
            SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest inValue = new SAPTraCuuDonHangMaster.ZCRM_DONHANG_HEADERRequest();
            inValue.ZCRM_DONHANG_HEADER = ZCRM_DONHANG_HEADER;
            return ((SAPTraCuuDonHangMaster.ZWS_CRM17)(this)).ZCRM_DONHANG_HEADERAsync(inValue);
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
