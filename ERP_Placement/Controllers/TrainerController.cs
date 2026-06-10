using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
//using QRCoder;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
namespace ERP_Placement.Controllers
{
    public class TrainerController : Controller
    {

        private readonly StudentDAL _dal;
        private readonly IWebHostEnvironment _env;

        public TrainerController(IConfiguration config, IWebHostEnvironment env)
        {
            _dal = new StudentDAL(config);
            _env=env;
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
            ViewBag.PremiumUserCount = counts.premium;
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
            string StudentId = model.StudentId.ToString();
            if (result > 0)
            {
                // Student Details
                DataTable studentDt = _dal.GetStudentDetailsById(StudentId);

                // Interview Details
                DataTable interviewDt = _dal.GetInterviewDetails(StudentId);

                if (studentDt.Rows.Count > 0 && interviewDt.Rows.Count > 0)
                {
                    string fullName = studentDt.Rows[0]["FullName"].ToString();
                    string email = studentDt.Rows[0]["Email"].ToString();
                  //  string email = "yogitapatil7219@gmail.com"; 

                    string branch = interviewDt.Rows[0]["Branch"].ToString();
                    string interviewType = interviewDt.Rows[0]["InterviewType"].ToString();
                    string interviewDate = Convert.ToDateTime(interviewDt.Rows[0]["InterviewDate"]).ToString("dd-MM-yyyy");
                    string interviewTime = interviewDt.Rows[0]["InterviewTime"].ToString();
                    string status = interviewDt.Rows[0]["Status"].ToString();

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("placementerp@gmail.com");
                    mail.To.Add(email);

                    mail.Subject = "Interview Schedule Notification";

                    mail.IsBodyHtml = true;

                    mail.Body = $@"
<html>
<head>
</head>
<body style='margin:0;padding:0;background-color:#f4f6f9;font-family:Arial,sans-serif;'>

    <table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 0;background-color:#f4f6f9;'>
        <tr>
            <td align='center'>

                <table width='650' cellpadding='0' cellspacing='0' 
                       style='background:#ffffff;border-radius:18px;overflow:hidden;
                       box-shadow:0 4px 18px rgba(0,0,0,0.08);'>

                    <!-- Header -->
                    <tr>
                        <td style='background:linear-gradient(135deg,#0f172a,#2563eb);
                                   padding:35px;text-align:center;color:white;'>

                            <h1 style='margin:0;font-size:30px;font-weight:bold;'>
                                Interview Opportunity
                            </h1>

                            <p style='margin-top:10px;font-size:15px;opacity:0.9;'>
                                Placement Portal Notification
                            </p>

                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style='padding:40px;'>

                            <p style='font-size:18px;color:#111827;margin-bottom:10px;'>
                                Dear <b>{fullName}</b>,
                            </p>

                            <p style='font-size:16px;color:#4b5563;line-height:1.8;'>
                                Greetings from the Placement Team!
                            </p>

                            <p style='font-size:16px;color:#4b5563;line-height:1.8;'>
                                We are pleased to inform you that an 
                                <b>interview opportunity</b> has been assigned to you.
                                Kindly login to the <b>Placement Portal</b> and apply for the interview.
                            </p>

                            <!-- Interview Details Card -->
                            <table width='100%' cellpadding='12' cellspacing='0'
                                   style='margin-top:25px;border:1px solid #e5e7eb;
                                   border-radius:12px;background:#f9fafb;'>

                                <tr>
                                    <td colspan='2'
                                        style='font-size:18px;font-weight:bold;
                                        color:#1e3a8a;padding-bottom:15px;'>
                                        Interview Details
                                    </td>
                                </tr>

                                <tr>
                                    <td style='font-weight:bold;color:#374151;width:40%;'>
                                        Branch
                                    </td>
                                    <td style='color:#111827;'>
                                        {branch}
                                    </td>
                                </tr>

                                <tr>
                                    <td style='font-weight:bold;color:#374151;'>
                                        Interview Type
                                    </td>
                                    <td style='color:#111827;'>
                                        {interviewType}
                                    </td>
                                </tr>

                                <tr>
                                    <td style='font-weight:bold;color:#374151;'>
                                        Interview Date
                                    </td>
                                    <td style='color:#111827;'>
                                        {interviewDate}
                                    </td>
                                </tr>

                                <tr>
                                    <td style='font-weight:bold;color:#374151;'>
                                        Interview Time
                                    </td>
                                    <td style='color:#111827;'>
                                        {interviewTime}
                                    </td>
                                </tr>

                                <tr>
                                    <td style='font-weight:bold;color:#374151;'>
                                        Status
                                    </td>
                                    <td>
                                        <span style='background:#dcfce7;
                                                     color:#166534;
                                                     padding:6px 14px;
                                                     border-radius:20px;
                                                     font-size:13px;
                                                     font-weight:bold;'>
                                            {status}
                                        </span>
                                    </td>
                                </tr>

                            </table>

                            <!-- Note -->
                            <p style='margin-top:30px;
                                      font-size:15px;
                                      color:#6b7280;
                                      line-height:1.8;'>

                                Please complete your application before the deadline
                                and ensure your availability for the interview process.

                            </p>

                            <!-- Footer Message -->
                            <div style='margin-top:35px;
                                        padding:20px;
                                        background:#eff6ff;
                                        border-left:5px solid #2563eb;
                                        border-radius:10px;'>

                                <p style='margin:0;
                                          color:#1e3a8a;
                                          font-size:15px;
                                          line-height:1.8;'>

                                    We wish you the very best for your interview and future career opportunities.

                                </p>

                            </div>

                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background:#111827;
                                   padding:22px;
                                   text-align:center;
                                   color:#d1d5db;
                                   font-size:14px;'>

                            © 2026 Placement Portal | Placement Team

                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>
";

                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new NetworkCredential("placementerp@gmail.com", "erkkhoqzdiiuigyj");
                    smtp.EnableSsl = true;

                    smtp.Send(mail);
                }

                return Json(new { success = true, message = "Interview Scheduled Successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to Schedule Interview" });
            }
        }
        //[HttpPost]
        //public JsonResult SaveInterview(InterviewScheduleModel model)
        //{
        //    int result = _dal.SaveInterview(model);
        //    string StudentId = model.StudentId.ToString();

