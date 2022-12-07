﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPTraCuuDonHang
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="SAPTraCuuDonHang.ZWS_CRM09")]
    public interface ZWS_CRM09
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_DONHANG is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM09:ZCRM_DONHANGRequest", ReplyAction="urn:sap-com:document:sap:rfc:functions:ZWS_CRM09:ZCRM_DONHANGResponse")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SAPTraCuuDonHang.ZCRM_DONHANGResponse1 ZCRM_DONHANG(SAPTraCuuDonHang.ZCRM_DONHANGRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM09:ZCRM_DONHANGRequest", ReplyAction="urn:sap-com:document:sap:rfc:functions:ZWS_CRM09:ZCRM_DONHANGResponse")]
        System.Threading.Tasks.Task<SAPTraCuuDonHang.ZCRM_DONHANGResponse1> ZCRM_DONHANGAsync(SAPTraCuuDonHang.ZCRM_DONHANGRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DONHANG1
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
    public partial class ZCRM_DONHANG
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
        
        private string lINEIDField;
        
        private string pLANTField;
        
        private string mATERIALField;
        
        private string mATERIALDESCField;
        
        private decimal dATHANG_KG2Field;
        
        private decimal uNITPRICEField;
        
        private string cURRENCYField;
        
        private decimal pRICEFOBKG2Field;
        
        private decimal pRICEFOBKG2_USDField;
        
        private decimal nETVALUE_USDField;
        
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string PLANT
        {
            get
            {
                return this.pLANTField;
            }
            set
            {
                this.pLANTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string MATERIAL
        {
            get
            {
                return this.mATERIALField;
            }
            set
            {
                this.mATERIALField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public string MATERIALDESC
        {
            get
            {
                return this.mATERIALDESCField;
            }
            set
            {
                this.mATERIALDESCField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public decimal DATHANG_KG2
        {
            get
            {
                return this.dATHANG_KG2Field;
            }
            set
            {
                this.dATHANG_KG2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=15)]
        public decimal UNITPRICE
        {
            get
            {
                return this.uNITPRICEField;
            }
            set
            {
                this.uNITPRICEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=16)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=17)]
        public decimal PRICEFOBKG2
        {
            get
            {
                return this.pRICEFOBKG2Field;
            }
            set
            {
                this.pRICEFOBKG2Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=18)]
        public decimal PRICEFOBKG2_USD
        {
            get
            {
                return this.pRICEFOBKG2_USDField;
            }
            set
            {
                this.pRICEFOBKG2_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=19)]
        public decimal NETVALUE_USD
        {
            get
            {
                return this.nETVALUE_USDField;
            }
            set
            {
                this.nETVALUE_USDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DONHANGResponse
    {
        
        private ZCRM_DONHANG[] dONHANG_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_DONHANG[] DONHANG_DATAS
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
    public partial class ZCRM_DONHANGRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPTraCuuDonHang.ZCRM_DONHANG1 ZCRM_DONHANG;
        
        public ZCRM_DONHANGRequest()
        {
        }
        
        public ZCRM_DONHANGRequest(SAPTraCuuDonHang.ZCRM_DONHANG1 ZCRM_DONHANG)
        {
            this.ZCRM_DONHANG = ZCRM_DONHANG;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DONHANGResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPTraCuuDonHang.ZCRM_DONHANGResponse ZCRM_DONHANGResponse;
        
        public ZCRM_DONHANGResponse1()
        {
        }
        
        public ZCRM_DONHANGResponse1(SAPTraCuuDonHang.ZCRM_DONHANGResponse ZCRM_DONHANGResponse)
        {
            this.ZCRM_DONHANGResponse = ZCRM_DONHANGResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public interface ZWS_CRM09Channel : SAPTraCuuDonHang.ZWS_CRM09, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3-preview3.21351.2")]
    public partial class ZWS_CRM09Client : System.ServiceModel.ClientBase<SAPTraCuuDonHang.ZWS_CRM09>, SAPTraCuuDonHang.ZWS_CRM09
    {
        
        public ZWS_CRM09Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SAPTraCuuDonHang.ZCRM_DONHANGResponse1 SAPTraCuuDonHang.ZWS_CRM09.ZCRM_DONHANG(SAPTraCuuDonHang.ZCRM_DONHANGRequest request)
        {
            return base.Channel.ZCRM_DONHANG(request);
        }
        
        public SAPTraCuuDonHang.ZCRM_DONHANGResponse ZCRM_DONHANG(SAPTraCuuDonHang.ZCRM_DONHANG1 ZCRM_DONHANG1)
        {
            SAPTraCuuDonHang.ZCRM_DONHANGRequest inValue = new SAPTraCuuDonHang.ZCRM_DONHANGRequest();
            inValue.ZCRM_DONHANG = ZCRM_DONHANG1;
            SAPTraCuuDonHang.ZCRM_DONHANGResponse1 retVal = ((SAPTraCuuDonHang.ZWS_CRM09)(this)).ZCRM_DONHANG(inValue);
            return retVal.ZCRM_DONHANGResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<SAPTraCuuDonHang.ZCRM_DONHANGResponse1> SAPTraCuuDonHang.ZWS_CRM09.ZCRM_DONHANGAsync(SAPTraCuuDonHang.ZCRM_DONHANGRequest request)
        {
            return base.Channel.ZCRM_DONHANGAsync(request);
        }
        
        public System.Threading.Tasks.Task<SAPTraCuuDonHang.ZCRM_DONHANGResponse1> ZCRM_DONHANGAsync(SAPTraCuuDonHang.ZCRM_DONHANG1 ZCRM_DONHANG)
        {
            SAPTraCuuDonHang.ZCRM_DONHANGRequest inValue = new SAPTraCuuDonHang.ZCRM_DONHANGRequest();
            inValue.ZCRM_DONHANG = ZCRM_DONHANG;
            return ((SAPTraCuuDonHang.ZWS_CRM09)(this)).ZCRM_DONHANGAsync(inValue);
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
