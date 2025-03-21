using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace MicroServicesProject.Controllers
{
    public class BaseController : Controller
    {

        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    }
}
