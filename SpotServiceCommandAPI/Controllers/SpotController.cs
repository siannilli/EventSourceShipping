using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using BaseDomainObjects;
using SharedShippingDomainsObjects.ValueObjects;
using SpotCharterDomain;
using SpotCharterServices;
using SpotCharterServices.Commands;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SpotHandlerCommandAPI.Controllers
{
    [Route("api/[controller]")]
    public class SpotController : Controller
    {
        private readonly ICommandHandler<CreateSpotCharter> createCharterHandler;
        private readonly ICommandHandler<ChangeCharterparty> changeCharterpartyHandler;
        private readonly ICommandHandler<ChangeVessel> changeVesselHandler;
        private readonly ICommandHandler<ChangeDemurrageRate> changeDemurrageHandler;
        private readonly ICommandHandler<ChangeBillOfLading> changeBLHandler;
        private readonly ICommandHandler<ChangeLaycan> changeLaycanHandler;

        public SpotController(
            ICommandHandler<CreateSpotCharter> createCharterHandler,
            ICommandHandler<ChangeCharterparty> changeCharterpartyHandler,
            ICommandHandler<ChangeVessel> changeVesselHandler,
            ICommandHandler<ChangeDemurrageRate> changeDemurrageHandler,
            ICommandHandler<ChangeBillOfLading> changeBLHandler,
            ICommandHandler<ChangeLaycan> changeLaycanHandler
            )
        {
            this.createCharterHandler = createCharterHandler;
            this.changeCharterpartyHandler = changeCharterpartyHandler;
            this.changeVesselHandler = changeVesselHandler;
            this.changeDemurrageHandler = changeDemurrageHandler;
            this.changeBLHandler = changeBLHandler;
            this.changeLaycanHandler = changeLaycanHandler;
        }

        // POST api/spot Creates a new spot
        [HttpPost]
        public IActionResult Create([FromBody]CreateSpotCharter createCommand)
        {
            if (createCommand == null)
                return BadRequest(new ArgumentNullException("CreateCommand"));
            
            return ProcessCommand(createCharterHandler, createCommand);            
        }

        // PUT api/values/5
        [HttpPut("{id}/changecharterparty")]
        public IActionResult ChangeCharterparty(string id, [FromBody]ChangeCharterparty changeCommand)
        {
            if (changeCommand == null)
                return BadRequest(new ArgumentNullException("command"));

            if (!changeCommand.SpotCharterId.Equals(new SpotCharterId(Guid.Parse(id))))
                return this.BadRequest(new ArgumentException($"Command aggregate id {changeCommand.SpotCharterId} does not match with URI parameter {id}."));

            return this.ProcessCommand(changeCharterpartyHandler, changeCommand);
        }

        // PUT api/values/5
        [HttpPut("{id}/changevessel")]
        public IActionResult ChangeVessel(string id, [FromBody]ChangeVessel changeCommand)
        {
            if (changeCommand == null)
                return BadRequest(new ArgumentNullException("command"));

            if (!changeCommand.SpotCharterId.Equals(new SpotCharterId(Guid.Parse(id))))
                return this.BadRequest(new ArgumentException($"Command aggregate id {changeCommand.SpotCharterId} does not match with URI parameter {id}."));

            return this.ProcessCommand(changeVesselHandler, changeCommand);
        }



        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return BadRequest();
        }

        private IActionResult ProcessCommand<TCommand>(            
            ICommandHandler<TCommand> commandHandler, TCommand command) where TCommand: ICommand
        {
            try
            {
                commandHandler.Handle(command);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }
    }
}
