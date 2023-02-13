using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XCK
{
    public class SaveXCKCommand : IRequest<bool>
    {

    }

    public class SaveXCKCommandHandler : IRequestHandler<SaveXCKCommand, bool>
    {
        public SaveXCKCommandHandler()
        {
        }

        public Task<bool> Handle(SaveXCKCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
