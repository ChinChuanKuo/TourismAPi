using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using tourismAPi.Models;

namespace tourismAPi.Controllers
{
    [EnableCors("Group")]
    [ApiController]
    [Route("[controller]")]
    public class GroupController : Controller
    {
        [HttpPost]
        public sitemsModels Post([FromForm] string licenses)
        {
            string clientip = Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd() == "::1" ? "127.0.0.1" : Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd();
            return new GroupClass().GetSearchModels(licenses, clientip);
        }
    }
}