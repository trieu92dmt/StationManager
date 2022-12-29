﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoanhSoTTKHTheoNV
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="DoanhSoTTKHTheoNV.ZWS_CRM11")]
    public interface ZWS_CRM11
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_DOANHSONHANVIEN is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM11:ZCRM_DOANHSONHANVIENRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse1 ZCRM_DOANHSONHANVIEN(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM11:ZCRM_DOANHSONHANVIENRequest", ReplyAction="*")]
        System.Threading.Tasks.Task<DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse1> ZCRM_DOANHSONHANVIENAsync(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHSONHANVIEN1
    {
        
        private string fROM_DATEField;
        
        private string pARNERFUNCTIONField;
        
        private string pERSONNELNUMBERField;
        
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
        public string PARNERFUNCTION
        {
            get
            {
                return this.pARNERFUNCTIONField;
            }
            set
            {
                this.pARNERFUNCTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string PERSONNELNUMBER
        {
            get
            {
                return this.pERSONNELNUMBERField;
            }
            set
            {
                this.pERSONNELNUMBERField = value;
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
    public partial class ZCRM_DOANHSONHANVIEN
    {
        
        private string pERSONNELNUMBERField;
        
        private string pARNERFUNCTIONField;
        
        private string pERSONNAMEField;
        
        private string pROFILEFOREIGNCODEField;
        
        private string pARNER_DESCField;
        
        private decimal gTGTField;
        
        private decimal hTTField;
        
        private decimal gTGT_USDField;
        
        private decimal hTT_USDField;
        
        private decimal tONGKHOILUONG_KGField;
        
        private decimal tHANHTIEN_USDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string PERSONNELNUMBER
        {
            get
            {
                return this.pERSONNELNUMBERField;
            }
            set
            {
                this.pERSONNELNUMBERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string PARNERFUNCTION
        {
            get
            {
                return this.pARNERFUNCTIONField;
            }
            set
            {
                this.pARNERFUNCTIONField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string PERSONNAME
        {
            get
            {
                return this.pERSONNAMEField;
            }
            set
            {
                this.pERSONNAMEField = value;
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
        public string PARNER_DESC
        {
            get
            {
                return this.pARNER_DESCField;
            }
            set
            {
                this.pARNER_DESCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public decimal GTGT
        {
            get
            {
                return this.gTGTField;
            }
            set
            {
                this.gTGTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public decimal HTT
        {
            get
            {
                return this.hTTField;
            }
            set
            {
                this.hTTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public decimal GTGT_USD
        {
            get
            {
                return this.gTGT_USDField;
            }
            set
            {
                this.gTGT_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public decimal HTT_USD
        {
            get
            {
                return this.hTT_USDField;
            }
            set
            {
                this.hTT_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public decimal TONGKHOILUONG_KG
        {
            get
            {
                return this.tONGKHOILUONG_KGField;
            }
            set
            {
                this.tONGKHOILUONG_KGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHSONHANVIENResponse
    {
        
        private ZCRM_DOANHSONHANVIEN[] dOANHSO_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_DOANHSONHANVIEN[] DOANHSO_DATAS
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
    public partial class ZCRM_DOANHSONHANVIENRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIEN1 ZCRM_DOANHSONHANVIEN;
        
        public ZCRM_DOANHSONHANVIENRequest()
        {
        }
        
        public ZCRM_DOANHSONHANVIENRequest(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIEN1 ZCRM_DOANHSONHANVIEN)
        {
            this.ZCRM_DOANHSONHANVIEN = ZCRM_DOANHSONHANVIEN;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DOANHSONHANVIENResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse ZCRM_DOANHSONHANVIENResponse;
        
        public ZCRM_DOANHSONHANVIENResponse1()
        {
        }
        
        public ZCRM_DOANHSONHANVIENResponse1(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse ZCRM_DOANHSONHANVIENResponse)
        {
            this.ZCRM_DOANHSONHANVIENResponse = ZCRM_DOANHSONHANVIENResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface ZWS_CRM11Channel : DoanhSoTTKHTheoNV.ZWS_CRM11, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class ZWS_CRM11Client : System.ServiceModel.ClientBase<DoanhSoTTKHTheoNV.ZWS_CRM11>, DoanhSoTTKHTheoNV.ZWS_CRM11
    {
        
        public ZWS_CRM11Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse1 DoanhSoTTKHTheoNV.ZWS_CRM11.ZCRM_DOANHSONHANVIEN(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest request)
        {
            return base.Channel.ZCRM_DOANHSONHANVIEN(request);
        }
        
        public DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse ZCRM_DOANHSONHANVIEN(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIEN1 ZCRM_DOANHSONHANVIEN1)
        {
            DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest inValue = new DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest();
            inValue.ZCRM_DOANHSONHANVIEN = ZCRM_DOANHSONHANVIEN1;
            DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse1 retVal = ((DoanhSoTTKHTheoNV.ZWS_CRM11)(this)).ZCRM_DOANHSONHANVIEN(inValue);
            return retVal.ZCRM_DOANHSONHANVIENResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse1> DoanhSoTTKHTheoNV.ZWS_CRM11.ZCRM_DOANHSONHANVIENAsync(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest request)
        {
            return base.Channel.ZCRM_DOANHSONHANVIENAsync(request);
        }
        
        public System.Threading.Tasks.Task<DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENResponse1> ZCRM_DOANHSONHANVIENAsync(DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIEN1 ZCRM_DOANHSONHANVIEN)
        {
            DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest inValue = new DoanhSoTTKHTheoNV.ZCRM_DOANHSONHANVIENRequest();
            inValue.ZCRM_DOANHSONHANVIEN = ZCRM_DOANHSONHANVIEN;
            return ((DoanhSoTTKHTheoNV.ZWS_CRM11)(this)).ZCRM_DOANHSONHANVIENAsync(inValue);
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