
namespace Themis.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string DeviceToken { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public int Status { get; set; }

        public ICollection<Exam> Exams { get; set; }

        // Other user-related properties
        public List<ExamResult> ExamResults { get; set; }
    }
}
