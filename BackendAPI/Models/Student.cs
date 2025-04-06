namespace BackendAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string FatherOccupation { get; set; }
        public string MotherOccupation { get; set; }
        public string FatherQualification { get; set; }
        public string MotherQualification { get; set; }
        public string AddressResidential { get; set; }
        public string AddressOffice { get; set; }
        public string PhoneResidential { get; set; }
        public string PhoneOffice { get; set; }
        public string GuardianName { get; set; }
        public string Nationality { get; set; }
        public string Religion { get; set; }
        public string AdmissionClass { get; set; }
    }
}
