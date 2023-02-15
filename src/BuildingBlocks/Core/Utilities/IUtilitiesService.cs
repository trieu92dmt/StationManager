using Microsoft.AspNetCore.Http;

namespace Core.Utilities
{
    public interface IUtilitiesService
    {
        /// <summary>
        /// Generate QR Code
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="fileName"></param>
        /// <param name="qrcodeText"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        string GenerateQRCode(string folder, string fileName, string qrcodeText, int size = 500);
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <param name="FileType"></param>
        /// <returns></returns>
        Task<string> UploadFile(IFormFile file, string folder, int minWidth = 300, int maxWidth = 1600, int minHeight = 300, int maxHeight = 1600, string FileType = null);

        /// <summary>
        /// GET MD5 Password
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        string GetMd5Sum(string str);
    }
}
