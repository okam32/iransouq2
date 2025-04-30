using jwtproject.Data;
using jwtproject.Model.Dtos;
using jwtproject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace jwtproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("RegisterDoctorStep1")]
        public async Task<IActionResult> RegisterDoctorStep1([FromBody] Step1Dto model)
        {
            if (_context.Doctors.Any(d => d.NationalCode == model.NationalCode))
            {
                return BadRequest(new { message = "This national code is already registered." });
            }

            var doctor = new Doctor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                NationalCode = model.NationalCode,
                Mobile = model.Mobile,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email
            };
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return Ok(new { doctorId = doctor.Id, message = "Step 1 completed successfully" });
        }

        //ویرایش اطلاعات مرحله 1
        [HttpPost]
        [Route("editdoctorinformation/{doctorId}")]
        public async Task<IActionResult> EditDoctorInfo([FromRoute] string doctorId, [FromBody] Step1Dto model)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }
                doctor.FirstName = model.FirstName;
                doctor.LastName = model.LastName;
                doctor.NationalCode = model.NationalCode;
                doctor.Mobile = model.Mobile;
                doctor.DateOfBirth = model.DateOfBirth;
                doctor.Email = model.Email;
                doctor.City = model.City;

                await _context.SaveChangesAsync();
                return Ok("doctor informaiton edited successfully");
            }
            return BadRequest("Invalid method");


        }

        // مرحله 2: ثبت اطلاعات تخصصی
        [HttpPost("RegisterDoctorStep2/{doctorId}")]
        public async Task<IActionResult> RegisterDoctorStep2(string doctorId, [FromBody] Step2Dto model)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var doctor = await _context.Doctors
                .Include(d => d.DoctorCategories)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                doctor.MedicalLicenseNumber = model.MedicalLicenseNumber;
                doctor.Specialty = model.Specialty;
                doctor.SubSpecialty = model.SubSpecialty;
                doctor.UniversityName = model.UniversityName;
                doctor.StudyLocation = model.StudyLocation;
                doctor.GraduationYear = model.GraduationYear;
                doctor.ClinicLicenseImage = model.ClinicLicenseImage;
                doctor.ProfileImage = model.ProfileImage;
                doctor.NationalCardImage = model.NationalCardImage;
                doctor.CriminalRecordImage = model.CriminalRecordImage;
                doctor.MedicalCardImage = model.MedicalCardImage;

                // دریافت و ثبت دسته‌بندی‌ها
                // حذف دسته‌بندی‌های قبلی و افزودن دسته‌بندی‌های جدید
                doctor.DoctorCategories.Clear();
                foreach (var categoryId in model.CategoryIds)
                {
                    var category = await _context.Category.FindAsync(categoryId);
                    if (category != null)
                    {
                        doctor.DoctorCategories.Add(new DoctorCategory
                        {
                            DoctorId = doctor.Id,
                            CategoryId = category.Id
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { doctorId = doctor.Id, message = "Step 2 completed successfully" });
            }
            return BadRequest("invalid method.");
        }

        // مرحله 3: ثبت اطلاعات محل کار و بانکی
        [HttpPost("RegisterDoctorStep3/{doctorId}")]
        public async Task<IActionResult> RegisterDoctorStep3(string doctorId, [FromBody] Step3Dto model)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "PUT")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
                if (doctor == null)
                {
                    return NotFound(new { message = "Doctor not found" });
                }

                doctor.Workplace = model.Workplace;
                doctor.ClinicPhone = model.ClinicPhone;
                doctor.ClinicAddress = model.ClinicAddress;
                doctor.AccountHolderName = model.AccountHolderName;
                doctor.IBAN = model.IBAN;
                doctor.BankName = model.BankName;
                doctor.DigitalSignatureImage = model.DigitalSignatureImage;
                doctor.IsCompleted = true;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Doctor registration completed successfully" });
            }
            return BadRequest("Invalid Method.");
        }

        [HttpGet]
        [Route("getalldoctors")]
        public async Task<IActionResult> GetallDoctors()
        {
            var doctors = await _context.Doctors
                .Include(g => g.DoctorCategories)
                .ThenInclude(c => c.Category)
                .Select(doctor => new
                {
                    doctor.Id,
                    doctor.FirstName,
                    doctor.LastName,
                    doctor.NationalCode,
                    doctor.Mobile,
                    doctor.DateOfBirth,
                    doctor.Email,
                    doctor.City,
                    doctor.MedicalLicenseNumber,
                    doctor.Specialty,
                    doctor.SubSpecialty,
                    doctor.UniversityName,
                    doctor.StudyLocation,
                    doctor.GraduationYear,
                    doctor.ClinicLicenseImage,
                    doctor.ProfileImage,
                    doctor.NationalCardImage,
                    doctor.CriminalRecordImage,
                    doctor.MedicalCardImage,
                    doctor.Workplace,
                    doctor.ClinicPhone,
                    doctor.ClinicAddress,
                    doctor.AccountHolderName,
                    doctor.IBAN,
                    doctor.BankName,
                    doctor.DigitalSignatureImage,
                    doctor.IsCompleted,
                    doctor.Rate,
                    categories = doctor.DoctorCategories.Select(dc => dc.Category)
                })
                .ToListAsync();

            return Ok(doctors);
        }
        [HttpGet]
        [Route("getdoctosbycategory/{id}")]
        public async Task<IActionResult> GetDoctosByCategory([FromRoute] int id)
        {
            var doctors = await _context.Doctors
                .Include(g => g.DoctorCategories)
                .ThenInclude(c => c.Category)
                .Where(x => x.DoctorCategories.Any(c => c.CategoryId == id))
                .Select(doctor => new
                {
                    doctor.Id,
                    doctor.FirstName,
                    doctor.LastName,
                    doctor.NationalCode,
                    doctor.Mobile,
                    doctor.DateOfBirth,
                    doctor.Email,
                    doctor.City,
                    doctor.MedicalLicenseNumber,
                    doctor.Specialty,
                    doctor.SubSpecialty,
                    doctor.UniversityName,
                    doctor.StudyLocation,
                    doctor.GraduationYear,
                    doctor.ClinicLicenseImage,
                    doctor.ProfileImage,
                    doctor.NationalCardImage,
                    doctor.CriminalRecordImage,
                    doctor.MedicalCardImage,
                    doctor.Workplace,
                    doctor.ClinicPhone,
                    doctor.ClinicAddress,
                    doctor.AccountHolderName,
                    doctor.IBAN,
                    doctor.BankName,
                    doctor.DigitalSignatureImage,
                    doctor.IsCompleted,
                    doctor.Rate,
                    categories = doctor.DoctorCategories.Select(dc => dc.Category)
                })
                .ToListAsync();

            return Ok(doctors);
        }

        [HttpGet]
        [Route("getdoctorbyid/{doctorid}")]
        public async Task<IActionResult> GetDoctorById([FromRoute] string doctorid)
        {
            var doctor = await _context.Doctors
                .Include (g => g.DoctorCategories)
                .ThenInclude(c => c.Category)
                .Select(doctor => new
                {
                    doctor.Id,
                    doctor.FirstName,
                    doctor.LastName,
                    doctor.NationalCode,
                    doctor.Mobile,
                    doctor.DateOfBirth,
                    doctor.Email,
                    doctor.City,
                    doctor.MedicalLicenseNumber,
                    doctor.Specialty,
                    doctor.SubSpecialty,
                    doctor.UniversityName,
                    doctor.StudyLocation,
                    doctor.GraduationYear,
                    doctor.ClinicLicenseImage,
                    doctor.ProfileImage,
                    doctor.NationalCardImage,
                    doctor.CriminalRecordImage,
                    doctor.MedicalCardImage,
                    doctor.Workplace,
                    doctor.ClinicPhone,
                    doctor.ClinicAddress,
                    doctor.AccountHolderName,
                    doctor.IBAN,
                    doctor.BankName,
                    doctor.DigitalSignatureImage,
                    doctor.IsCompleted,
                    doctor.Rate,
                    categories = doctor.DoctorCategories.Select(dc => dc.Category)
                })
                .FirstOrDefaultAsync(x => x.Id == doctorid);

            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }
        [HttpPost]
        [Route("deletedoctor/{doctorid}")]
        public async Task<IActionResult> DeletDoctor([FromRoute] string doctorid)
        {
            var methodOverride = Request.Headers["X-HTTP-Method-Override"].ToString();
            if (methodOverride == "DELETE")
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.Id == doctorid);
                if (doctor == null)
                {
                    return NotFound();
                }
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                return Ok("doctor deleted successfully.");
            }
            return BadRequest("Invalid method");

        }
        [HttpPost]
        [Route("addrate/{id}")]
        public async Task<IActionResult> AddRate([FromBody]int score, [FromRoute]string id)
        {
            // دریافت کاربر فعلی از Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var rate = new RateUser();
            rate.Score = score;
            rate.DoctorId = id;
            rate.UserId = userId;

            await _context.RateUsers.AddAsync(rate);
            await _context.SaveChangesAsync();

            return Ok("Score Add Successfully.");

        }
        [HttpGet]
        [Route("getdoctorrate/{docid}")]
        public async Task<IActionResult> GetRate([FromRoute]string docid)
        {
            var doc = await _context.Doctors.FirstOrDefaultAsync(x => x.Id == docid);

            var ratelist = await _context.RateUsers.Where(x => x.DoctorId == docid).ToListAsync();
            double sum = 0;
            int count = 0;
            double rateavg;
            foreach (var rate in ratelist)
            {
                sum = rate.Score;
            }
            rateavg = sum / ratelist.Count;

            doc.Rate = rateavg;

            await _context.SaveChangesAsync();

            return Ok(doc);
        }
    }
}
