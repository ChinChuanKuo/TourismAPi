using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using tourismAPi.Models;

namespace tourismAPi.Controllers
{
    [EnableCors("Birthday")]
    [ApiController]
    [Route("[controller]")]

    public class BirthdayController : Controller
    {
        [HttpGet]
        public int Get(string birthday)
        {
            string clientip = Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd() == "::1" ? "127.0.0.1" : Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd();
            return new BirthdayClass().GetSearchModels(birthday, clientip);
        }
    }
}