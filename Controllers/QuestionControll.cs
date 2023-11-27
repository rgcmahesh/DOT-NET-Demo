using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Themis.Data;
using Themis.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Themis.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class QuestionControll : ControllerBase
    {
        private readonly ThemisContext _questionContext;

        public QuestionControll(ThemisContext _themisContext)
        {
            _questionContext = _themisContext;

        }

        // GET: api/<QuestionController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> Get()
        {
            var questions = await _questionContext.Questions.ToListAsync();

            return questions;
        }

        // POST api/<QuestionController>
        [HttpPost]
        public async Task<ActionResult<Question>> Post(Question question)
        {
            try
            {
                // Check if the associated user exists
                var existingExam = await _questionContext.Exams.FindAsync(question.ExamId);

                if (existingExam == null)
                {
                    return NotFound($"Question with ID {question.ExamId} not found.");
                }

                // Assign the associated user to the question
               // question.Exams = existingUser;

                // Add the question to the context
                _questionContext.Questions.Add(question);

                // Save changes
                await _questionContext.SaveChangesAsync();

                // Return the created Question
                return Ok(question.ExamId);
               // return CreatedAtAction(nameof(Get), new { id = question.QuestionId }, question);
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // GET api/<QuestionController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> Get(int id)
        {
            var question = await _questionContext.Questions.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // PUT api/<QuestionController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Question questionObj)
        {
            var question = await _questionContext.Questions.FindAsync(id);
            if (question != null)
            {
                question.ExamId = questionObj.ExamId;
                question.QuestionText = questionObj.QuestionText;
                question.option1 = questionObj.option1;
                question.option2 = questionObj.option2;
                question.option3 = questionObj.option3;
                question.option4 = questionObj.option4;
                question.Answer = questionObj.Answer;
                await _questionContext.SaveChangesAsync();
                return Ok(question);
            }
            return NotFound();
        }

        // DELETE api/<F>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_questionContext.Questions == null)
            {
                return NotFound();
            }
            var question = await _questionContext.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _questionContext.Questions.Remove(question);
            await _questionContext.SaveChangesAsync();

            return Ok();
        }
    }
}
