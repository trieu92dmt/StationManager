using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;
using ZXing.QrCode;
using System.Data;

namespace ISD.Admin.Handlers
{
    /// <summary>
    /// Summary description for ViewHangTag
    /// </summary>
    public class ViewHangTag : IHttpHandler
    {
        EntityDataContext _context = new EntityDataContext();
        public void ProcessRequest(HttpContext context)
        {
            string contentType = string.Empty, folderPath = string.Empty, physicalFolderPath = string.Empty, physicalFilePath = string.Empty, filePath = string.Empty, fileNameWithoutExtension = string.Empty;
            byte[] fileContent = null;

            string strWorkOrderCardId = context.Request.QueryString["Id"];
            if (!string.IsNullOrEmpty(strWorkOrderCardId))
            {
                Guid WorkOrderCardId = Guid.Parse(strWorkOrderCardId);
                //https://itemsview.ancuong.com/vieworder/orderkey
                var workOder = (from wCard in _context.WorkOrderCardModel
                               join wOrder in _context.WorkOrderModel on wCard.WorkOrderId equals wOrder.WorkOrderId
                               where wCard.WorkOrderCardId == WorkOrderCardId
                               select wOrder).FirstOrDefault();

                if (workOder == null)
                {
                    context.Response.ContentType = "image/jpeg";
                    context.Response.WriteFile("~/Upload/QRCode/noimage.jpg");
                }

                //var url = string.Format("https://itemsview.ancuong.com/vieworder/{0}", HangTag);
                fileContent = GenerateQRCode(strWorkOrderCardId, strWorkOrderCardId);
                if (fileContent != null)
                {
                    context.Response.ContentType = "image/png";
                    context.Response.BinaryWrite(fileContent);
                }
                else
                {
                    context.Response.ContentType = "image/jpeg";
                    context.Response.WriteFile("~/Upload/QRCode/noimage.jpg");
                }
            }
            else
            {
                context.Response.ContentType = "image/jpeg";
                context.Response.WriteFile("~/Upload/QRCode/noimage.jpg");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public byte[] GenerateQRCode(string fileName, string qrcodeText, int size = 500)
        {
            string folderPath = "/Upload/report/HangTagQRcode/";
            string imagePath = "/Upload/report/HangTagQRcode/" + fileName + ".png";
            // If the directory doesn't exist then create it.
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter()
            {
                Options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = size,
                    Height = size,
                    //ErrorCorrection = ErrorCorrectionLevel.H,
                    Margin = 0,
                    PureBarcode = true
                }
            };
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = System.Web.HttpContext.Current.Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return ImageToByteArray(barcodeBitmap);
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(imageIn, typeof(byte[]));
            return xByte;
        }
    }
}