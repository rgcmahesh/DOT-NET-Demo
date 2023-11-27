using System.ComponentModel.DataAnnotations.Schema;

namespace Themis.Model
{
    public class Exam
    {

        public int ExamId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string ExamName { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation properties for the one-to-many relationship
        [System.Text.Json.Serialization.JsonIgnore]

        [ForeignKey("UserId")]
        public User User { get; set; }
        public Course Course { get; set; }

        public List<Question> Questions { get; set; }

        public List<ExamResult> ExamResults { get; set; }
    }
}
