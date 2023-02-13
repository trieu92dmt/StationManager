using MediatR;
using MES.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MES.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class XNVLGCController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IXNVLGCQuery _query;

        public XNVLGCController(IMediator mediator, IXNVLGCQuery query)
        {
            _mediator = mediator;
            _query = query;
        }
    }
}
