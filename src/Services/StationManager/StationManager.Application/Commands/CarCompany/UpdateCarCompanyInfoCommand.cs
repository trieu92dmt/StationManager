using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using StationManager.Application.DTOs.CarCompany;
using StationManager.Application.Services;

namespace StationManager.Application.Commands.CarCompany
{
    public class UpdateCarCompanyInfoCommand : IRequest<ApiResponse>
    {
        //Id
        public Guid CarCompanyId { get; set; }
        //Mã nhà xe
        public int CarCompanyCode { get; set; }
        //Tên nhà xe
        public string CarCompanyName { get; set; }
        //Email
        public string Email { get; set; }
        //Hotline
        public string Hotline { get; set; }
        //Số điện thoại
        public string PhoneNumber { get; set; }
        //Địa chỉ văn phòng
        public string OfficeAddress { get; set; }
        //Ảnh đại diện
        public string Image { get; set; }
        //Ảnh đại diện mới
        public IFormFile NewImage { get; set; }
        //Ảnh thumnail
        public string Thumnail { get; set; }
        //Ảnh đại diện mới
        public IFormFile NewThumnail { get; set; }
        //Mô tả
        public string Description { get; set; } 
        //Danh sách social media
        public List<SocialMediaResponse> SocialMediaResponses { get; set; } = new List<SocialMediaResponse>();
    }

    public class UpdateCarCompanyInfoCommandHandler : IRequestHandler<UpdateCarCompanyInfoCommand, ApiResponse>
    {
        private readonly IRepository<CarCompanyModel> _carCompanyRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<CarCompany_SocialMedia_MappingModel> _mappingRepo;
        private readonly ICloudinaryService _cloudinaryService;

        public UpdateCarCompanyInfoCommandHandler(IRepository<CarCompanyModel> carCompanyRepo, IUnitOfWork unitOfWork, 
                                                  IRepository<CarCompany_SocialMedia_MappingModel> mappingRepo, ICloudinaryService cloudinaryService)
        {
            _carCompanyRepo = carCompanyRepo;
            _unitOfWork = unitOfWork;
            _mappingRepo = mappingRepo;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse> Handle(UpdateCarCompanyInfoCommand request, CancellationToken cancellationToken)
        {
            //Get car company
            var carCompany = await _carCompanyRepo.GetQuery(x => x.CarCompanyId == request.CarCompanyId)
                                                  .Include(x => x.CarCompany_SocialMedia_MappingModel)
                                                  .FirstOrDefaultAsync();

            if (carCompany == null)
            {
                return new ApiResponse
                {
                    Code = 200,
                    IsSuccess = false,
                    Data = false,
                    Message = "Không tìm thấy nhà xe"
                };
            }

            //Update nhà xe
            carCompany.CarCompanyName = request.CarCompanyName;
            carCompany.Email = request.Email;
            carCompany.Image = request.NewImage != null ? _cloudinaryService.UploadImageCloudinary(request.NewImage) : request.Image;
            carCompany.Hotline = request.Hotline;
            carCompany.PhoneNumber = request.PhoneNumber;
            carCompany.OfficeAddress = request.OfficeAddress;
            carCompany.Thumnail = request.NewThumnail != null ? _cloudinaryService.UploadImageCloudinary(request.NewThumnail) : request.Thumnail;
            carCompany.Description = request.Description;


            //Update thông tin social media
            //var list mapping 
            var mappings = _mappingRepo.GetQuery(x => x.CarCompanyId == carCompany.CarCompanyId);
            //Remove
            _mappingRepo.RemoveRange(mappings);
            

            //Thêm mới social media
            foreach (var item in request.SocialMediaResponses)
            {
                carCompany.CarCompany_SocialMedia_MappingModel.Add(new CarCompany_SocialMedia_MappingModel
                {
                    CarCompany_SocialMedia_MappingId = Guid.NewGuid(),
                    CarCompanyId = carCompany.CarCompanyId,
                    SocialMediaCode = item.SocialMediaCode,
                    Link = item.Link,
                    Actived = true
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return new ApiResponse
            {
                Code = 200,
                IsSuccess = true,
                Data = true,
                Message = "Cập nhật thông tin nhà xe thành công"
            };
        }
    }
}
