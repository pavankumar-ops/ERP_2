using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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



        public IActionResult AddVacancy(string id)
        {
            
            return View();
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

            TempData["Success"] =
                $"Company Registered Successfully! Company ID: {companyId}";

            return RedirectToAction("CompanyRegistration");
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
    }
}
