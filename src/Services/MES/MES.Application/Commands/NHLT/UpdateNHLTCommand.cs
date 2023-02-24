using Core.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NHLT
{
    public class UpdateNHLTCommand : IRequest<ApiResponse>
    {
        public List<UpdateNHLT> UpdateNHLTs { get; set; } = new List<UpdateNHLT>();
    }

    public class UpdateNHLT
    {
        //Id
        //Plant
        //Material
        //Sloc
        //Customer
        //confỉm quantity
        //SL kèm bao bì

    }

    public class UpdateNHLTCommandHandler : IRequestHandler<UpdateNHLTCommand, ApiResponse>
    {
        public UpdateNHLTCommandHandler()
        {
        }

        public Task<ApiResponse> Handle(UpdateNHLTCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
