﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPDoanhSoXuatBan
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="SAPDoanhSoXuatBan.ZWS_CRM10")]
    public interface ZWS_CRM10
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_DOANHSOXUATBAN is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM10:ZCRM_DOANHSOXUATBANRequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse1 ZCRM_DOANHSOXUATBAN(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM10:ZCRM_DOANHSOXUATBANRequest", ReplyAction="*")]
        System.Threading.Tasks.Task<SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse1> ZCRM_DOANHSOXUATBANAsync(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHSOXUATBAN1
    {
        
        private string bILLINGDOCUMENTField;
        
        private string dISTRIBUTIONCHANNELField;
        
        private string fROM_DATEField;
        
        private string pROFILEFOREIGNCODEField;
        
        private string sALESORGField;
        
        private string tO_DATEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string BILLINGDOCUMENT
        {
            get
            {
                return this.bILLINGDOCUMENTField;
            }
            set
            {
                this.bILLINGDOCUMENTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string DISTRIBUTIONCHANNEL
        {
            get
            {
                return this.dISTRIBUTIONCHANNELField;
            }
            set
            {
                this.dISTRIBUTIONCHANNELField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
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
    public partial class ZCRM_DOANHSOXUATBAN
    {
        
        private string nHANVIENKINHDOANHField;
        
        private string tROLYBANHANGField;
        
        private string cUSTOMERField;
        
        private string bILLINGDOCUMENTField;
        
        private string lINEIDField;
        
        private decimal tHANHTIENField;
        
        private string cURRENCYField;
        
        private decimal kL_KG2Field;
        
        private decimal tHANHTIEN_USDField;
        
        private string pRODUCTHIERACHYField;
        
        private string bUYERField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string NHANVIENKINHDOANH
        {
            get
            {
                return this.nHANVIENKINHDOANHField;
            }
            set
            {
                this.nHANVIENKINHDOANHField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string TROLYBANHANG
        {
            get
            {
                return this.tROLYBANHANGField;
            }
            set
            {
                this.tROLYBANHANGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string CUSTOMER
        {
            get
            {
                return this.cUSTOMERField;
            }
            set
            {
                this.cUSTOMERField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string BILLINGDOCUMENT
        {
            get
            {
                return this.bILLINGDOCUMENTField;
            }
            set
            {
                this.bILLINGDOCUMENTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string LINEID
        {
            get
            {
                return this.lINEIDField;
            }
            set
            {
                this.lINEIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public decimal THANHTIEN
        {
            get
            {
                return this.tHANHTIENField;
            }
            set
            {
                this.tHANHTIENField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public decimal KL_KG2
        {
            get
            {
                return this.kL_KG2Field;
            }
            set
            {
                this.kL_KG2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public string BUYER
        {
            get
            {
                return this.bUYERField;
            }
            set
            {
                this.bUYERField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHSOXUATBANResponse
    {
        
        private ZCRM_DOANHSOXUATBAN[] dOANHSO_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_DOANHSOXUATBAN[] DOANHSO_DATAS
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
    public partial class ZCRM_DOANHSOXUATBANRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBAN1 ZCRM_DOANHSOXUATBAN;
        
        public ZCRM_DOANHSOXUATBANRequest()
        {
        }
        
        public ZCRM_DOANHSOXUATBANRequest(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBAN1 ZCRM_DOANHSOXUATBAN)
        {
            this.ZCRM_DOANHSOXUATBAN = ZCRM_DOANHSOXUATBAN;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DOANHSOXUATBANResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse ZCRM_DOANHSOXUATBANResponse;
        
        public ZCRM_DOANHSOXUATBANResponse1()
        {
        }
        
        public ZCRM_DOANHSOXUATBANResponse1(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse ZCRM_DOANHSOXUATBANResponse)
        {
            this.ZCRM_DOANHSOXUATBANResponse = ZCRM_DOANHSOXUATBANResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface ZWS_CRM10Channel : SAPDoanhSoXuatBan.ZWS_CRM10, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class ZWS_CRM10Client : System.ServiceModel.ClientBase<SAPDoanhSoXuatBan.ZWS_CRM10>, SAPDoanhSoXuatBan.ZWS_CRM10
    {
        
        public ZWS_CRM10Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse1 SAPDoanhSoXuatBan.ZWS_CRM10.ZCRM_DOANHSOXUATBAN(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest request)
        {
            return base.Channel.ZCRM_DOANHSOXUATBAN(request);
        }
        
        public SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse ZCRM_DOANHSOXUATBAN(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBAN1 ZCRM_DOANHSOXUATBAN1)
        {
            SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest inValue = new SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest();
            inValue.ZCRM_DOANHSOXUATBAN = ZCRM_DOANHSOXUATBAN1;
            SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse1 retVal = ((SAPDoanhSoXuatBan.ZWS_CRM10)(this)).ZCRM_DOANHSOXUATBAN(inValue);
            return retVal.ZCRM_DOANHSOXUATBANResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse1> SAPDoanhSoXuatBan.ZWS_CRM10.ZCRM_DOANHSOXUATBANAsync(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest request)
        {
            return base.Channel.ZCRM_DOANHSOXUATBANAsync(request);
        }
        
        public System.Threading.Tasks.Task<SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANResponse1> ZCRM_DOANHSOXUATBANAsync(SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBAN1 ZCRM_DOANHSOXUATBAN)
        {
            SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest inValue = new SAPDoanhSoXuatBan.ZCRM_DOANHSOXUATBANRequest();
            inValue.ZCRM_DOANHSOXUATBAN = ZCRM_DOANHSOXUATBAN;
            return ((SAPDoanhSoXuatBan.ZWS_CRM10)(this)).ZCRM_DOANHSOXUATBANAsync(inValue);
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