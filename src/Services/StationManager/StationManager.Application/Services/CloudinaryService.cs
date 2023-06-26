using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.Services
{
    public interface ICloudinaryService
    {
        string UploadImageCloudinary(IFormFile file);
    }

    public class CloudinaryService : ICloudinaryService
    {
        public string UploadImageCloudinary(IFormFile file)
        {
            var cloudinary = new Cloudinary(new Account("minhtrieu-cloudinary", "568465589926894", "abhM0GqLGiZf2OuZM4qaP4-nqPw"));

            //Upload
            using (var stream = file.OpenReadStream())
            {

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = "my-test-image"
                };

                var uploadResult = cloudinary.Upload(uploadParams);

                //Transformation
                cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(100).Height(150).Crop("fill")).BuildUrl("my-test-image");

                return uploadResult.Uri.AbsoluteUri;
            }
        }
    }
}
