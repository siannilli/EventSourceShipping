using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SpotCharterViewModel;
using SharedShippingDomainsObjects.ValueObjects;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SpotServiceQueryAPI.Controllers
{
    [Route("api/spot")]
    public class SpotQueryController : Controller
    {
        private readonly ISpotCharterQueryRepository repository;

        public SpotQueryController( ISpotCharterQueryRepository repository)
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

        [HttpGet("search")]
        public IActionResult Search()
        {
            var queryString = HttpContext.Request.Query;            
            var query = this.repository.Find();

            if (!HttpContext.Request.QueryString.HasValue)
                return Ok(new SpotCharterView[] { }); //returns empty array

            try
            {
                // handle query filters
                if (queryString.ContainsKey("vsl"))
                    query = query.Where(s => string.Compare(s.VesselName, queryString["vsl"], true) == 0);

                if (queryString.ContainsKey("cp"))
                    query = query.Where(s => string.Compare(s.CharterpartyName, queryString["cp"], true) == 0);

                DateTime parsedDate = DateTime.MinValue;
                if (queryString.ContainsKey("cpd_min") && DateTime.TryParse(queryString["cpd_min"], out parsedDate))
                    query = query.Where(s => s.CharterpartyDate >= parsedDate);

                if (queryString.ContainsKey("cpd_max") && DateTime.TryParse(queryString["cpd_max"], out parsedDate))
                    query = query.Where(s => s.CharterpartyDate <= parsedDate);

                if (queryString.ContainsKey("laycan") && DateTime.TryParse(queryString["laycan"], out parsedDate))
                    query = query.Where(s => s.Laycan.From <= parsedDate && s.Laycan.To >= parsedDate);

                // handle sort options
                if (queryString.ContainsKey("sort"))
                {
                    var sortOption = queryString["sort"];
                    var propInfo = typeof(SpotCharterView).GetRuntimeProperty(sortOption);

                    if (propInfo == null)
                        throw new ArgumentException($"Invalid sort field {sortOption}", "sort");

                    if (queryString.ContainsKey("sortDirection") && string.Compare(queryString["sortDirection"], "desc", true) == 0)
                        query = query.OrderByDescending((s) => propInfo.GetValue(s, null));
                    else
                        query = query.OrderBy(s => propInfo.GetValue(s, null));
                }

                int skip = 0, take = 10;
                if (queryString.ContainsKey("skip"))
                    int.TryParse(queryString["skip"], out skip);

                if (queryString.ContainsKey("take"))
                    int.TryParse(queryString["take"], out take);

                return ReturnQueryResults(query, skip, take);

            }
            catch (Exception ex)
            {
                return this.BadRequest(ex);
            }
        }

        [HttpGet("recentlyadded")]
        public IActionResult GetRecents()
        {
            return ReturnQueryResults(this.repository.Find().OrderByDescending(s => s.LastUpdate).Take(5));
        }

        [HttpGet("scheduled")]
        public IActionResult GetScheduled()
        {
            return Ok(this.repository.Find(s => s.BillOfLading == null));
        }

        [HttpGet("missingfreightrate")]
        public IActionResult GetMissingFreightRate()
        {
            return this.ReturnQueryResults(this.repository.ChartersMissingFreightRate());
        }

        [HttpGet("missingdemurrage")]
        public IActionResult GetMissingDemurrageTerms()
        {
            return this.ReturnQueryResults(this.repository.ChartersMissingDemurrageTerms());
        }

        [HttpGet("missingporfolio")]
        public IActionResult GetMissingPortfolio()
        {
            return this.ReturnQueryResults(this.repository.ChartersMissingPortfolio());
        }

        private IActionResult ReturnQueryResults(IQueryable<SpotCharterView> predicate, int skip = 0, int take = 10)
        {

            var total = predicate.Count();

            return Ok(
                new
                {
                    total = total,
                    page = skip / take + 1,
                    pages = total / take,
                    results =  predicate.Select(s => new
                    {
                        s.Id,
                        s.CharterpartyDate,
                        s.CharterpartyId,
                        s.CharterpartyName,
                        s.VesselId,
                        s.VesselName,
                        s.Version
                    }).Skip(skip).Take(take),
                });
        }
    }
}
