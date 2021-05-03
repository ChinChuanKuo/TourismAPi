using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using tourismAPi.Models;

namespace tourismAPi.Controllers
{
    [EnableCors("Money")]
    [ApiController]
    [Route("[controller]")]

    public class MoneyController : Controller
    {
        [HttpGet]
        public string Get(string categoryId, string traffic, string items)
        {
            string clientip = Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd() == "::1" ? "127.0.0.1" : Request.HttpContext.Connection.RemoteIpAddress.ToString().TrimEnd();
            return new MoneyClass().GetSearchModels(categoryId, traffic, items, clientip);
        }
    }
}