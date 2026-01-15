using ERP_Placement.DAL;
using ERP_Placement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;

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

        public ActionResult TraineerLogin()
        {
            return View();
        }

        public ActionResult Student_LoginPage()
        {
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
                return RedirectToAction("Login");
            }

            // Login success
            HttpContext.Session.SetString("StudentId", dt.Rows[0]["StudentId"].ToString());
            HttpContext.Session.SetString("StudentName", dt.Rows[0]["FirstName"].ToString());

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

        // POST: Login/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Login/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Login/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Login/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Login/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
