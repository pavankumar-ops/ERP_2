using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;

using System.Net.Mail;

namespace ERP_Placement.Controllers
{
    public class Student : Controller
    {
        private readonly StudentDAL _dal;
        private readonly IWebHostEnvironment _env;

        public Student(IConfiguration config, IWebHostEnvironment env)
        {
            _dal = new StudentDAL(config);
            _env=env;
        }

        public IActionResult Student_Enroll()
        {

            var skills = _dal.GetSkills();   // get skills from database
            ViewBag.Skills = skills;

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
                .ToLower();

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
                "photo"
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

            TempData["success"] = "1";
            return RedirectToAction("Student_Enroll");
        }

        public IActionResult StudentDB()
        {
            string studentId = HttpContext.Session.GetString("StudentId");

            // --- Applied Interviews ---
            var interviews = _dal.AppliedJobStudDB(studentId);

            DataTable dt = new DataTable();
            dt.Columns.Add("CompanyName");
            dt.Columns.Add("JobTitle");
            dt.Columns.Add("JobLocation");
            dt.Columns.Add("Salary");
            dt.Columns.Add("InterviewDate");
            dt.Columns.Add("InterviewTime");

            foreach (var i in interviews)
            {
                dt.Rows.Add(i.CompanyName, i.JobTitle, i.JobLocation, i.Salary, i.InterviewDate, i.InterviewTime);
            }

            ViewBag.AppliedInterviews = dt;

            // --- Assigned Interviews ---
            var assign = _dal.AssignJobStudDB(studentId); // Replace with real assigned interviews DAL later

            DataTable dt1 = new DataTable();
            dt1.Columns.Add("CompanyName");
            dt1.Columns.Add("JobTitle");

            foreach (var i1 in assign)
            {
                dt1.Rows.Add(i1.CompanyName, i1.JobTitle); // ✅ Fixed
            }

            ViewBag.AssignInterviews = dt1;


            var myOfferLetter = _dal.OfferLetterDBStud(studentId);

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("CompanyName");
            dt2.Columns.Add("JobTitle");
            dt2.Columns.Add("JobLocation");
            dt2.Columns.Add("Salary");
            dt2.Columns.Add("InterviewId", typeof(int));


            foreach (var i2 in myOfferLetter)
            {
                dt2.Rows.Add(i2.CompanyName, i2.JobTitle, i2.JobLocation, i2.Salary, i2.InterviewId); // ✅ Fixed
            }

            ViewBag.OfferLetters = dt2;

            //ViewBag.Applied = 70;
            //ViewBag.Selected = 30;

            var test = _dal.GetTestList();

            int TestCount = test.Count;
            ViewBag.TestCount = TestCount;
            var statuscount = _dal.CountStudDB(studentId);

            // Count by status
            int appliedCount = statuscount.Count(x => x.Status == "Applied");
            int assignCount = statuscount.Count(x => x.Status == "Assign");
            int selectedCount = statuscount.Count(x => x.Status == "Selected");

            int total = appliedCount + selectedCount;

            // double appliedPercent = 0;
            double selectedPercent = 0;

            if (total > 0)
            {
                selectedPercent = (selectedCount * 100.0) / total;
            }


            // Send counts to view
            ViewBag.Applied = appliedCount;
            ViewBag.Assign = assignCount;
            ViewBag.Selected = selectedCount;


            ViewBag.total = total;
            ViewBag.SelectedPercentage = Math.Round(selectedPercent);


            return View();
        }




        [HttpGet]
        public IActionResult TestList()
        {
            var testList = _dal.GetTestList();   // call DAL method
            return View(testList);
        }
        public IActionResult StartTest(int testId)
        {
            var questions = _dal.GetQuestionsByTestId(testId);
            return View(questions);
        }
        [HttpPost]
        public IActionResult StartTest(int testId, IFormCollection form)
        {
            var questions = _dal.GetQuestionsByTestId(testId);

            foreach (var q in questions)
            {
                string key = "q_" + q.QuestionId;

                if (!string.IsNullOrEmpty(form[key]))
                {
                    q.SelectedOption = form[key];
                }
            }

            return View("Result", questions);
        }


        public IActionResult ScheduledInterviews()
        {
            string studentId = HttpContext.Session.GetString("StudentId");
            // string studentId = "1011";

            DataTable dt = _dal.GetStudentInterviews(studentId);

            ViewBag.InterviewList = dt;

            return View();
        }


        [HttpPost]

        public IActionResult ApplyInterview(int InterviewId, string HREmailId, int VacancyId, int StudentId)
        {

            SendInterviewMail(StudentId, HREmailId);
            _dal.UpdateInterviewStatus(VacancyId);
            TempData["Success"] =  "Interview Applied Successfully";

            return Json(new { success = true, message = "Interview Applied Successfully" });
        }

        [HttpPost]


