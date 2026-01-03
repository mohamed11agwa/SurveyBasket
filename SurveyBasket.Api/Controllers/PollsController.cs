using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Polls;
using SurveyBasket.Api.Entities;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController : ControllerBase
    {
        private readonly IPollService _pollService;

        public PollsController(IPollService pollService)
        {
            _pollService = pollService;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var polls = await _pollService.GetAllAsync();
            return Ok(polls);
        }


        [HttpGet("current")]
        public async Task<IActionResult> GetCurrent()
        {
            var polls =  await _pollService.GetCurrentAsync();
            return Ok(polls);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _pollService.GetByIdAsync(id);

            //if (poll == null)
            //    return NotFound();
            //var response = poll.Adapt<PollResponse>();
            //return Ok(response);
            
            return result.IsSuccess
                ? Ok(result.Value) 
                //: Problem(statusCode: StatusCodes.Status404NotFound, title:result.Error.Code, detail:result.Error.Description);
                  : result.ToProblem(StatusCodes.Status404NotFound);
        }


        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)// [FromServices] IValidator<CreatePollRequest> validator)
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
            var result = await _pollService.AddAsync(request, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
                : result.ToProblem(StatusCodes.Status409Conflict);
            
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request
            , CancellationToken cancellationToken)
        {
            var result = await _pollService.UpdateAsync(id, request, cancellationToken);
            //if (!isUpdated)
            //    return NotFound();
            //return NoContent();
            //return result.IsSuccess ? NoContent() : NotFound(result.Error);

            //return result.IsSuccess ?
            //    NoContent() 
            //    //: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
            //    : result.ToProblem(StatusCodes.Status404NotFound);

            if (result.IsSuccess)
                return NoContent();
            return result.Error.Equals(PollErrors.PollNotFound)
                ? result.ToProblem(StatusCodes.Status404NotFound)
                 : result.ToProblem(StatusCodes.Status409Conflict);
                
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollService.DeleteAsync(id, cancellationToken);
            //if (!isDeleted)
            //    return NotFound();
            //return NoContent();
        return result.IsSuccess
                ? NoContent()
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
                : result.ToProblem(StatusCodes.Status404NotFound);
        }

        [HttpPut("{id}/togglePublish")]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
            //if (!isUpdated)
            //    return NotFound();
            //return NoContent();
            return result.IsSuccess 
                ? NoContent()
                //: Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
                : result.ToProblem(StatusCodes.Status404NotFound);
        }
    }
}
