using System.ComponentModel.DataAnnotations;

namespace jwtproject.Model
{
    public class Doctor
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // مرحله 1: اطلاعات شخصی
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? NationalCode { get; set; }
        public string? Mobile { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }

        // مرحله 2: اطلاعات تخصصی
        public string? MedicalLicenseNumber { get; set; }
        public string? Specialty { get; set; }
        public string? SubSpecialty { get; set; }
        public string? UniversityName { get; set; }
        public string? StudyLocation { get; set; }
        public int? GraduationYear { get; set; }
        public string? ClinicLicenseImage { get; set; }
        public string? ProfileImage { get; set; }
        public string? NationalCardImage { get; set; }
        public string? CriminalRecordImage { get; set; }
        public string? MedicalCardImage { get; set; }
        public List<DoctorCategory> DoctorCategories { get; set; }

        // مرحله 3: اطلاعات محل کار و بانکی
        public string? Workplace { get; set; }
        public string? ClinicPhone { get; set; }
        public string? ClinicAddress { get; set; }
        public string? AccountHolderName { get; set; }
        public string? IBAN { get; set; }
        public string? BankName { get; set; }
        public string? DigitalSignatureImage { get; set; }

        public bool IsCompleted { get; set; } = false;
        public double Rate { get; set; }
    }
}
