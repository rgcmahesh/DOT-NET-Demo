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
    public class ExamController : ControllerBase
    {
        private readonly ThemisContext _examContext;

        public ExamController(ThemisContext _themisContext)
        {
            _examContext = _themisContext;

        }

        // GET: api/<ExamController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exam>>> Get()
        {
            var examsWithQuestionsAndCourse = await _examContext.Exams
                    .Include(exam => exam.Questions)
                    .Include(exam => exam.Course) // Include the Course
                    .ToListAsync();

            var examDtos = examsWithQuestionsAndCourse.Select(exam => new Exam
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                CourseId = exam.CourseId,
                DurationMinutes = exam.DurationMinutes,
                UserId = exam.UserId,
                Course = new Course
                {
                    CourseId = exam.Course.CourseId,
                    CourseName = exam.Course.CourseName,
                    // Map other properties as needed
                },
                Questions = exam.Questions.Select(question => new Question
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    // Map other properties as needed
                })
                .ToList()
            }).ToList();

            return examDtos;
        }

        // GET api/<ExamController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exam>> Get(int id)
        {

            var examWithQuestionsAndCourse = await _examContext.Exams
                .Include(exam => exam.Questions)
                .Include(exam => exam.Course)
                .FirstOrDefaultAsync(exam => exam.ExamId == id);

            if (examWithQuestionsAndCourse == null)
            {
                return NotFound($"Exam with ID {id} not found.");
            }

            var examDto = new Exam
            {
                ExamId = examWithQuestionsAndCourse.ExamId,
                ExamName = examWithQuestionsAndCourse.ExamName,
                CourseId = examWithQuestionsAndCourse.CourseId,
                DurationMinutes = examWithQuestionsAndCourse.DurationMinutes,
                UserId = examWithQuestionsAndCourse.UserId,
                Course = new Course
                {
                    CourseId = examWithQuestionsAndCourse.Course.CourseId,
                    CourseName = examWithQuestionsAndCourse.Course.CourseName,
                    // Map other properties as needed
                },
                Questions = examWithQuestionsAndCourse.Questions.Select(question => new Question
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    option1 = question.option1,
                    option2 = question.option2,
                    option3 = question.option3,
                    option4 = question.option4,
                    Answer = question.Answer,
                    // Map other properties as needed
                })
                .ToList()
            };

            return examDto;

        }

        // POST api/<ExamController>
        [HttpPost]
        public async Task<ActionResult<Exam>> Post(Exam exam)
        {
            try
            {
                // Ensure that the associated User exists in the database
                var existingUser = await _examContext.Users.FindAsync(exam.UserId);

                if (existingUser == null)
                {
                    return NotFound($"Exam with ID {exam.UserId} not found.");
                }

                // Associate the User with the Exam
                exam.User = existingUser;

                // Add the Exam to the context and save changes
                _examContext.Exams.Add(exam);
                await _examContext.SaveChangesAsync();

                // Return the created Exam
                return CreatedAtAction(nameof(Get), new { id = exam.ExamId }, exam);
            }
            catch (Exception ex)
            {
                // Log or handle the exception appropriately
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        // PUT api/<ExamController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Exam examObj)
        {
            var exam = await _examContext.Exams.FindAsync(id);
            if (exam != null)
            {
                exam.UserId = examObj.UserId;
                exam.CourseId = examObj.CourseId;
                exam.ExamName = examObj.ExamName;
                exam.DurationMinutes = examObj.DurationMinutes;
                await _examContext.SaveChangesAsync();
                return Ok(exam);
            }
            return NotFound();
        }

        // DELETE api/<ExamController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            if (_examContext.Exams == null)
            {
                return NotFound();
            }
            var exam = await _examContext.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            _examContext.Exams.Remove(exam);
            await _examContext.SaveChangesAsync();

            return Ok();
        }
    }
}

