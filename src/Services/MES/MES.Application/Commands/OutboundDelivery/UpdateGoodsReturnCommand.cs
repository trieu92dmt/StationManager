using ISD.Core.Interfaces.Databases;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.OutboundDelivery
{
    public class UpdateGoodsReturnCommand : IRequest<bool>
    {
    }

    public class UpdateGoodsReturnCommandHandler : IRequestHandler<UpdateGoodsReturnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReturnModel> _nkhtRepo;

        public UpdateGoodsReturnCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReturnModel> nkhtRepo)
        {
            _unitOfWork = unitOfWork;
            _nkhtRepo = nkhtRepo;
        }

        public Task<bool> Handle(UpdateGoodsReturnCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