        //    if (result > 0)
        //    {
        //        return Json(new { success = true, message = "Interview Scheduled Successfully" });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Failed to Schedule Interview" });
        //    }
        //}

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

                    mail.Subject = "🎉 Congratulations! You Are Selected";

                    mail.Body = $@"

<div style='background:#f4f6fb;
            padding:40px;
            font-family:Segoe UI,Arial,sans-serif;'>

    <div style='max-width:650px;
                margin:auto;
                background:white;
                border-radius:22px;
                overflow:hidden;
                box-shadow:0 10px 30px rgba(0,0,0,0.12);'>

        <!-- HEADER -->

        <div style='background:linear-gradient(135deg,#3b245f,#6d4db3);
                    padding:40px;
                    text-align:center;
                    color:white;'>

            <h1 style='margin:0;
                       font-size:34px;
                       font-weight:700;'>

                🎉 Congratulations!

            </h1>

            <p style='margin-top:12px;
                      font-size:16px;
                      opacity:0.9;'>

                Your hard work has paid off 🚀

            </p>

        </div>

        <!-- BODY -->

        <div style='padding:40px;
                    color:#333;'>

            <h2 style='margin-top:0;
                       color:#3b245f;
                       font-size:26px;'>

                Dear {firstName} {lastName},

            </h2>

            <p style='font-size:16px;
                      line-height:30px;
                      margin-top:20px;'>

                We are delighted to inform you that you have been
                <b style='color:#28a745;'>successfully selected</b>
                in

                <span style='color:#6d4db3;
                             font-weight:700;
                             font-size:18px;'>

                    {companyName}

                </span>

                🎯

            </p>

            <!-- SUCCESS CARD -->

            <div style='background:#f7f3ff;
                        border-left:6px solid #6d4db3;
                        padding:25px;
                        margin-top:30px;
                        border-radius:14px;'>

                <h3 style='margin-top:0;
                           color:#3b245f;'>

                    Selection Details

                </h3>

                <table style='width:100%;
                              font-size:15px;
                              margin-top:15px;'>

                    <tr>
                        <td style='padding:10px 0;'>
                            <b>Company</b>
                        </td>

                        <td>
                            {companyName}
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:10px 0;'>
                            <b>Status</b>
                        </td>

                        <td style='color:#28a745;
                                   font-weight:700;'>

                            SELECTED ✅

                        </td>
                    </tr>

                    <tr>
                        <td style='padding:10px 0;'>
                            <b>Offer Letter</b>
                        </td>

                        <td>
                            Attached with this email 📄
                        </td>
                    </tr>

                </table>

            </div>

            <!-- MESSAGE -->

            <p style='font-size:15px;
                      line-height:28px;
                      margin-top:35px;'>

                This achievement reflects your dedication,
                preparation, and consistent efforts.

                We wish you a successful and bright future ahead 🌟

            </p>

            <!-- BUTTON -->

            <div style='text-align:center;
                        margin-top:40px;'>

                <a href='#'
                   style='background:linear-gradient(135deg,#3b245f,#6d4db3);
                          color:white;
                          text-decoration:none;
                          padding:14px 34px;
                          border-radius:12px;
                          font-weight:600;
                          display:inline-block;
                          box-shadow:0 6px 18px rgba(59,36,95,0.3);'>

