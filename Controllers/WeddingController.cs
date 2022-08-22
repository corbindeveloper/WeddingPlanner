using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
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
    public WeddingController(WeddingPlannerContext context)
    {
        db = context;
    }


    // ===========================================================
    // RENDER NEW WEDDING FORM
    [HttpGet("/new/wedding")]
    public IActionResult NewWedding()
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        return View("NewWedding");
    }


    // ===========================================================
    // SUBMIT NEW WEDDING
    [HttpPost("/submit/wedding")]
    public IActionResult submitWedding(Wedding newWedding)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        if (ModelState.IsValid == false) {
            return View("NewWedding");
        }

        // if (newWedding.UserId == null) {
        //     RedirectToAction("/");
        // }

        newWedding.UserId = (int?)uid;
        db.Weddings.Add(newWedding);
        db.SaveChanges();

        return RedirectToAction("Dashboard", "User");
    }

    // ===========================================================
    // RSVP
    [HttpPost("Wedding/Rsvp/{weddingId}")]
    public IActionResult Rsvp(int weddingId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        Connection connection = new Connection();
        connection.WeddingId = weddingId;

        if (uid == null)
        {
            return View("Index");
        }
        
        connection.UserId = (int)uid;
        
        
        db.Connections.Add(connection);
        db.SaveChanges();

        // return View("Dashboard");
        return RedirectToAction("Dashboard", "User");
    }

    // ===========================================================
    // UN -- RSVP
    [HttpPost("wedding/unrsvp/{weddingId}")]
    public IActionResult UnRsvp(int weddingId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        Connection? connection = db.Connections
            .FirstOrDefault(c => c.UserId == (int)uid && c.WeddingId == weddingId);

        if (connection != null)
        {
            db.Connections.Remove(connection);
            db.SaveChanges();
        }

        // return View("Dashboard");
        return RedirectToAction("Dashboard", "User");
    }

    // ===========================================================
    // DELETE WEDDING
    [HttpPost("wedding/deletewedding/{weddingId}")]
    public IActionResult DeleteWedding(int weddingId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        Wedding? wedding = db.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);

        if (wedding != null)
        {
            db.Weddings.Remove(wedding);
            db.SaveChanges();
        }    
        return RedirectToAction("Dashboard", "User");
    }

    // ===========================================================
    // VIEW WEDDING
    [HttpGet("/view/{weddingId}")]
    public IActionResult ViewWedding(int weddingId)
    {
        if (!loggedIn)
        {
            return RedirectToAction("Index");
        }

        Wedding? wedding = db.Weddings.FirstOrDefault(wedding => wedding.WeddingId == weddingId);

        List<Connection>? guestList = db.Connections
            .Include(connections => connections.User)
            .Where(connection => connection.WeddingId == weddingId)
            .ToList();

        ViewBag.GuestList = guestList;

        return View("ViewWedding", wedding);
    }






}