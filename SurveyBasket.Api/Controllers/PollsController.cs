using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Contracts.Requests;
using SurveyBasket.Api.Contracts.Responses;
using SurveyBasket.Api.Models;
using SurveyBasket.Api.Services;

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


        [HttpGet("")]
        public IActionResult GetAll()
        {
            var polls = _pollService.GetAll();
            var response = polls.Adapt<IEnumerable<PollResponse>>();
            return Ok(response);
        }


        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var poll = _pollService.GetById(id);

            if (poll == null)
                return NotFound();
            var response = poll.Adapt<PollResponse>();
            return Ok(response);
        }


        [HttpPost("")]
        public IActionResult Add([FromBody]CreatePollRequest request)// [FromServices] IValidator<CreatePollRequest> validator)
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
            var newPoll = _pollService.Add(request.Adapt<Poll>());
            return CreatedAtAction(nameof(GetById), new { id = newPoll.Id }, newPoll);
        }


        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id,[FromBody] CreatePollRequest request)
        {
            var isUpdated = _pollService.Update(id, request.Adapt<Poll>());
            if(!isUpdated)
                return NotFound();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int id)
        {
            var isDeleted = _pollService.Delete(id);
            if (!isDeleted)
                return NotFound();
            return NoContent();
        }
    }
}
