﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAPDoanhThuXuatKhau
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="SAPDoanhThuXuatKhau.ZWS_CRM12")]
    public interface ZWS_CRM12
    {
        
        // CODEGEN: Generating message contract since the operation ZCRM_DOANHTHUXUATKHAU is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM12:ZCRM_DOANHTHUXUATKHAURequest", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse1 ZCRM_DOANHTHUXUATKHAU(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZWS_CRM12:ZCRM_DOANHTHUXUATKHAURequest", ReplyAction="*")]
        System.Threading.Tasks.Task<SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse1> ZCRM_DOANHTHUXUATKHAUAsync(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHTHUXUATKHAU
    {
        
        private string cOMPARATIVEField;
        
        private string fROM_DATEField;
        
        private string pLANTField;
        
        private string tHITRUONGCAP2XKField;
        
        private string tO_DATEField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string COMPARATIVE
        {
            get
            {
                return this.cOMPARATIVEField;
            }
            set
            {
                this.cOMPARATIVEField = value;
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string THITRUONGCAP2XK
        {
            get
            {
                return this.tHITRUONGCAP2XKField;
            }
            set
            {
                this.tHITRUONGCAP2XKField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHTHUXUATKHAU_THITRG
    {
        
        private string tHITRUONGCAP2XKField;
        
        private decimal sANLUONGField;
        
        private decimal dOANHSO_USDField;
        
        private decimal pLANT_SANLUONGField;
        
        private decimal pLANT_DOANHSO_USDField;
        
        private decimal tILESANLUONGField;
        
        private decimal tILEDOANHSOField;
        
        private string cOMPARATIVEField;
        
        private decimal cP_SANLUONGField;
        
        private decimal cP_DOANHSO_USDField;
        
        private decimal cP_PLANT_SANLUONGField;
        
        private decimal cP_PLANT_DOANHSO_USDField;
        
        private decimal cP_TILESANLUONGField;
        
        private decimal cP_TILEDOANHSOField;
        
        private decimal sOSANHSANLUONGField;
        
        private decimal sOSANHDOANHSOField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string THITRUONGCAP2XK
        {
            get
            {
                return this.tHITRUONGCAP2XKField;
            }
            set
            {
                this.tHITRUONGCAP2XKField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public decimal SANLUONG
        {
            get
            {
                return this.sANLUONGField;
            }
            set
            {
                this.sANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public decimal DOANHSO_USD
        {
            get
            {
                return this.dOANHSO_USDField;
            }
            set
            {
                this.dOANHSO_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public decimal PLANT_SANLUONG
        {
            get
            {
                return this.pLANT_SANLUONGField;
            }
            set
            {
                this.pLANT_SANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public decimal PLANT_DOANHSO_USD
        {
            get
            {
                return this.pLANT_DOANHSO_USDField;
            }
            set
            {
                this.pLANT_DOANHSO_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public decimal TILESANLUONG
        {
            get
            {
                return this.tILESANLUONGField;
            }
            set
            {
                this.tILESANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public decimal TILEDOANHSO
        {
            get
            {
                return this.tILEDOANHSOField;
            }
            set
            {
                this.tILEDOANHSOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string COMPARATIVE
        {
            get
            {
                return this.cOMPARATIVEField;
            }
            set
            {
                this.cOMPARATIVEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public decimal CP_SANLUONG
        {
            get
            {
                return this.cP_SANLUONGField;
            }
            set
            {
                this.cP_SANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public decimal CP_DOANHSO_USD
        {
            get
            {
                return this.cP_DOANHSO_USDField;
            }
            set
            {
                this.cP_DOANHSO_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public decimal CP_PLANT_SANLUONG
        {
            get
            {
                return this.cP_PLANT_SANLUONGField;
            }
            set
            {
                this.cP_PLANT_SANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public decimal CP_PLANT_DOANHSO_USD
        {
            get
            {
                return this.cP_PLANT_DOANHSO_USDField;
            }
            set
            {
                this.cP_PLANT_DOANHSO_USDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public decimal CP_TILESANLUONG
        {
            get
            {
                return this.cP_TILESANLUONGField;
            }
            set
            {
                this.cP_TILESANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public decimal CP_TILEDOANHSO
        {
            get
            {
                return this.cP_TILEDOANHSOField;
            }
            set
            {
                this.cP_TILEDOANHSOField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public decimal SOSANHSANLUONG
        {
            get
            {
                return this.sOSANHSANLUONGField;
            }
            set
            {
                this.sOSANHSANLUONGField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=15)]
        public decimal SOSANHDOANHSO
        {
            get
            {
                return this.sOSANHDOANHSOField;
            }
            set
            {
                this.sOSANHDOANHSOField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZCRM_DOANHTHUXUATKHAUResponse
    {
        
        private ZCRM_DOANHTHUXUATKHAU_THITRG[] dOANHSO_DATASField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZCRM_DOANHTHUXUATKHAU_THITRG[] DOANHSO_DATAS
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
    public partial class ZCRM_DOANHTHUXUATKHAURequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAU ZCRM_DOANHTHUXUATKHAU;
        
        public ZCRM_DOANHTHUXUATKHAURequest()
        {
        }
        
        public ZCRM_DOANHTHUXUATKHAURequest(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAU ZCRM_DOANHTHUXUATKHAU)
        {
            this.ZCRM_DOANHTHUXUATKHAU = ZCRM_DOANHTHUXUATKHAU;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZCRM_DOANHTHUXUATKHAUResponse1
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse ZCRM_DOANHTHUXUATKHAUResponse;
        
        public ZCRM_DOANHTHUXUATKHAUResponse1()
        {
        }
        
        public ZCRM_DOANHTHUXUATKHAUResponse1(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse ZCRM_DOANHTHUXUATKHAUResponse)
        {
            this.ZCRM_DOANHTHUXUATKHAUResponse = ZCRM_DOANHTHUXUATKHAUResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public interface ZWS_CRM12Channel : SAPDoanhThuXuatKhau.ZWS_CRM12, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.3")]
    public partial class ZWS_CRM12Client : System.ServiceModel.ClientBase<SAPDoanhThuXuatKhau.ZWS_CRM12>, SAPDoanhThuXuatKhau.ZWS_CRM12
    {
        
        public ZWS_CRM12Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse1 SAPDoanhThuXuatKhau.ZWS_CRM12.ZCRM_DOANHTHUXUATKHAU(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest request)
        {
            return base.Channel.ZCRM_DOANHTHUXUATKHAU(request);
        }
        
        public SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse ZCRM_DOANHTHUXUATKHAU(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAU ZCRM_DOANHTHUXUATKHAU1)
        {
            SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest inValue = new SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest();
            inValue.ZCRM_DOANHTHUXUATKHAU = ZCRM_DOANHTHUXUATKHAU1;
            SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse1 retVal = ((SAPDoanhThuXuatKhau.ZWS_CRM12)(this)).ZCRM_DOANHTHUXUATKHAU(inValue);
            return retVal.ZCRM_DOANHTHUXUATKHAUResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse1> SAPDoanhThuXuatKhau.ZWS_CRM12.ZCRM_DOANHTHUXUATKHAUAsync(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest request)
        {
            return base.Channel.ZCRM_DOANHTHUXUATKHAUAsync(request);
        }
        
        public System.Threading.Tasks.Task<SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAUResponse1> ZCRM_DOANHTHUXUATKHAUAsync(SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAU ZCRM_DOANHTHUXUATKHAU)
        {
            SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest inValue = new SAPDoanhThuXuatKhau.ZCRM_DOANHTHUXUATKHAURequest();
            inValue.ZCRM_DOANHTHUXUATKHAU = ZCRM_DOANHTHUXUATKHAU;
            return ((SAPDoanhThuXuatKhau.ZWS_CRM12)(this)).ZCRM_DOANHTHUXUATKHAUAsync(inValue);
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
