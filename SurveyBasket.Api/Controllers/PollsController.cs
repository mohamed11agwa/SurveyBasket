using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Services;
using System.Threading;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var polls = await _pollService.GetAllAsync();
            var response = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var poll = await _pollService.GetByIdAsync(id);

            if (poll == null)
                return NotFound();
            var response = poll.Adapt<PollResponse>();
            return Ok(response);
        }


        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody]PollRequest request, CancellationToken cancellationToken)// [FromServices] IValidator<CreatePollRequest> validator)
        {
            //var validationResult  = validator.Validate(request);
            //if(!validationResult.IsValid)
            //{
            //    var modelState = new ModelStateDictionary();
            //    validationResult.Errors.ForEach(error =>
            //    {
            //        modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            //    });
            //    return ValidationProblem(modelState);
            //}
            var newPoll =await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request
            , CancellationToken cancellationToken)
        {
            var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>(), cancellationToken);
            if (!isUpdated)
                return NotFound();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var isUpdated = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
            if (!isUpdated)
                return NotFound();
            return NoContent();
        }
    }
}
