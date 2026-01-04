using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetAllAsync(pollId, cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return result.ToProblem();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int pollId, int id, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetByIdAsync(pollId, id, cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return result.ToProblem();
        }


        [HttpPost("")]
        public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionService.AddAsync(pollId, request, cancellationToken);
            return result.IsSuccess 
                ? CreatedAtAction(nameof(GetById), new { pollId = pollId, id = result.Value.Id }, result.Value)
                : result.ToProblem();



        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute]int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionService.UpdateAsync(pollId, id, request, cancellationToken);
            //if (result.IsSuccess)
            //    return NoContent();
            //return result.Error.Equals(QuestionErrors.DuplicatedQuestionContent)
            //    ? result.ToProblem(StatusCodes.Status409Conflict)
            //    : result.ToProblem(StatusCodes.Status404NotFound);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }


        [HttpPost("{id}/toggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, int id, CancellationToken cancellationToken)
        {
            var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
            if (result.IsSuccess)
                return NoContent();
            return result.ToProblem();
        }




    }
}
