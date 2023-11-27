using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Themis.Data;
using Themis.Model;

namespace Themis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : Controller
    {
        private readonly ThemisContext _courseContext;

        public CoursesController(ThemisContext themisContext)
        {
            _courseContext = themisContext;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var coursesWithUsers = await _courseContext.Courses.ToListAsync();

            return coursesWithUsers;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _courseContext.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _courseContext.Courses.Add(course);
            await _courseContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, Course courseObj)
        {
            var course = await _courseContext.Courses.FindAsync(id);
            if (course != null)
            {
                course.CourseName = courseObj.CourseName;
                course.Description = courseObj.Description;
                await _courseContext.SaveChangesAsync();
                return Ok(course);
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            if (_courseContext.Courses == null)
            {
                return NotFound();
            }
            var course = await _courseContext.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _courseContext.Courses.Remove(course);
            await _courseContext.SaveChangesAsync();

            return Ok();
        }

        // Add PUT and DELETE actions as needed

        private bool CourseExists(int id)
        {
            return _courseContext.Courses.Any(e => e.CourseId == id);
        }
    }
}