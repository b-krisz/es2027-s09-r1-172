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

        [HttpGet("select")]
        public async Task<IActionResult> GetSelect([FromQuery] string? id)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("JsonApi");
                HttpResponseMessage response = await client.GetAsync("projects?id=" + id);
                string responseRaw = await response.Content.ReadAsStringAsync();
                List<Project> projects = JsonSerializer.Deserialize<List<Project>>(responseRaw) ?? new List<Project>();
                
                if (projects.Count == 0)
                    return NotFound();
                return Ok(projects.FirstOrDefault());
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
                List<Project> filteredProjects = allProjects.Where(project =>
                    project.name.Contains(query ?? "") || project.id.Contains(query ?? "")).ToList();
                
                return Ok(filteredProjects.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);                
            }
        }
    }
}