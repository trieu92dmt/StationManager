using ISD.API.Constant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace ISD.API.Repositories
{
    public interface IUploadFilesLibrary
    {
        Task<string> UploadFile(IFormFile file, string folder, int minWidth = 300, int maxWidth = 1600, int minHeight = 300, int maxHeight = 1600, string FileType = null);
        //(string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
        //string SizeConverter(long bytes);
        (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory);
    }

    [Obsolete()]   // warn if referenced  
    public class UploadFilesLibrary: IUploadFilesLibrary
    {
        private IHostingEnvironment _hostingEnvironment;
        public UploadFilesLibrary(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxHeight"></param>
        /// <param name="FileType"></param>
        /// <returns></returns>
        public async Task<string> UploadFile(IFormFile file, string folder, int minWidth = 300, int maxWidth = 1600, int minHeight = 300, int maxHeight = 1600, string FileType = null)
        {
            string ret = "";
            string parth = "";
            try
            {
                if (file != null && file.Length > 0)
                {
                    // nếu có chọn file
                    int indexDot = file.FileName.LastIndexOf('.');
                    string filename = file.FileName.Substring(0, indexDot);

                    //Convert file name
                    filename = ConvertToNoMarkString(filename);
                    //get file type
                    string fileType = file.FileName.Substring(indexDot);
                    //Đổi tên lại thành chuỗi phân biệt tránh trùng
                    filename = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + filename;
                    //folder path
                    var existPath = Path.Combine(/*_hostingEnvironment.ContentRootPath */new ConfigManager().DocumentDomain + "Upload/" + folder);
                    Directory.CreateDirectory(existPath);

                    //file path
                    filename = filename + fileType;
                    parth = Path.Combine(existPath, filename);

                    //gán giá trị trả về
                    ret = filename;

                    //Save File
                    using (var stream = new FileStream(parth, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            catch
            {
                ret = null;
            }
            return ret;
        }

        /// <summary>
        /// DownloadFiles
        /// </summary>
        /// <param name="subDirectory"></param>
        /// <returns></returns>
        public (string fileType, byte[] archiveData, string archiveName) DownloadFiles(string subDirectory)
        {
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";

            var files = Directory.GetFiles(Path.Combine(_hostingEnvironment.ContentRootPath, "Upload/"+ subDirectory)).ToList();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    files.ForEach(file =>
                    {
                        var theFile = archive.CreateEntry(file);
                        using (var streamWriter = new StreamWriter(theFile.Open()))
                        {
                            streamWriter.Write(File.ReadAllText(file));
                        }

                    });
                }
                return ("application/zip", memoryStream.ToArray(), zipName);
            }

        }

        public string ConvertToNoMarkString(string text)
        {
            try
            {
                //Ky tu dac biet

                for (int i = 32; i < 48; i++)
                {
                    text = text.Replace(((char)i).ToString(), "-");
                }
                text = text.Replace(".", "");
                text = text.Replace("?", "");
                text = text.Replace(" ", "-");
                text = text.Replace(",", "-");
                text = text.Replace(";", "-");
                text = text.Replace(":", "-");

                text = text.Replace("\"", "");
                text = text.Replace("–", "");
                text = text.Replace("“", "");
                text = text.Replace("”", "");

                text = text.Replace("(", "-");
                text = text.Replace(")", "-");
                text = text.Replace("@", "-");
                text = text.Replace("&", "-");
                text = text.Replace("*", "-");
                text = text.Replace("\\", "-");
                text = text.Replace("+", "-");
                text = text.Replace("/", "-");
                text = text.Replace("#", "-");
                text = text.Replace("$", "-");
                text = text.Replace("%", "-");
                text = text.Replace("^", "-");
                text = text.Replace("--", "-");
                text = text.Replace("--", "-");
                if (text.Substring(0, 1) == "-")
                {
                    text = text.Substring(1);
                }
                if (text.Substring(text.Length - 1) == "-")
                {
                    text = text.Substring(0, text.Length - 1);
                }
                //'Dấu Ngang
                text = text.Replace("A", "A");
                text = text.Replace("a", "a");
                text = text.Replace("Ă", "A");
                text = text.Replace("ă", "a");
                text = text.Replace("Â", "A");
                text = text.Replace("â", "a");
                text = text.Replace("E", "E");
                text = text.Replace("e", "e");
                text = text.Replace("Ê", "E");
                text = text.Replace("ê", "e");
                text = text.Replace("I", "I");
                text = text.Replace("i", "i");
                text = text.Replace("O", "O");
                text = text.Replace("o", "o");
                text = text.Replace("Ô", "O");
                text = text.Replace("ô", "o");
                text = text.Replace("Ơ", "O");
                text = text.Replace("ơ", "o");
                text = text.Replace("U", "U");
                text = text.Replace("u", "u");
                text = text.Replace("Ư", "U");
                text = text.Replace("ư", "u");
                text = text.Replace("Y", "Y");
                text = text.Replace("y", "y");

                //    'Dấu Huyền
                text = text.Replace("À", "A");
                text = text.Replace("à", "a");
                text = text.Replace("Ằ", "A");
                text = text.Replace("ằ", "a");
                text = text.Replace("Ầ", "A");
                text = text.Replace("ầ", "a");
                text = text.Replace("È", "E");
                text = text.Replace("è", "e");
                text = text.Replace("Ề", "E");
                text = text.Replace("ề", "e");
                text = text.Replace("Ì", "I");
                text = text.Replace("ì", "i");
                text = text.Replace("Ò", "O");
                text = text.Replace("ò", "o");
                text = text.Replace("Ồ", "O");
                text = text.Replace("ồ", "o");
                text = text.Replace("Ờ", "O");
                text = text.Replace("ờ", "o");
                text = text.Replace("Ù", "U");
                text = text.Replace("ù", "u");
                text = text.Replace("Ừ", "U");
                text = text.Replace("ừ", "u");
                text = text.Replace("Ỳ", "Y");
                text = text.Replace("ỳ", "y");

                //'Dấu Sắc
                text = text.Replace("Á", "A");
                text = text.Replace("á", "a");
                text = text.Replace("Ắ", "A");
                text = text.Replace("ắ", "a");
                text = text.Replace("Ấ", "A");
                text = text.Replace("ấ", "a");
                text = text.Replace("É", "E");
                text = text.Replace("é", "e");
                text = text.Replace("Ế", "E");
                text = text.Replace("ế", "e");
                text = text.Replace("Í", "I");
                text = text.Replace("í", "i");
                text = text.Replace("Ó", "O");
                text = text.Replace("ó", "o");
                text = text.Replace("Ố", "O");
                text = text.Replace("ố", "o");
                text = text.Replace("Ớ", "O");
                text = text.Replace("ớ", "o");
                text = text.Replace("Ú", "U");
                text = text.Replace("ú", "u");
                text = text.Replace("Ứ", "U");
                text = text.Replace("ứ", "u");
                text = text.Replace("Ý", "Y");
                text = text.Replace("ý", "y");

                //'Dấu Hỏi
                text = text.Replace("Ả", "A");
                text = text.Replace("ả", "a");
                text = text.Replace("Ẳ", "A");
                text = text.Replace("ẳ", "a");
                text = text.Replace("Ẩ", "A");
                text = text.Replace("ẩ", "a");
                text = text.Replace("Ẻ", "E");
                text = text.Replace("ẻ", "e");
                text = text.Replace("Ể", "E");
                text = text.Replace("ể", "e");
                text = text.Replace("Ỉ", "I");
                text = text.Replace("ỉ", "i");
                text = text.Replace("Ỏ", "O");
                text = text.Replace("ỏ", "o");
                text = text.Replace("Ổ", "O");
                text = text.Replace("ổ", "o");
                text = text.Replace("Ở", "O");
                text = text.Replace("ở", "o");
                text = text.Replace("Ủ", "U");
                text = text.Replace("ủ", "u");
                text = text.Replace("Ử", "U");
                text = text.Replace("ử", "u");
                text = text.Replace("Ỷ", "Y");
                text = text.Replace("ỷ", "y");

                //'Dấu Ngã   
                text = text.Replace("Ã", "A");
                text = text.Replace("ã", "a");
                text = text.Replace("Ẵ", "A");
                text = text.Replace("ẵ", "a");
                text = text.Replace("Ẫ", "A");
                text = text.Replace("ẫ", "a");
                text = text.Replace("Ẽ", "E");
                text = text.Replace("ẽ", "e");
                text = text.Replace("Ễ", "E");
                text = text.Replace("ễ", "e");
                text = text.Replace("Ĩ", "I");
                text = text.Replace("ĩ", "i");
                text = text.Replace("Õ", "O");
                text = text.Replace("õ", "o");
                text = text.Replace("Ỗ", "O");
                text = text.Replace("ỗ", "o");
                text = text.Replace("Ỡ", "O");
                text = text.Replace("ỡ", "o");
                text = text.Replace("Ũ", "U");
                text = text.Replace("ũ", "u");
                text = text.Replace("Ữ", "U");
                text = text.Replace("ữ", "u");
                text = text.Replace("Ỹ", "Y");
                text = text.Replace("ỹ", "y");

                //'Dẫu Nặng
                text = text.Replace("Ạ", "A");
                text = text.Replace("ạ", "a");
                text = text.Replace("Ặ", "A");
                text = text.Replace("ặ", "a");
                text = text.Replace("Ậ", "A");
                text = text.Replace("ậ", "a");
                text = text.Replace("Ẹ", "E");
                text = text.Replace("ẹ", "e");
                text = text.Replace("Ệ", "E");
                text = text.Replace("ệ", "e");
                text = text.Replace("Ị", "I");
                text = text.Replace("ị", "i");
                text = text.Replace("Ọ", "O");
                text = text.Replace("ọ", "o");
                text = text.Replace("Ộ", "O");
                text = text.Replace("ộ", "o");
                text = text.Replace("Ợ", "O");
                text = text.Replace("ợ", "o");
                text = text.Replace("Ụ", "U");
                text = text.Replace("ụ", "u");
                text = text.Replace("Ự", "U");
                text = text.Replace("ự", "u");
                text = text.Replace("Ỵ", "Y");
                text = text.Replace("ỵ", "y");
                text = text.Replace("Đ", "D");
                text = text.Replace("đ", "d");
            }
            catch
            {
            }
            return text.ToLower();

        }

    }
}
