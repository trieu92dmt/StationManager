
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.TruckInfo
{
    public class UpdateTruckInfoCommand : IRequest<bool>
    {
        //Id
        public Guid TruckInfoId { get; set; }
        //
    }
}
