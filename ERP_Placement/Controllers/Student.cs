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

        //   [HttpPost]
        //   public async Task<IActionResult> StudSave(
        //StudentRegistration model,
        //IFormFile Personal_Photo,
        //IFormFile SSC_Document_Path,
        //IFormFile HSC_Document_Path,
        //IFormFile Diploma_Document_Path,
        //IFormFile BTech_Document_Path,
        //IFormFile Resume)
        //   {
        //       string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        //       if (!Directory.Exists(uploadPath))
        //           Directory.CreateDirectory(uploadPath);

        //       async Task<string> SaveFile(IFormFile file)
        //       {
        //           if (file == null || file.Length == 0)
        //               return null;

        //           string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //           string fullPath = Path.Combine(uploadPath, fileName);

        //           using (var stream = new FileStream(fullPath, FileMode.Create))
        //           {
        //               await file.CopyToAsync(stream);
        //           }

        //           return "/uploads/" + fileName;
        //       }

        //       // Save file paths in model
        //       model.Personal_Photo = await SaveFile(Personal_Photo);
        //       model.SSC_Document_Path = await SaveFile(SSC_Document_Path);
        //       model.HSC_Document_Path = await SaveFile(HSC_Document_Path);
        //       model.Diploma_Document_Path = await SaveFile(Diploma_Document_Path);
        //       model.BTech_Document_Path = await SaveFile(BTech_Document_Path);
        //       model.ResumePath = await SaveFile(Resume);

        //       model.RegistrationDate = DateTime.Now;

        //      _dal.InsertStudentDetails(model).ToString();



        //       //model.StudentId = studentId;
        //       //_dal.InsertStudentEducation(model);

        //       TempData["success"] = "Student Registration Saved Successfully!";
        //       return RedirectToAction("Student_Enroll");
        //   }


        [HttpPost]
        public async Task<IActionResult> StudSave(
        StudentRegistration model,
        IFormFile Personal_Photo,
        IFormFile SSC_Document_Path,
        IFormFile HSC_Document_Path,
        IFormFile Diploma_Document_Path,
        IFormFile BTech_Document_Path,
        IFormFile Resume)
        {
            // 🔹 Student name for file
            string studentName =
                $"{model.FirstName}_{model.MiddleName}_{model.LastName}"
                .Replace(" ", "")
                .ToUpper();

            async Task<string> SaveFile(
                IFormFile file,
                string folderName,
                string suffix)
            {
                if (file == null || file.Length == 0)
                    return null;

                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    folderName
                );

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string extension = Path.GetExtension(file.FileName);
                string fileName = $"{studentName}_{suffix}{extension}";
                string fullPath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // ✅ EXACT PATH YOU ASKED FOR
                return $"/uploads/{folderName}/{fileName}";
            }

            model.Personal_Photo = await SaveFile(
                Personal_Photo,
                "Profile",
                "PHOTO"
            );

            model.SSC_Document_Path = await SaveFile(
                SSC_Document_Path,
                "SSC",
                "SSC"
            );

            model.HSC_Document_Path = await SaveFile(
                HSC_Document_Path,
                "HSC",
                "HSC"
            );

            model.Diploma_Document_Path = await SaveFile(
                Diploma_Document_Path,
                "DIPLOMA",
                "DIPLOMA"
            );

            model.BTech_Document_Path = await SaveFile(
                BTech_Document_Path,
                "BTECH",
                "BTECH"
            );

            model.ResumePath = await SaveFile(
                Resume,
                "RESUME",
                "RESUME"
            );

            model.RegistrationDate = DateTime.Now;

            _dal.InsertStudentDetails(model);

            TempData["success"] = "Student Registration Saved Successfully!";
            return RedirectToAction("Student_Enroll");
        }

        public IActionResult StudentDB()
        {
            return View( );
        }
    }

}
