using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Login.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Login.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        
        public HomeController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }
        
        [HttpPost("")]
        public IActionResult Register(User FromForm)
        {
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.Password = Hasher.HashPassword(FromForm, FromForm.Password);
                _context.Add(FromForm);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View("Log");
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser FromForm)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == FromForm.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return RedirectToAction("Login");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(FromForm, userInDb.Password, FromForm.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    return RedirectToAction("Login");
                }
                return RedirectToAction("Success");
            }
            return RedirectToAction("Login");
        }
        
        [HttpGet("success")]
        public IActionResult Success()
        {
            return View("Success");
        }
    }


}