                    Best Wishes From Placement Team 🚀

                </a>

            </div>

        </div>

        <!-- FOOTER -->

        <div style='background:#f1f3f8;
                    padding:20px;
                    text-align:center;
                    color:#666;
                    font-size:13px;'>

            © Educatal Placement Portal <br/>
            Empowering Student Careers 💜

        </div>

    </div>

</div>

";

                    mail.IsBodyHtml = true;

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



        // LOAD PAGE
        public ActionResult Event()
        {
            return View();
        }

        // GET ALL EVENTS FOR CALENDAR
        [HttpGet]
        public JsonResult GetEvents()
        {
            var data = _dal.GetAllEvents();

            var events = data.Select(item => new
            {
                id = item.EventId,
                title = item.EventName,
                start = item.EventDate.ToString("yyyy-MM-dd") + "T" +
                        item.EventTime.ToString(@"hh\:mm"),
                color = "#0d6efd"
            }).ToList();

            return Json(events);
        }

        // SAVE EVENT
        [HttpPost]
        public JsonResult SaveEvent(PlacementEvents model)
        {
            try
            {
                if (model.EventDate < DateTime.Today)
                {
                    return Json(new { success = false, message = "Past date not allowed." });
                }

                if (model.EventTime < new TimeSpan(10, 0, 0) ||
                    model.EventTime > new TimeSpan(17, 0, 0))
                {
                    return Json(new { success = false, message = "Time allowed only 10 AM to 5 PM." });
                }

                _dal.SaveEvent(model);

                return Json(new { success = true, message = "Event Saved Successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET EVENT DETAILS BY ID
        [HttpGet]
        public JsonResult GetEventById(int id)
        {
            var item = _dal.GetEventById(id);

            return Json(new
            {
                EventId = item.EventId,
                EventName = item.EventName,
                Branch = item.Branch,
                SpeakerName = item.SpeakerName,
                EventMode = item.EventMode,
                MeetingLink = item.MeetingLink,
                Venue = item.Venue,
                EventDate = item.EventDate.ToString("dd-MM-yyyy"),
                EventTime = item.EventTime.ToString(@"hh\:mm")
            });
        }


        public ActionResult PremiumUsers()
        {
            DataTable dt = _dal.GetPaymentList();

            return View(dt);
        }



        public IActionResult DownloadReceipt(int id)
        {
            try
            {
                DataTable dt = _dal.GetReceiptById(id);

                // ✅ Check Data Exists
                if (dt.Rows.Count == 0)
                {
                    return Content("Receipt Not Found");
                }

                // ✅ Get Relative Path From DB
                string receiptPath = dt.Rows[0]["Receipt"].ToString();

                // Example:
                // /Receipt/file.pdf

                // ✅ Convert To Physical Path
                string fullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    receiptPath.TrimStart('/')
                );

                // ✅ File Exists Check
                if (!System.IO.File.Exists(fullPath))
                {
                    return Content("PDF File Not Found");
                }

                // ✅ Read File
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);

                // ✅ Get File Name
                string fileName = Path.GetFileName(fullPath);

                // ✅ Download PDF
                return File(
                    fileBytes,
                    "application/pdf",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return Content("Error : " + ex.Message);
            }
        }

        // OPEN NOTES PAGE
        //public IActionResult ViewNotes(int id)
        //{
        //    // DEMO DATA
        //    TopicNotes note = new TopicNotes()
        //    {
        //        Id = id,
        //        TopicName = "Process Scheduling",
        //        SubjectName = "Operating System",
        //        PdfPath = "/notes/os.pdf",
        //        Description = "OS Unit 3 Notes"
        //    };

        //    return View(note);
        //}

        //public IActionResult ViewNotes(int id)
        //{
        //    return Content("QR Working Successfully");
        //}

        //// GENERATE QR
        //public IActionResult GenerateQR()
        //{
        //    string url = "http://192.168.0.102:5000/Notes/ViewNotes/1";

        //    QRCodeGenerator qrGenerator = new QRCodeGenerator();

        //    QRCodeData qrCodeData =
        //        qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

        //    PngByteQRCode qrCode =
        //        new PngByteQRCode(qrCodeData);

        //    byte[] qrCodeImage = qrCode.GetGraphic(20);

        //    string folder =
        //        Path.Combine(_env.WebRootPath, "qrcodes");

        //    if (!Directory.Exists(folder))
        //    {
        //        Directory.CreateDirectory(folder);
        //    }

        //    string fileName = "note1.png";

        //    string filePath =
        //        Path.Combine(folder, fileName);

        //    System.IO.File.WriteAllBytes(filePath, qrCodeImage);

        //    ViewBag.QRCode = "/qrcodes/" + fileName;

        //    return View();
        //}



    }
}
