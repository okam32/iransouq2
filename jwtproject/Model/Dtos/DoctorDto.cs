using System.ComponentModel.DataAnnotations.Schema;

namespace jwtproject.Model.Dtos
{
    public class DoctorDto
    {
    }
    public class Step1Dto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string Mobile { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
    }

    public class Step2Dto
    {
        public string MedicalLicenseNumber { get; set; }
        public string Specialty { get; set; }
        public string SubSpecialty { get; set; }
        public string UniversityName { get; set; }
        public string StudyLocation { get; set; }
        public int GraduationYear { get; set; }
        public string ClinicLicenseImage { get; set; }
        public string ProfileImage { get; set; }
        public string NationalCardImage { get; set; }
        public string CriminalRecordImage { get; set; }
        public string MedicalCardImage { get; set; }
        
        public List<int> CategoryIds { get; set; }
    }

    public class Step3Dto
    {
        public string Workplace { get; set; }
        public string ClinicPhone { get; set; }
        public string ClinicAddress { get; set; }
        public string AccountHolderName { get; set; }
        public string IBAN { get; set; }
        public string BankName { get; set; }
        public string DigitalSignatureImage { get; set; }
    }
    public class DoctorCategoryDto
    {
        public int DoctorId { get; set; }
        public int CategoryId { get; set; }
    }

}
