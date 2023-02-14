using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XCK
{
    public class UpdateXCKCommand : IRequest<bool>
    {
    }

    public class UpdateXCK
    {

    }

    public class UpdateXCKCommandHanler : IRequestHandler<UpdateXCKCommand, bool>
    {
        public Task<bool> Handle(UpdateXCKCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
