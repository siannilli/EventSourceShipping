using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SpotCharterViewModel;
using SharedShippingDomainsObjects.ValueObjects;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SpotServiceQueryAPI.Controllers
{
    [Route("api/[controller]")]
    public class SpotController : Controller
    {
        private readonly ISpotCharterQueryRepository repository;

        public SpotController( ISpotCharterQueryRepository repository)
        {
            this.repository = repository;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try
            {
                var spotId = new SpotCharterId(Guid.Parse(id));
                var spot = repository.GetBySpotCharterId(spotId);

                return Ok(spot);                
            }

            catch(FormatException ex)
            {
                return BadRequest();
            }
            catch(Exception ex)
            {
                return this.StatusCode(500, ex);
            }
        }

        [HttpGet("recentlyadded")]
        public IActionResult GetRecents()
        {
            try
            {                
                return this.Ok(this.repository.Find().OrderByDescending(s => s.LastUpdate).Take(5));
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex);
            }
        }

    }
}
