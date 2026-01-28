using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommandLineTool.Models;
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

        [HttpGet("paginate")]
        public async Task<IActionResult> GetPaginate()
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("JsonApi");
                HttpResponseMessage response = await client.GetAsync("projects");
                
                return Ok(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("search")]
        public async Task<IActionResult> GetSearch([FromQuery] string? query)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("JsonApi");
                HttpResponseMessage response = await client.GetAsync($"projects");
                string responseRaw = await response.Content.ReadAsStringAsync();
                List<Project> allProjects = JsonSerializer.Deserialize<List<Project>>(responseRaw) ?? new List<Project>();
                
                return Ok(allProjects.Where(project => project.name.Contains(query ?? "") || project.id.Contains(query ?? "")));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);                
            }
        }
    }
}