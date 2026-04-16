using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace ERP_Placement.Controllers
{
    public class TrainerController : Controller
    {

        private readonly StudentDAL _dal;

        public TrainerController(IConfiguration config)
        {
            _dal = new StudentDAL(config);
        }

        // GET: TrainerController1
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult TraineerDB()
        {
            var recentStudents = _dal.GetRecentStudents();
            ViewBag.LatestJobs = _dal.GetLatestJobs();
            ViewBag.UpcomingInterviews = _dal.GetUpcomingInterviews();
            ViewBag.RecentStudents = recentStudents;

            var placementData = _dal.GetPlacementActivity();

            ViewBag.AssignedCount = placementData.AssignedCount;
            ViewBag.AppliedCount = placementData.AppliedCount;
            ViewBag.SelectedCount = placementData.SelectedCount;
            ViewBag.RecentTests = _dal.GetRecentTests();

            var counts = _dal.GetDashboardCounts();

            ViewBag.StudentCount = counts.students;
            ViewBag.CompanyCount = counts.companies;
            ViewBag.InterviewCount = counts.interviews;
            ViewBag.OfferCount = counts.offers;
            return View();
        }
        // GET: TrainerController1/Details/5
        public ActionResult Stud_List(Placement_Coordinator_Model model)
        {
            DataTable dt = _dal.GetAllStudents();
            return View(dt);
        }

        [HttpGet]
        public IActionResult CompanyRegistration()
        {
            return View("CompanyRegistration");
        }

        [HttpGet]
        public IActionResult CompanyList()
        {
            
            DataTable dt = _dal.GetAllCompany();
           
            return View("CompanyList", dt);
            
        }



        [HttpGet]
        public IActionResult AddVacancy(JobVacancyModel model, string id)
        {

            HttpContext.Session.SetString("CompanyId", id);
            model.CompanyId = Convert.ToInt32(id);

            DataTable dt = _dal.GetVaccany(id);

            ViewBag.VacancyList = dt;

            return View(model);
        
        
        }

        public IActionResult Companydropdown()
        {

            DataTable dt = _dal.GetAllCompany();
            ViewBag.Compantlist = dt;

            return View("ScheduleInterview", dt);

        }


        [HttpPost]
        public JsonResult SaveVacancy(JobVacancyModel model)
        {
            try
            {
                int result = _dal.SaveVacancy(model);

                if (result > 0)
                {
                    return Json(new { success = true, message = "Vacancy Added Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to Add Vacancy" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        




        // PAGE LOAD
        [HttpGet]
        public IActionResult ScheduleInterview(string vacancyId)
        {
            ViewBag.VacancyList = _dal.GetVacancyDropdown();
            ViewBag.StudentList = _dal.GetStudentDropdown();
            ViewBag.InterviewList = _dal.GetInterviewList();
            ViewBag.Compantlist = _dal.GetAllCompany();
            return View();
        }


        // SAVE INTERVIEW
        [HttpPost]
        public JsonResult SaveInterview(InterviewScheduleModel model)
        {
            int result = _dal.SaveInterview(model);

            if (result > 0)
            {
                return Json(new { success = true, message = "Interview Scheduled Successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to Schedule Interview" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CompanySave(
    Placement_Coordinator_Model model,
    IFormFile CompanyLogo)
        {
            
            string companyName = model.CompanyName
                .Replace(" ", "")
                .ToUpper();

            async Task<string> SaveFile(IFormFile file, string folderName, string suffix)
            {
                if (file == null || file.Length == 0)
                    return null;

                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "img",
                    folderName
                );

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string extension = Path.GetExtension(file.FileName);
                string fileName = $"{companyName}_{suffix}{extension}";
                string fullPath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                // ✅ Path stored in DB
                return $"/img/{folderName}/{fileName}";
            }

            // 🔥 SAVE LOGO
            model.CompanyLogo = await SaveFile(
                CompanyLogo,
                "comapny",   // spelling preserved as you want
                "LOGO"
            );

            model.RegisteredBy = "Placement Coordinator";
            model.CompanyRegistrationDate = DateTime.Now;

            int companyId = _dal.InsertCompany(model);



            return RedirectToAction("CompanyList");
        }

        public ActionResult Approve(string id)
        {
            // 1️⃣ Get student data
            DataTable dt = _dal.GetStudById(id);

            if (dt == null || dt.Rows.Count == 0)
            {
                TempData["error"] = "Student not found";
                return RedirectToAction("Stud_List");
            }

            // ✅ Get first row correctly
            DataRow row = dt.Rows[0];

            string firstName = row["FirstName"].ToString();
            string email = row["Email"].ToString();

            // 2️⃣ Generate password (Firstname + 4 digits)
            Random rnd = new Random();
            string password = firstName + rnd.Next(1000, 9999);

            // 3️⃣ Update DB (save generated password)
            _dal.ApproveStudent(id, password);

            // 4️⃣ Send Email
            SendApprovalEmail(email, firstName, password);

            TempData["success"] = "Student approved and password sent!";
            return RedirectToAction("Stud_List");
        }




        private void SendApprovalEmail(string toEmail, string name, string password)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("placementerp@gmail.com", "Placement Team");
                mail.To.Add(toEmail);
                mail.Subject = "Placement Portal Login Approved";

                mail.Body = $@"
                        Hello {name},

                        Your registration has been approved.

                        Login Credentials:
                        Username: {toEmail}
                        Password: {password}

                        Please change your password after login.

                        Regards,
                        Placement Team";

                mail.IsBodyHtml = false;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new NetworkCredential(
                    "placementerp@gmail.com",
                    "erkkhoqzdiiuigyj"
                );

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error sending email: " + ex.Message;
            }
        }


        public IActionResult Reject(int id)
        {
            int result = _dal.RejectStudentStudlist(id);

            if (result > 0)
            {
                TempData["success"] = "Student Rejected Successfully!";
            }
            else
            {
                TempData["error"] = "Something went wrong!";
            }

            return RedirectToAction("Stud_List");
        }

        public ActionResult MakeOfferLetter()
        {
        
            DataTable dt = _dal.GetInterviewAppliedStudents();
            return View(dt);
        }

        public IActionResult RejectStudentoffer(int InterviewId)
        {


            _dal.RejectStudent(InterviewId);
            TempData["Message"] = "Student rejected successfully!";

            return RedirectToAction("MakeOfferLetter");
        }
        public IActionResult DeleteVacancy(int Id)
        {
            string CompanyId = HttpContext.Session.GetString("CompanyId");

            _dal.DeleteVacancy(Id);

            TempData["Message"] = "Delete Vacancy successfully!";

            return RedirectToAction("AddVacancy", new { id = CompanyId });
        }

        [HttpPost]
        public async Task<IActionResult> UploadOfferLetter(
        int StudentId,

        int CompanyId,
        int VacancyId,
        int InterviewId,
        IFormFile OfferLetterFile)
        {
            if (OfferLetterFile == null || OfferLetterFile.Length == 0)
            {
                TempData["error"] = "Please upload offer letter";
                return RedirectToAction("InterviewStudentList");
            }

            string uploadPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "OfferLetter"
            );

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // 🔹 DAL call for student + company name
            DataTable dt = _dal.OfferletterName(StudentId.ToString());

            string firstName = "";
            string lastName = "";
            string companyName = "";
            //string Email = "nickaher1004@gmail.com";
            string Email = "salunkhevaish753@gmail.com";
           // string Email = "";

            if (dt.Rows.Count > 0)
            {
                firstName = dt.Rows[0]["FirstName"].ToString().Replace(" ", "").ToLower();
                lastName = dt.Rows[0]["LastName"].ToString().Replace(" ", "").ToLower();
                companyName = dt.Rows[0]["CompanyName"].ToString().Replace(" ", "").ToLower();
               //
               //
               Email = dt.Rows[0]["Email"].ToString().Replace(" ", "").ToLower();
            }

            string extension = Path.GetExtension(OfferLetterFile.FileName);

            string fileName = $"{firstName}_{lastName}_{companyName}_offer{extension}";

            string fullPath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await OfferLetterFile.CopyToAsync(stream);
            }

            string offerPath = $"/uploads/OfferLetter/{fileName}";

            OfferLetterModel model = new OfferLetterModel
            {
                StudentId = StudentId,
                CompanyId = CompanyId,
                VacancyId = VacancyId,
                InterviewId = InterviewId,
                OfferLetterFile = offerPath
            };
            SendOfferLetter(firstName, lastName, companyName, Email, offerPath);
            SelectStudent(InterviewId);
            _dal.InsertOfferLetter(model);


            TempData["success"] = "Offer Letter Uploaded Successfully";

            return RedirectToAction("MakeOfferLetter");
        }


        private void SendOfferLetter(string firstName, string lastName ,string companyName, string email, string offerPath)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("placementerp@gmail.com", "Placement Team");
                    mail.To.Add(email);

                    mail.Subject = "Congratulations! You Are Selected";

                    mail.Body = $@"
Dear {firstName} {lastName},

Congratulations!

You have been selected in {companyName}.

Please find your Offer Letter attached.

Regards,
Placement Team
";

                    string fullPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        offerPath.TrimStart('/')
                    );

                    if (System.IO.File.Exists(fullPath))
                    {
                        Attachment attachment = new Attachment(fullPath);
                        mail.Attachments.Add(attachment);
                    }

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;

                        smtp.Credentials = new NetworkCredential(
                            "placementerp@gmail.com",
                            "erkkhoqzdiiuigyj"
                        );

                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Error sending email: " + ex.Message;
            }
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

        public void SelectStudent(int id)
        {
            _dal.SelectStudent(id);

          
            
        }

        [HttpPost]
        public ActionResult RejectStudent(int id)
        {
            _dal.RejectStudent(id);
            return RedirectToAction("InterviewStudentList");
        }

        public ActionResult Test()
        {
            return View("Test");
        }

        public ActionResult TestList()
        {
            DataTable dt = _dal.GetAllTest();

            List<Placement_Coordinator_Model> list = new List<Placement_Coordinator_Model>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Placement_Coordinator_Model
                {
                    TestId = Convert.ToInt32(row["TestId"]),
                    TestName = row["TestName"].ToString(),
                    Branch = row["Branch"].ToString(),
                    CreatedBy = row["CreatedBy"].ToString(),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            return View(list);
        }


        [HttpPost]
        public JsonResult SubmitTest([FromBody] Placement_Coordinator_Model model)
        {
            if (model == null || model.QuestionList == null)
            {
                return Json(new { success = false, message = "Invalid data received" });
            }

            if (model.QuestionList.Count != 20)
            {
                return Json(new { success = false, message = "Exactly 20 questions required" });
            }

            int testId = _dal.InsertTest(model);

            return Json(new
            {
                success = true,
                message = "Test saved successfully",
                TestId = testId
            });
        }
       



        // GET: TrainerController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TrainerController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainerController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrainerController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TrainerController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrainerController1/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public IActionResult ViewOfferLetter(int interviewId)
        {
            if (interviewId == 0)
                return BadRequest();

            string offerPath = _dal.ViewOfferLetter(interviewId);

            if (string.IsNullOrEmpty(offerPath))
                return NotFound("Offer Letter not found");

            // PDF file open in browser
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", offerPath.TrimStart('/').Replace("/", "\\"));

            var mimeType = "application/pdf";
            return PhysicalFile(fullPath, mimeType);
        }


        public JsonResult GetTestQuestions(int testId)
        {
            DataTable dt = _dal.GetTestQuestionsByTestId(testId);

            var questions = dt.AsEnumerable().Select(x => new
            {
                QuestionNo = x["QuestionNo"].ToString(),
                Question = x["Question"].ToString(),
                A = x["OptionA"].ToString(),
                B = x["OptionB"].ToString(),
                C = x["OptionC"].ToString(),
                D = x["OptionD"].ToString(),
                Correct = x["CorrectOption"].ToString()
            });

            return Json(questions);
        }


        public IActionResult DeleteTest(int id)
        {
            _dal.DeleteTest(id);

            TempData["msg"] = "Test Deleted Successfully";

            return RedirectToAction("TestList");
        }


        public JsonResult GetVacancyByCompany(int companyId)
        {
            DataTable dt = _dal.GetVacancyByCompany(companyId);

            var list = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    VacancyId = row["VacancyId"],
                    JobTitle = row["JobTitle"].ToString()
                });
            }

            return Json(list);
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
    }
}
