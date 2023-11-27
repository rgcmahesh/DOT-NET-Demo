using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Themis.Data;
using Themis.Model;
using System.Text;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Themis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ThemisContext _authContext;

        public UserController(ThemisContext themisContext)
        {
            _authContext = themisContext;

        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            
            if (userObj == null)
                return BadRequest();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x => x.Email == userObj.Email);
            
            if (user == null)
                return NotFound(new { Message = "User not found!" });
 
            if (user != null && !BCrypt.Net.BCrypt.Verify(userObj.Password, user.Password))
            return NotFound(new { Message = "Incorrect Password! Please check your password!" });

            user.Token = CreateJwt(user);

            var home = user.Role == "learner" ? "/learner" : "/admin";


            return Ok(new
            {
                access_token = user.Token,
                data = user,
                home = home
            });

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            // check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist" });

            //check username
            var pass = CheckPasswordStrength(userObj.Password);
            if(!string.IsNullOrEmpty(pass)) 
            {
                return BadRequest(new { Message = pass.ToString() });
            }

           

            //userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            // hash password
            userObj.Password = BCrypt.Net.BCrypt.HashPassword(userObj.Password);
            userObj.Role = userObj.Role == "admin" ? "admin" : "learner";
            userObj.Token = "";
            //userObj.CourseId = userObj.Course.CourseId;
          

            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return Ok(userObj);

        }

        private async Task<bool> CheckEmailExistAsync(string email)
        {
             return await _authContext.Users.AnyAsync(x => x.Email == email);
        }
        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
                sb.Append("Minimum password length should be 8 " + Environment.NewLine);
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
                sb.Append("Password should be AlphaNumeric " + Environment.NewLine);
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter " + Environment.NewLine);
            return sb.ToString();
        }

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Themis Login Personal Access Token");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            var credentails = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentails
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var usersWithCourses = await _authContext.Users.Where(user => user.Role == "learner").ToListAsync();

            return usersWithCourses;

        }

        [HttpGet("GetAllFaculties")]
        [Authorize]
        public async Task<ActionResult<User>> GetAllFaculties()
        {
            return Ok(await _authContext.Users.Where(user => user.Role == "faculty").ToListAsync());
        }

        [Authorize]
        [HttpPost("AddFaculty")]
        public async Task<IActionResult> AddFaculty([FromBody] User userObj)
        {
            if (userObj == null)
            {
                return BadRequest();
            }

            // check email
            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist" });

            //check username
            var pass = CheckPasswordStrength(userObj.Password);
            if (!string.IsNullOrEmpty(pass))
            {
                return BadRequest(new { Message = pass.ToString() });
            }

            //userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            // hash password
            userObj.Password = BCrypt.Net.BCrypt.HashPassword(userObj.Password);
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();

            return Ok(userObj);

        }

        [Authorize]
        [HttpPut]
        [Route("EditFaculty/{id}")]
        public async Task<IActionResult> EditFaculty(int id, User userObj)
        {
            var user = await _authContext.Users.FindAsync(id);
            if (user != null)
            {
                user.FirstName = userObj.FirstName;
                user.LastName = userObj.LastName;
                user.Email = userObj.Email;
                user.UserName = userObj.UserName;
                user.Password = BCrypt.Net.BCrypt.HashPassword(userObj.Password);
                await _authContext.SaveChangesAsync();
                return Ok(user);
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet]
        [Route("ShowFaculty/{id}")]
        public async Task<IActionResult> ShowFaculty(int id)
        {
            var user = await _authContext.Users.FindAsync(id);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete]
        [Route("DeleteFaculty/{id}")]
        public async Task<IActionResult> DeleteFaculty(int id)
        {
            if (_authContext.Users == null)
            {
                return NotFound();
            }
            var user = await _authContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _authContext.Users.Remove(user);
            await _authContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("AdminDashboard")]
        public async Task<IActionResult> AdminDashboard()
        {
            return Ok(new
            {
                students = await _authContext.Users.Where(user => user.Role == "learner").CountAsync(),
                faculty = await _authContext.Users.Where(user => user.Role == "faculty").CountAsync(),
                courses = await _authContext.Courses.CountAsync(),
                exams = await _authContext.Exams.CountAsync(),
                questions = await _authContext.Questions.CountAsync()
            });
        }

    }

}
