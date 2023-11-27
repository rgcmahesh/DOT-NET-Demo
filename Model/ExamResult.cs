namespace Themis.Model
{
    public class ExamResult
    {
        public int ExamResultId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ExamId { get; set; }
        public Exam Exam { get; set; }
        public Double Score { get; set; }
        // Other exam result-related properties
    }
}
