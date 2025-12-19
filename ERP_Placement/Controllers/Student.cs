using ERP_Placement.Models;
using ERP_Placement.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
namespace ERP_Placement.Controllers
{
    public class Student : Controller
    {
        private readonly StudentDAL _dal;

        public Student(IConfiguration config)
        {
            _dal = new StudentDAL(config);
        }

        public IActionResult Student_Enroll()
        {
            return View();
        }

        [HttpPost]
        public IActionResult StudSave(StudentRegistration model,
    IFormFile Personal_Photo,
    IFormFile SSC_Document_Path,
    IFormFile HSC_Document_Path,
    IFormFile Diploma_Document_Path,
    IFormFile BTech_Document_Path,
    IFormFile Resume)

        {
            

            // ---------------------------
            // 1️⃣ Save Uploaded Files
            // ---------------------------
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            async Task<string> SaveFile(IFormFile file)
            {
                if (file == null) return null;
                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string fullPath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return "/uploads/" + fileName;
            }

            //model.SSC_Document_Path = await SaveFile(model.SSC_File);
            //model.HSC_Document_Path = await SaveFile(model.HSC_File);
            //model.Diploma_Document_Path = await SaveFile(model.Diploma_File);
            //model.BTech_Document_Path = await SaveFile(model.BTech_File);

            model.RegistrationDate = DateTime.Now;

            // ---------------------------
            // 2️⃣ Save StudentDetails

            string studentId = _dal.InsertStudentDetails(model).ToString();


            // ---------------------------
            // 3️⃣ Save StudentEducation

            model.StudentId = studentId;
            _dal.InsertStudentEducation(model);

            TempData["success"] = "Student Registration Saved Successfully!";
            return RedirectToAction("Student_Enroll");
        }
    }
}
