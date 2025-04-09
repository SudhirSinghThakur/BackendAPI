using System.Text.Json.Serialization;

namespace BackendAPI.Models
{
    public class ExamSchedule
    {
        public int Id { get; set; }
        public string Grade { get; set; }
        public string ExamType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        // Navigation property for related ExamDetails
        public ICollection<ExamDetails> ExamDetails { get; set; }
    }
}
