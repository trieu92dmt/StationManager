using APIDoanhSoKH;
using APIDuyetGoDonHang;
using APIHopDong;
using DoanhSoTTKHTheoNV;
using HopDongDK;
using ISD.API.EntityModels.Data;
using ISD.API.EntityModels.Models;
using ISD.API.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAPAttachmentFilesSO;
using SAPBCHopDongMaster;
using SAPCanhBaoDonHang;
using SAPDoanhSoXuatBan;
using SAPDoanhSoXuatBanMaster;
using SAPDoanhThuXuatKhau;
using SAPDoanhThuXuatKhauMaster;
using SAPSyncNhanVien;
using SAPTonKho;
using SAPTraCuuCuocTau;
using SAPTraCuuCuocTauMaster;
using SAPTraCuuDonHang;
using SAPTraCuuDonHangMaster;
using ServiceSAP;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ISD.API.Repositories
{
    public class SaleOrderRepository
    {
        public Guid SYSTEM;
        private string MessagesError;
        private readonly IISDConfigManager configuration;  
        public SaleOrderRepository(EntityDataContext _context)
        {
            SYSTEM = new Guid("FD68F5F8-01F9-480F-ACB7-BA5D74D299C8");
            this.configuration = new ISDConfigManager();

        }

       

        #region Doanh số xuất bán

        public Task<ZCRM_ATTACHMENT_FILESResponse1> SAPAttachmentFilesSO(ZCRM_ATTACHMENT_FILES input)
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Name = "ZBN";
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.SendTimeout = TimeSpan.FromHours(1);
            var endpoint = new EndpointAddress(new Uri("http://erpqas.minhphu.com:8002/sap/bc/srt/rfc/sap/zws_crm19/400/zsn/zbn"));

            var client = new ZWS_CRM19Client(binding, endpoint);
            client.ClientCredentials.UserName.UserName = configuration.Username;
            client.ClientCredentials.UserName.Password = configuration.Password;
            
            var response = client.ZCRM_ATTACHMENT_FILESAsync(input);
            return response;
        }
        #endregion

    }

}
