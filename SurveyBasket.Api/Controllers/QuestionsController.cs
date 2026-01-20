using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Abstractions.Consts;
using SurveyBasket.Api.Abstractions;
using SurveyBasket.Api.Authtentication.Filters;
using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Errors;
using SurveyBasket.Api.Services;

namespace SurveyBasket.Api.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        [HttpGet("")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetAll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetAllAsync(pollId, cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return result.ToProblem();
        }


        [HttpGet("{id}")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetById([FromRoute] int pollId, int id, CancellationToken cancellationToken)
        {
            var result = await _questionService.GetByIdAsync(pollId, id, cancellationToken);
            if (result.IsSuccess)
                return Ok(result.Value);
            return result.ToProblem();
        }


        [HttpPost("")]
        [HasPermission(Permissions.AddQuestions)]
        public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await _questionService.AddAsync(pollId, request, cancellationToken);
            return result.IsSuccess 
                ? CreatedAtAction(nameof(GetById), new { pollId = pollId, id = result.Value.Id }, result.Value)
                : result.ToProblem();



        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.UpdateQuestions)]
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
        [HasPermission(Permissions.UpdateQuestions)]
        public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, int id, CancellationToken cancellationToken)
        {
            var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
            if (result.IsSuccess)
                return NoContent();
            return result.ToProblem();
        }




    }
}
