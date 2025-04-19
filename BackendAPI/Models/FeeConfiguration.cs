using System.ComponentModel.DataAnnotations;

namespace BackendAPI.Models
{
    public class FeeConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public string AcademicYear { get; set; }

        [Required]
        public decimal AdmissionFee { get; set; }

        [Required]
        public decimal TuitionFee { get; set; }

        [Required]
        public decimal OtherFee { get; set; }

        [Required]
        public decimal TotalFee { get; set; }
    }
}
