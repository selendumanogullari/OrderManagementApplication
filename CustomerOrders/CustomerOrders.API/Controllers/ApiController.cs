using Microsoft.AspNetCore.Mvc;

[ApiVersion("1")]
[Route("api/v{version:ApiVersion}/[controller]")]
[ApiController]
namespace CustomerOrders.API.Controllers
{
    public class ApiController: ControllerBase
    {
    }
}