using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase

    {
        // <summary>
        /// test
        /// </summary>
        /// <returns></returns>
        [HttpGet("test1")]
        public string Test()
        {
            return "test OK";
        }

    }
}
