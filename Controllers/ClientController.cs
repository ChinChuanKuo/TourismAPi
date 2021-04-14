using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using tourismAPi.Models;

namespace tourismAPi.Controllers
{
    [EnableCors("Client")]
    [ApiController]
    [Route("[controller]")]

    public class ClientController : Controller
    {
        [HttpGet]
        public string Get(string userid, string username)
        {
            string clientip = Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd() == "::1" ? "127.0.0.1" : Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd();
            return new ClientClass().GetSearchModels(userid, username, clientip);
        }
    }
}