using Microsoft.AspNetCore.Mvc;
using text_compare.Core.Interface;
using text_compare.Models;

namespace text_compare.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CompareController : ControllerBase
    {

        private readonly ICompareText _compareTextService;

        public CompareController(ICompareText compareTextService)
        {
            _compareTextService = compareTextService;
        }

        [HttpPost("LineByLine")]
        public IActionResult CompareLineByLine([FromBody] TextComparisonModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.BaseText) || string.IsNullOrEmpty(model.ModifiedText))
            {
                return BadRequest("Please provide valid input texts.");
            }

            var changes = _compareTextService.CompareLineByLine(model.BaseText, model.ModifiedText);

            return Ok(changes);
        }


        [HttpPost("WordByWord")]
        public IActionResult CompareWordByWord([FromBody] TextComparisonModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.BaseText) || string.IsNullOrEmpty(model.ModifiedText))
            {
                return BadRequest("Please provide valid input texts.");
            }

            var changes = _compareTextService.CompareWordByWord(model.BaseText, model.ModifiedText);

            return Ok(changes);
        }

        [HttpPost("LetterByLetter")]
        public IActionResult CompareLetterByLetter([FromBody] TextComparisonModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.BaseText) || string.IsNullOrEmpty(model.ModifiedText))
            {
                return BadRequest("Please provide valid input texts.");
            }

            var changes = _compareTextService.CompareLetterByLetter(model.BaseText, model.ModifiedText);

            return Ok(changes);
        }
    }
}
