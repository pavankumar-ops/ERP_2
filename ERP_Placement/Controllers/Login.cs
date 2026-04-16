using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Net;
using System.Net.Mail;


namespace ERP_Placement.Controllers
{
    public class Login : Controller
    {
        // GET: Login

        private readonly StudentDAL _dal;
       

        public Login(IConfiguration config)
        {
            _dal = new StudentDAL(config);
            

        }

        [HttpGet]
        public ActionResult Student_ForgotPassword()
        {
            return View();
        }

        

        public ActionResult TraineerLogin()
        {
            return View();
        }

        public ActionResult Student_LoginPage()
        {
            return View();
        }
        [HttpPost]   
        public ActionResult Traineer_Login(login_Properties model)
        {
            //string Username = "placementerp@gmail.com";
            //string Username = "placementerp@gmail.com";
            string Username = "1";
           



            string Password = "1";
           // string Password = "Placement@123";

           

             
            if (model.UserId == Username && model.Password == Password)
            {
                // Login success
                HttpContext.Session.SetString("TraineerId", "1");
                HttpContext.Session.SetString("TraineerName", "Placement Cordinator");
                return RedirectToAction("TraineerDB", "Trainer");
            }
            else
            {
                TempData["error"] = "Invalid username or password";
                return RedirectToAction("TraineerLogin");
            }

            return View();
        }


        [HttpPost]
        public IActionResult Stud_Login(login_Properties model)
        {
            string Username = model.UserId;
            string Password = model.Password;
            DataTable dt = _dal.StudentLogin(Username, Password);

            if (dt.Rows.Count == 0)
            {
                TempData["error"] = "Invalid username or password";
                return RedirectToAction("Student_LoginPage");
            }

            

            // Login success
            HttpContext.Session.SetString("StudentId", dt.Rows[0]["StudentId"].ToString());
            HttpContext.Session.SetString("StudentName", dt.Rows[0]["FirstName"].ToString());
            string studentId = dt.Rows[0]["StudentId"].ToString();

            
            return RedirectToAction("StudentDB", "Student");
        }
        // GET: Login/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        public JsonResult SendOTP(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return Json(new { success = false, message = "Email is required" });
                }

                // 1️⃣ Check Email Exists
                DataTable dt = _dal.GetUserByEmail(email);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return Json(new { success = false, message = "Email not registered or Not Approved Yet " });
                }

                DataRow row = dt.Rows[0];
                string name = row["FirstName"].ToString();

                // 2️⃣ Generate OTP
                Random rnd = new Random();
                string otp = rnd.Next(100000, 999999).ToString();

                // 3️⃣ Save OTP in DB
                _dal.SaveOTP(email, otp, DateTime.Now.AddMinutes(5));

                // 4️⃣ Send Email
                SendOTPEmail(email, name, otp);

                return Json(new { success = true });
                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void SendOTPEmail(string toEmail, string name, string otp)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("placementerp@gmail.com", "Placement Team");
                mail.To.Add(toEmail);
                mail.Subject = "Password Reset OTP";

                mail.Body = $@"
Hello {name},

Your OTP for password reset is:

OTP: {otp}

This OTP is valid for 5 minutes.

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
                TempData["error"] = "Error sending OTP: " + ex.Message;
            }
        }

        [HttpPost]
        public JsonResult VerifyOTP(string email, string otp)
        {
            bool isValid = _dal.VerifyOTP(email, otp);

            if (isValid)
            {
              //  _dal.MarkOTPUsed(email, otp); // optional but recommended
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Invalid or expired OTP" });
        }


        [HttpPost]
        public JsonResult ResetPassword(string email, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                return Json(new { success = false, message = "Invalid data" });
            }

            bool isUpdated = _dal.UpdateStudentPassword(email, newPassword);

            if (isUpdated)
            {
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Password update failed" });
        }


        public IActionResult StudLogout()
        {
            HttpContext.Session.Clear(); // remove all session
            return RedirectToAction("Index", "Home");
        }


        //public void MyUpcommingInterviews()
        //{
        //    string studentId = HttpContext.Session.GetString("StudentId");
            

        //    return (interviews); // Pass the list to view
        //}
    }
}
