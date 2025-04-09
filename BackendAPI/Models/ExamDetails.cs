using System.Text.Json.Serialization;

namespace BackendAPI.Models
{
    public class ExamDetails
    {
        public int Id { get; set; }
        public int ExamScheduleId { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }

        [JsonIgnore]
        // Navigation property for ExamSchedule
        public ExamSchedule ExamSchedule { get; set; }
    }
}
