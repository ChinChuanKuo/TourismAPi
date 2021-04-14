using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using tourismAPi.Models;

namespace tourismAPi.Controllers
{
    [EnableCors("Insert")]
    [ApiController]
    [Route("[controller]")]
    public class InsertController : Controller
    {
        //List<Dictionary<string, object>> items
        [HttpPost]
        public statusModels Post([FromForm] bool traffic, [FromForm] bool location, [FromForm] string items)
        {
            string clientip = Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd() == "::1" ? "127.0.0.1" : Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd();
            return new InsertClass().GetSendModels(traffic, location, items, clientip);
        }
    }
}