        public ActionResult SendInterviewMail(int studentId, string hrEmail)
        {
            try
            {
                string studentIdStr = studentId.ToString();

                DataTable dt = _dal.GetStudDetails(studentIdStr);

                if (dt.Rows.Count == 0)
                {
                    return Content("Student not found");
                }

                DataRow student = dt.Rows[0];

                string studfirstname = student["FirstName"].ToString();
                string studlastname = student["LastName"].ToString();
                string resumePath = student["Resume"].ToString().TrimStart('/');

                string studentName = studfirstname + " " + studlastname;

                string fullResumePath = Path.Combine(_env.WebRootPath, resumePath);

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("placementerp@gmail.com");
                mail.To.Add(hrEmail);

                mail.Subject = "Interview Scheduled - Student Resume";

                mail.Body = "Dear HR,<br/><br/>" +
                            "Interview has been scheduled for the following student:<br/><br/>" +
                            "<b>Student Name :</b> " + studentName + "<br/><br/>" +
                            "Please find the attached resume.<br/><br/>" +
                            "Regards,<br/>Placement Cell";

                mail.IsBodyHtml = true;

                if (System.IO.File.Exists(fullResumePath))
                {
                    mail.Attachments.Add(new Attachment(fullResumePath));
                }

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("placementerp@gmail.com", "erkkhoqzdiiuigyj");
                smtp.EnableSsl = true;

                smtp.Send(mail);

                return Content("Mail Sent Successfully");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }


        public IActionResult AssignCompany()
        {
            string studentId = HttpContext.Session.GetString("StudentId");
            // string studentId = "1011";

            DataTable dt = _dal.AssignComapnyStud(studentId);

            ViewBag.InterviewList = dt;

            return View();



        }
        public IActionResult AppliedJob()
        {
            string studentId = HttpContext.Session.GetString("StudentId");
            // string studentId = "1011";

            DataTable dt = _dal.AppliedJobStud(studentId);

            ViewBag.InterviewList = dt;

            return View();



        }


        public IActionResult DownloadOfferLetter(int interviewId)
        {
            // 🔹 Get file path from DB
            string filePath = _dal.GetOfferLetterPath(interviewId);

            if (string.IsNullOrEmpty(filePath))
            {
                TempData["error"] = "Offer letter not found.";
                return RedirectToAction("MakeOfferLetter");
            }

            // 🔹 Full physical path
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/').Replace("/", "\\"));

            if (!System.IO.File.Exists(fullPath))
            {
                TempData["error"] = "Offer letter file missing on server.";
                return RedirectToAction("MakeOfferLetter");
            }

            string fileName = Path.GetFileName(fullPath);

            // 🔹 Return file as download
            return PhysicalFile(fullPath, "application/pdf", fileName);
        }

        [HttpGet]
        public JsonResult GetSkills()
        {
            var skills = _dal.GetSkills();
            return Json(skills);
        }

        [HttpPost]
        public JsonResult AddSkill(string skillName)
        {
            _dal.AddSkill(skillName);
            return Json(new { success = true });
        }

        public IActionResult About()
        {
            return View("About");
        }

        public IActionResult TPO()
        {
            return View("TPO");
        }

        public IActionResult FAQ()
        {
            return View("FAQ's");
        }

        public IActionResult EditStudent()
        {
            //string studentId = "1019";
             string studentId = HttpContext.Session.GetString("StudentId");
            var skills = _dal.GetSkills();   // get skills from database
            ViewBag.Skills = skills;
            var data = _dal.GetStudent(studentId);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(
    string MobileNo,
    string AlternateMobile,
    string Email,
    string Acadamic_Year,
    string BTech_Percentages,
    string BTech_Passing_Year,
    string Skills,
    string FirstName,
    string MiddleName,
    string LastName,
    string StudentId,
    string Personal_Photo,          // old profile path
    string BTech_Document_Path,     // old btech path
    string ResumePath,              // old resume path
    IFormFile Profile_photo,
    IFormFile BTech_File,
    IFormFile Resume
)
        {
            StudentRegistration model = new StudentRegistration();

            // ✅ BASIC DATA
            model.MobileNo = MobileNo;
            model.AlternateMobile = AlternateMobile;
            model.Email = Email;
            model.Acadamic_Year = Acadamic_Year;
            model.BTech_Percentages = BTech_Percentages;
            model.BTech_Passing_Year = BTech_Passing_Year;
            model.Skills = Skills;
            model.StudentId = StudentId;

            // 🔥 STUDENT NAME (FOR FILE NAME)
            string studentName = $"{FirstName}_{MiddleName}_{LastName}"
                .Replace(" ", "")
                .ToLower();

            // 🔥 COMMON FILE SAVE FUNCTION
            async Task<string> SaveFile(IFormFile file, string folderName, string suffix)
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

                return $"/uploads/{folderName}/{fileName}";
            }

            // =====================================================
            // 🔥 PROFILE PHOTO
            // =====================================================
            if (Profile_photo != null)
            {
                model.Personal_Photo = await SaveFile(Profile_photo, "Profile", "photo");
            }
            else
            {
                model.Personal_Photo = Personal_Photo; // keep old
            }

            // =====================================================
            // 🔥 BTECH FILE
            // =====================================================
            if (BTech_File != null)
            {
                model.BTech_Document_Path = await SaveFile(BTech_File, "BTECH", "BTECH");
            }
            else
            {
                model.BTech_Document_Path = BTech_Document_Path; // keep old
            }

            // =====================================================
            // 🔥 RESUME
            // =====================================================
            if (Resume != null)
            {
                model.ResumePath = await SaveFile(Resume, "RESUME", "RESUME");
            }
            else
            {
                model.ResumePath = ResumePath; // keep old
            }

            // 👉 CALL DAL HERE
            _dal.UpdateStudent(model);

            return Json(new { success = true });
        }
    }
 }
