using DockerComposeDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DockerComposeDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ColorContext colorContext;

        public ValuesController(ColorContext colorContext)
        {
            this.colorContext = colorContext;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Color>> GetColorItems()
        {
            return colorContext.ColorItems;
        }
    }
}
