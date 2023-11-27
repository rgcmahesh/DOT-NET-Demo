using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Themis.Data;
using Themis.Model;

namespace Themis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamResultController : ControllerBase
    {

        private readonly ThemisContext _resultContext;

        public ExamResultController(ThemisContext _themisContext)
        {
            _resultContext = _themisContext;

        }

        // GET: api/<ExamResultController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExamResult>>> Get()
        {
            var examResult = await _resultContext.ExamResult.ToListAsync();

            return examResult;
        }

        // POST api/<ExamResultController>
        [HttpPost]
        public async Task<ActionResult<Exam>> Post(ExamResult examResult)
        {

            try
            {
                // Ensure that the associated Exam exists in the database
                var existingExam = await _resultContext.Exams.FindAsync(examResult.ExamId);

                if (existingExam == null)
                {
                    return NotFound($"Exam Result with ID {examResult.ExamResultId} not found.");
                }

                // Add the Exam Result to the context and save changes
                _resultContext.ExamResult.Add(examResult);
                await _resultContext.SaveChangesAsync();

                // Return the created Exam
                return Ok(examResult.ExamId);
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{userId}/{examId}")]
        public async Task<ActionResult<ExamResult>> GetExamResult(int userId, int examId)
        {

            var examResult = await _resultContext.ExamResult
                .Where(r => r.UserId == userId)
                .Where(r => r.ExamId == examId)
                .ToListAsync();

            return Ok(examResult);
        }

    }

}
