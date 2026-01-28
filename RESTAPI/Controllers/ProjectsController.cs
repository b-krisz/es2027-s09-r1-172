using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RESTAPI.Controllers
{
    
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProjectsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("JsonApi");
                HttpResponseMessage response = await client.GetAsync("projects");
                
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return NotFound(e.Message);                
            }
        }
    }
}