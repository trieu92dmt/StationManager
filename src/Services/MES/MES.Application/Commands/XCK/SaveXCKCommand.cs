using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XCK
{
    public class SaveXCKCommand : IRequest<bool>
    {
        //1. Plant
        public string Plant { get; set; }
        //2. Reservation
        public string MyProperty { get; set; }
        //3. Reservation Item
        //4. Material
        //5. Material Desc
        //6. Stor. Loc
        //7. Receiving Storage Location
        //8. Batch
        //9. Unit
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
