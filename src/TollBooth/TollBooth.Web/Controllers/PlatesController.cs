using Microsoft.AspNetCore.Mvc;

namespace TollBooth.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatesController : Controller
    {
        [HttpGet("toreview")]
        public ActionResult GetPlatesToReview()
        {
            return Ok();
        }

        [HttpGet("processed")]
        public ActionResult GetProcessedPlates()
        {
            return Ok();
        }
    }
}