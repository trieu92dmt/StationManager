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
    public class SaveGoodsReturnCommand : IRequest<bool>
    {

    }

    public class SaveOutboundDeliveryCommandHandler : IRequestHandler<SaveGoodsReturnCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReturnModel> _nkhtRepo;

        public SaveOutboundDeliveryCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReturnModel> nkhtRepo)
        {
            _unitOfWork = unitOfWork;
            _nkhtRepo = nkhtRepo;
        }

        public Task<bool> Handle(SaveGoodsReturnCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
