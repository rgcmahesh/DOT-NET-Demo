namespace Themis.Model
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public string QuestionText { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string option3 { get; set; }
        public string option4 { get; set; }
        public int Answer { get; set; }
        public Exam Exams { get; set; }
    }
}
