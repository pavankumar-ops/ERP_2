using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Razorpay.Api;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
            bool studisPremium = _dal.IsUserPremium(studentId);

            if (studisPremium == true)
            {
                ViewBag.IsPremium = true;
            }
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
        public ActionResult StudEvent()
        {
            return View();
        }

        string key = "rzp_test_Swok65AlTsoaVf";
        string secret = "tLccKFZhnv0oJmvpX5pHX3Ga";



        [HttpPost]
        public JsonResult CreateOrder()
        {
            try
            {
                RazorpayClient client = new RazorpayClient(key, secret);

                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", 29900);
                options.Add("currency", "INR");
                options.Add("receipt", "receipt_" + DateTime.Now.Ticks);

                Order order = client.Order.Create(options);

                string orderId = order["id"].ToString(); // 👈 debug here

                return Json(new
                {
                    orderId = orderId,
                    key = key
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        
        
        [HttpPost]
        public ActionResult VerifyPayment(string razorpay_payment_id,
                                          string razorpay_order_id,
                                          string razorpay_signature)
        {
            try
            {
                Dictionary<string, string> attributes = new Dictionary<string, string>();

                attributes.Add("razorpay_payment_id", razorpay_payment_id);
                attributes.Add("razorpay_order_id", razorpay_order_id);
                attributes.Add("razorpay_signature", razorpay_signature);

                Utils.verifyPaymentSignature(attributes);

                int studentId = Convert.ToInt32(HttpContext.Session.GetString("StudentId"));

                _dal.SavePayment(studentId, razorpay_payment_id, 299);
                SendPaymentReceipt(razorpay_payment_id);
                _dal.MakeUserPremium(studentId);
                string receiptPath = HttpContext.Session.GetString("ReceiptPath");
                _dal.UpdateReceiptPath(studentId, razorpay_payment_id, receiptPath);


                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        public byte[] GenerateReceiptPDF( string razorpay_payment_id)
        {
            string studentId = HttpContext.Session.GetString("StudentId");

            DataTable dt = _dal.GetLatestPaymentStudentDetails(studentId);

            if (dt.Rows.Count == 0)
                return null;

            string studentName = dt.Rows[0]["StudentName"].ToString();
            string studentEmail = dt.Rows[0]["Email"].ToString();
            string paymentId = dt.Rows[0]["PaymentId"].ToString();
            string amount = dt.Rows[0]["Amount"].ToString();
            string paymentDate = Convert.ToDateTime(dt.Rows[0]["PaymentDate"])
                                    .ToString("dd MMM yyyy hh:mm tt");

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);

                PdfWriter.GetInstance(doc, ms);

                doc.Open();

                // ================= HEADER =================

                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24,BaseColor.White);

                PdfPTable headerTable = new PdfPTable(1);
                headerTable.WidthPercentage = 100;

                PdfPCell headerCell = new PdfPCell(
                    new Phrase("EDUCATAL PLACEMENT PORTAL", headerFont)
                );

                headerCell.BackgroundColor = new BaseColor(63, 81, 181);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Padding = 15;
                headerCell.Border = 0;

                headerTable.AddCell(headerCell);

                doc.Add(headerTable);

                // ================= TITLE =================

                doc.Add(new Paragraph(" "));

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);

                Paragraph title = new Paragraph("PAYMENT RECEIPT", titleFont);

                title.Alignment = Element.ALIGN_CENTER;

                doc.Add(title);

                doc.Add(new Paragraph(" "));

                // ================= RECEIPT TABLE =================

                PdfPTable table = new PdfPTable(2);

                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 35, 65 });

                Font labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.White);

                Font valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

                BaseColor themeColor = new BaseColor(63, 81, 181);

                // Method for adding rows
                void AddRow(string label, string value)
                {
                    PdfPCell cell1 = new PdfPCell(new Phrase(label, labelFont));
                    cell1.BackgroundColor = themeColor;
                    cell1.Padding = 10;

                    PdfPCell cell2 = new PdfPCell(new Phrase(value, valueFont));
                    cell2.Padding = 10;

                    table.AddCell(cell1);
                    table.AddCell(cell2);
                }

                AddRow("Student Name", studentName);
                AddRow("Student Email", studentEmail);
                AddRow("Payment ID", paymentId);
                AddRow("Amount", "₹ " + amount);
                AddRow("Plan", "Premium Lifetime");
                AddRow("Payment Status", "SUCCESS");
                AddRow("Payment Date", paymentDate);

                doc.Add(table);

                // ================= FOOTER =================

                doc.Add(new Paragraph(" "));

                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 11);

                Paragraph footer = new Paragraph(
                    "Thank you for purchasing premium access from Educatal Placement Portal 🎉",
                    footerFont
                );

                footer.Alignment = Element.ALIGN_CENTER;

                doc.Add(footer);

                doc.Close();

                // ================= SAVE PDF =================

                string folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "Receipt"
                );

                // Create folder automatically
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // File name
                string safeName = studentName.Replace(" ", "_");

                string fileName = safeName + "_" + paymentId + ".pdf";

                string fullPath = Path.Combine(folderPath, fileName);
                string dbPath = "/Receipt/" + fileName;
                HttpContext.Session.SetString("ReceiptPath", dbPath);
                //string studentId = HttpContext.Session.GetString("StudentId");

                System.IO.File.WriteAllBytes(fullPath, ms.ToArray());

                return ms.ToArray();
            }
        }


        public void SendPaymentReceipt( string razorpay_payment_id)
        {
            string studentId = HttpContext.Session.GetString("StudentId");
            DataTable dt = _dal.GetLatestPaymentStudentDetails(studentId);
            string studentName = dt.Rows[0]["StudentName"].ToString();
            string toEmail = dt.Rows[0]["Email"].ToString();
            string paymentId = dt.Rows[0]["PaymentId"].ToString();
            string amount = dt.Rows[0]["Amount"].ToString();
            string paymentDate = Convert.ToDateTime(dt.Rows[0]["PaymentDate"])
                                    .ToString("dd MMM yyyy hh:mm tt");
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("placementerp@gmail.com");

            mail.To.Add(toEmail);

            mail.Subject = "Premium Membership Activated";

            mail.Body = $@"

<div style='font-family:Arial,Helvetica,sans-serif;
            background:#f4f6fb;
            padding:30px;'>

    <div style='max-width:600px;
                margin:auto;
                background:white;
                border-radius:18px;
                overflow:hidden;
                box-shadow:0 8px 25px rgba(0,0,0,0.1);'>

        <!-- Header -->
        <div style='background:linear-gradient(135deg,#3f51b5,#5c6bc0);
                    color:white;
                    padding:30px;
                    text-align:center;'>

            <h1 style='margin:0;
                       font-size:28px;'>
                EDUCATAL PLACEMENT PORTAL
            </h1>

            <p style='margin-top:10px;
                      font-size:15px;
                      opacity:0.9;'>
                Premium Membership Activated
            </p>
        </div>

        <!-- Body -->
        <div style='padding:35px; color:#333;'>

            <h2 style='margin-top:0;
                       color:#3f51b5;'>
                Hi {studentName} 👋
            </h2>

            <p style='font-size:16px;
                      line-height:28px;'>

                Your payment has been received successfully and your
                <b>Premium Membership</b> is now activated 🎉
            </p>

            <!-- Success Box -->
            <div style='background:#f5f7ff;
                        border-left:5px solid #3f51b5;
                        padding:18px;
                        margin-top:25px;
                        border-radius:10px;'>

                <h3 style='margin-top:0;
                           color:#3f51b5;'>
                    Payment Details
                </h3>

                <table style='width:100%;
                              font-size:15px;'>

                    <tr>
                        <td style='padding:8px 0;'>
                            <b>Status</b>
                        </td>

                        <td style='color:green;'>
                            SUCCESS ✅
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:8px 0;'>
                            <b>Plan</b>
                        </td>

                        <td>
                            Premium Lifetime
                        </td>
                    </tr>

                    <tr>
                        <td style='padding:8px 0;'>
                            <b>Amount</b>
                        </td>

                        <td>
                            ₹299
                        </td>
                    </tr>

                </table>
            </div>

            <p style='margin-top:30px;
                      font-size:15px;
                      line-height:26px;'>

                Your payment receipt PDF has been attached with this email.
            </p>

            <div style='text-align:center;
                        margin-top:35px;'>

                <a href='#'
                   style='background:#3f51b5;
                          color:white;
                          text-decoration:none;
                          padding:14px 30px;
                          border-radius:10px;
                          display:inline-block;
                          font-weight:bold;'>

                    Explore Premium Tests 🚀
                </a>
            </div>

        </div>

        <!-- Footer -->
        <div style='background:#f1f3f8;
                    text-align:center;
                    padding:20px;
                    font-size:13px;
                    color:#666;'>

            Thank you for choosing
            <b>Educatal Placement Portal</b> ❤️
        </div>

    </div>

</div>

";

            mail.IsBodyHtml = true;




            // ✅ Generate PDF
            byte[] pdfBytes = GenerateReceiptPDF(razorpay_payment_id);

            Attachment attachment = new Attachment(
                new MemoryStream(pdfBytes),
                "PaymentReceipt.pdf",
                "application/pdf"
            );

            mail.Attachments.Add(attachment);

            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;

            smtp.Credentials = new NetworkCredential(
                "placementerp@gmail.com",
                "erkkhoqzdiiuigyj"
            );

            smtp.EnableSsl = true;

            smtp.Send(mail);
        }

        //public ActionResult SendPaymentReceipt()
        //{


        //        string hrEmail = "tanmaysshinde09@gmail.com";
        //        // Sample Student Name
        //        string studentName = "Tanmay Shinde";

        //        // ✅ Dynamic PDF Path
        //        string fileName = "tanmay_rrtttrttt_shinde_pay_SoM7mM0Hvb15NC.pdf";

        //        string fullResumePath = Path.Combine(
        //            Directory.GetCurrentDirectory(),
        //            "wwwroot",
        //            "Receipt",
        //            fileName
        //        );

        //        // ================= MAIL =================

        //        MailMessage mail = new MailMessage();

        //        mail.From = new MailAddress("placementerp@gmail.com");

        //        mail.To.Add(hrEmail);

        //        mail.Subject = "Interview Scheduled - Student Receipt";

        //        mail.Body = $@"
        //    Dear HR,<br/><br/>

        //    Interview has been scheduled for the following student:<br/><br/>

        //    <b>Student Name :</b> {studentName}<br/><br/>

        //    Please find the attached receipt PDF.<br/><br/>

        //    Regards,<br/>
        //    Placement Cell
        //";

        //        mail.IsBodyHtml = true;

        //        // ================= ATTACHMENT =================

        //        if (System.IO.File.Exists(fullResumePath))
        //        {
        //            mail.Attachments.Add(
        //                new Attachment(fullResumePath)
        //            );
        //        }

        //        // ================= SMTP =================

        //        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        //        smtp.UseDefaultCredentials = false;

        //        smtp.Credentials = new NetworkCredential(
        //            "placementerp@gmail.com",
        //            "erkkhoqzdiiuigyj"
        //        );

        //        smtp.EnableSsl = true;

        //        smtp.Send(mail);

        //        return Content("Mail Sent Successfully");
        //    }



    }
 }
