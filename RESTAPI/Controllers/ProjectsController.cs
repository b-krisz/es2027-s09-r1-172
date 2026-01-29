using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        public async Task<IActionResult> GetSelect([FromQuery] string id)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("JsonApi");
                
                HttpResponseMessage response = await client.GetAsync("projects/" + id);
                string responseRaw = await response.Content.ReadAsStringAsync();
                
                Project project = JsonSerializer.Deserialize<Project>(responseRaw);
                if (project == null)
                    return NotFound();
                
                return Ok(project);
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

        [HttpPut("toggleTurbine")]
        public async Task<IActionResult> PatchToggleTurbine([FromQuery] string id, [FromQuery] int x, [FromQuery] int y)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("JsonApi");
                
                HttpResponseMessage projectsResponse = await client.GetAsync("projects/" + id);
                string projectResponseRaw = await projectsResponse.Content.ReadAsStringAsync();
                
                Project project = JsonSerializer.Deserialize<Project>(projectResponseRaw);

                if (project == null)
                    return NotFound("Project doesnt exist");

                if (project.cells[x + y * 20].hasTurbine)
                {
                    project.cells[x + y * 20].hasTurbine = false;
                    string turbineRequestFalse = JsonSerializer.Serialize(project);
                    HttpResponseMessage turbineResponseFalse = await client.PutAsync("projects/" + id + "", new StringContent(turbineRequestFalse, Encoding.UTF8, "application/json"));
                    return Ok();
                }
                if (project.cells[x + y * 20].type == "Mountain" || project.cells[x + y * 20].type == "Lake")
                    return BadRequest("Cant place on Mountain or Lake");
                
                // OFFSETS
                int leftOffset = (x - 1);
                int rightOffset = (x + 1);
                int topOffset = (y - 1);
                int bottomOffset = (y + 1);

                Cell? lt = null;
                Cell? rt = null;
                Cell? lb = null;
                Cell? rb = null;
                Cell? l = null;
                Cell? r = null;
                Cell? t = null;
                Cell? b = null;
                
                // OFFSET OUT OF BOUNDS
                if (leftOffset >= 0 && leftOffset < 20)
                {
                    l = project.cells[leftOffset + y * 20];      // LEFT OFFSET
                    
                    if (topOffset >= 0 && topOffset < 20)
                    {
                        lt = project.cells[leftOffset + topOffset * 20]; // LEFT TOP OFFSET
                    }
                    
                    if (bottomOffset >= 0 && bottomOffset < 20)
                    {
                        lb = project.cells[leftOffset + bottomOffset * 20]; // LEFT BOTTOM OFFSET
                    }
                }
                if (rightOffset >= 0 && rightOffset < 20)
                {
                    r = project.cells[rightOffset + y * 20];      // RIGHT OFFSET
                    
                    if (topOffset >= 0 && topOffset < 20)
                    {
                        rt = project.cells[rightOffset + topOffset * 20]; // RIGHT TOP OFFSET
                    }
                    
                    if (bottomOffset >= 0 && bottomOffset < 20)
                    {
                        rb = project.cells[rightOffset + bottomOffset * 20]; // RIGHT BOTTOM OFFSET
                    }
                }
                if (topOffset >= 0 && topOffset < 20)
                {
                    t = project.cells[(x) + topOffset * 20];      // TOP OFFSET
                }
                if (bottomOffset >= 0 && bottomOffset < 20)
                {
                    b = project.cells[(x) + bottomOffset * 20];      // BOTTOM OFFSET
                }

                if (lt is { hasTurbine: true } || rt is { hasTurbine: true } ||
                    lb is { hasTurbine: true } || rb is { hasTurbine: true } ||
                    l is { hasTurbine: true } || r is { hasTurbine: true } ||
                    t is { hasTurbine: true } || b is { hasTurbine: true })
                {
                    return BadRequest("Turbine in range");
                }
                
                project.cells[x + y * 20].hasTurbine = true;
                string turbineRequestTrue = JsonSerializer.Serialize(project);
                HttpResponseMessage turbineResponseTrue = await client.PutAsync("projects/" + id + "", new StringContent(turbineRequestTrue, Encoding.UTF8, "application/json"));
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}