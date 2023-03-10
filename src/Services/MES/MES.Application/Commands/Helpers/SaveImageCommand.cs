using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.Helpers
{
    public class SaveImageCommand : IRequest<Guid>
    {
        public IFormFile Image { get; set; }
    }

    public class SaveImageCommandHandler : IRequestHandler<SaveImageCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ImageTest> _imgRep;

        public SaveImageCommandHandler(IUnitOfWork unitOfWork, IRepository<ImageTest> imgRep)
        {
            _unitOfWork = unitOfWork;
            _imgRep = imgRep;
        }

        public async Task<Guid> Handle(SaveImageCommand request, CancellationToken cancellationToken)
        {
            var rs = Guid.NewGuid();
            //Convert img to bytes arr
            using (var ms = new MemoryStream())
            {
                await request.Image.CopyToAsync(ms);
                var fileBytes = ms.ToArray();

                _imgRep.Add(new ImageTest
                {
                    Id = rs,
                    Image = fileBytes
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return rs;
        }
    }
}
