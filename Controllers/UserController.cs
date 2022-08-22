using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class UserController : Controller
{ 
    // _context is just a variable name -- It can be called anything such as db, or DATABASE
        private WeddingPlannerContext db;


    // ===========================================================
    // UUID
    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("UUID");
        }
    }


    private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }

    // here we can "inject" our context service into the constructor
    public UserController(WeddingPlannerContext context)
    {
        db = context;
    }


    // ===========================================================
    // INDEX
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View("Index");
    }


    // ===========================================================
    // DASHBOARD
    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        List<Wedding> wedding = db.Weddings.Include(wedding => wedding.Connections).ToList();

        return View("Dashboard", wedding);
    }


    // ===========================================================
    // REGISTER
    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (db.Users.Any(user => user.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Is already in use");
            }
        }


        if (ModelState.IsValid == false)
        {
            return Index();
        }

        // Hash Passwords
        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();
        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);

        db.Users.Add(newUser);
        db.SaveChanges();

        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        HttpContext.Session.SetString("Name", newUser.FullName());

        // return RedirectToAction("All", "Post"); // To the all method in the post controller
        return RedirectToAction("Dashboard");
    }


    // ===========================================================
    // LOGIN
    [HttpPost("/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        // Verify data is valid
        if (ModelState.IsValid == false)
        {
            return Index();
        }

        // Make sure email exist within db
        User? dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);

        if (dbUser == null)
        {
            ModelState.AddModelError("LoginEmail", "not found");
            return Index();
        }

        // Compare hashed passwords
        PasswordHasher<LoginUser> hashBrowns = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashBrowns.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);
        
        if (pwCompareResult == 0)
        {
            ModelState.AddModelError("LoginPassword", "is not correct");
            return Index();
        }

        // Log the user in
        HttpContext.Session.SetInt32("UUID", dbUser.UserId);
        HttpContext.Session.SetString("Name", dbUser.FullName());
        
        return RedirectToAction("Dashboard");
        // return Dashboard();
    }


    // ===========================================================
    // LOGOUT
    [HttpPost("/clear/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

} 