using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Models;

using Microsoft.AspNetCore.Http; //added
using Microsoft.EntityFrameworkCore; //added
using Microsoft.AspNetCore.Identity; //addedfor hash
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ScrumBoard.Controllers
{
  public class HomeController : Controller
  {
    private ReviewContext _context;
    public HomeController(ReviewContext context)
    {
      _context = context;
    }
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpPost("register")] //must be same as asp-action
    public IActionResult register(Userlogin newuser)
    {

      sqlUser validuser = _context.usertable.SingleOrDefault(a => a.email == newuser.email); //.email is existing usertable email list
      if (validuser != null)
      { // != email is taken
        ViewBag.email = "This email is taken. Try another email.";
        return View("Index");
      }
      if (ModelState.IsValid != true && validuser != null)
      { //not valid (error msg will show) & taken email
        ViewBag.email = "This email is taken. Try another email.";
        return View("Index");
      }
      if (ModelState.IsValid)
      {
        // transfer the form user to sqluser model, which graps the table from the usertable. copy.name is the usertable's name.
        sqlUser copy = new sqlUser();
        copy.name = newuser.name;
        copy.lastname = newuser.lastname;
        copy.email = newuser.email;
        copy.pw = newuser.pw;
        copy.level = newuser.level;
        // hash the password of the copied user
        PasswordHasher<sqlUser> Hasher = new PasswordHasher<sqlUser>();
        copy.pw = Hasher.HashPassword(copy, copy.pw);

        HttpContext.Session.SetString("username", copy.name);
        // let's save them to sql
        _context.usertable.Add(copy);
        _context.SaveChanges();
        // make an id of a made user from usertable now
        sqlUser madeuser = _context.usertable.SingleOrDefault(a => a.email == copy.email);
        HttpContext.Session.SetInt32("userid", madeuser.id);

        return RedirectToAction("Board");
      }
      return View("Index");
    }

    [HttpPost("login")]
    public IActionResult Login(string email, string pw)
    {
      sqlUser existinguser = _context.usertable.Where(a => a.email == email).SingleOrDefault();
      if (existinguser != null && pw != null)
      { //if that email exists
        PasswordHasher<sqlUser> Hasher = new PasswordHasher<sqlUser>(); //passwordhasher for sqluser
        if (0 != Hasher.VerifyHashedPassword(existinguser, existinguser.pw, pw))
        { //if not 0 meaning password matched
          HttpContext.Session.SetString("username", existinguser.name);
          HttpContext.Session.SetInt32("userid", existinguser.id);
          return RedirectToAction("Board");
        }
        else
        {
          ViewBag.email2 = "Email is found but password does not match.";
          return View("Index");
        }
      }
      else
      {
        ViewBag.email2 = "Email is NOT found AND password does not match.";
        return View("Index");
      }
    }

        //    [HttpGet("result")]
        // public IActionResult Result() {
        //     List<sqlUser> Allusers = _context.usertable.OrderBy(o=>o.name).ToList(); //what is the reviews table name here for, and _context?       
        //     ViewBag.username = HttpContext.Session.GetString("username");
        //     return View("Result", Allusers);
        // }


    [HttpGet("logout")]
    public IActionResult logout()
    {
      HttpContext.Session.Clear();
      return RedirectToAction("Index");
    }

    //// ********************************************************************************** ////
    [HttpPost("/sorting")]
    public IActionResult Sorting(string sort){
      if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");

      HttpContext.Session.SetString("sortQ", sort);
      return RedirectToAction("Board");
    }
  
  
    [HttpGet("/board")]
    public IActionResult Board(){
      if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");

  // Creator's name sort
      if (HttpContext.Session.GetString("sortQ") == "UserId") {
        List<Scrum> allscrums = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(j=>j.user).OrderBy(x=>x.user.name).ToList();
        return View(allscrums); 

  // end date sort 
      } else if (HttpContext.Session.GetString("sortQ") == "enddate") {
        List<Scrum> allscrums = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(j=>j.user).OrderBy(x=>x.enddate).ToList();
        return View(allscrums); 

  // status sort 
      } else if (HttpContext.Session.GetString("sortQ") == "status") {
        List<Scrum> allscrums = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(j=>j.user).OrderBy(x=>x.status).ToList();
        return View(allscrums); 

  //default sort : newest task at top
      } else {
        List<Scrum> allscrums = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(j=>j.user).OrderByDescending(x=>x.id).ToList();
        return View(allscrums); 
      }
    }


    [HttpGet("/add")]
    public IActionResult Add(){
        if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
        return View(); 
    } 

    [HttpPost("process")]
    public IActionResult Process (Scrum newtask){ //asp-action name
        if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
        if (ModelState.IsValid) {
            if(newtask.enddate < DateTime.Now.Date || newtask.enddate < newtask.startdate || newtask.enddate == newtask.startdate ){
                ModelState.AddModelError("enddate", "Ending Date must be in the future, or after the start date.");
                return View("Add");
            }

            newtask.UserId = (int)HttpContext.Session.GetInt32("userid"); 
            _context.scrumtable.Add(newtask);
            _context.SaveChanges();
            // return RedirectToAction("Board");

            return RedirectToAction("About", new { taskid = newtask.id });
        }
        return View("Add"); 
    } 

    [HttpGet("/edit/{taskid}")]
    public IActionResult Edit (int taskid){
      if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
      
      Scrum onetask = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(x=>x.user).SingleOrDefault(x=>x.id == taskid);
      ViewBag.about = onetask;
      ViewBag.errors = ViewBag.errors;

      return View(); 
    }


    [HttpPost("/processEdit/{taskid}")]
    public IActionResult ProcessEdit (Scrum edittask, int taskid){ //asp-action name
      if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");

      // Scrum existingTask = _context.scrumtable.SingleOrDefault(x=>x.id == taskid);
      Scrum existingTask = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(x=>x.user).SingleOrDefault(x=>x.id == taskid);

      
      if (ModelState.IsValid && existingTask != null) {
        //add another if to check if this was the creator id!
        if(edittask.enddate < DateTime.Now.Date || edittask.enddate < edittask.startdate || edittask.enddate == edittask.startdate ){
          ModelState.AddModelError("enddate", "Ending Date must be in the future, or after the start date."); 
          //to pass the modelstate error to RedToAct("Edit") by viewbag
          foreach (var modelState in ModelState.Values) {
            foreach (ModelError error in modelState.Errors) {
              var message = (error.Exception ==null)? error.ErrorMessage : error.Exception.Message;   
          // System.Console.WriteLine(message);
          ViewBag.errors = message;
            } 
          }
          return RedirectToAction("Edit");
        }

        existingTask.title = edittask.title; 
        existingTask.dept = edittask.dept; 
        existingTask.startdate = edittask.startdate; 
        existingTask.enddate = edittask.enddate; 
        existingTask.status = edittask.status; 
        existingTask.dx = edittask.dx; 
        _context.SaveChanges();
        // return RedirectToAction("Board");

        return RedirectToAction("About", new { taskid = existingTask.id });
      }
      return RedirectToAction("Edit"); 
    } 


    [HttpGet("/about/{taskid}")]
    public IActionResult About(int taskid){
      if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");

      Scrum onetask = _context.scrumtable.Include(x=>x.user).Include(x=>x.parts).ThenInclude(x=>x.user).SingleOrDefault(x=>x.id == taskid);
      ViewBag.makername = onetask.user.name + " " + onetask.user.lastname;

      return View("About", onetask);
    }


    [HttpGet ("/delete/{taskid}")]
    public IActionResult Delete(int taskid) {
        if (HttpContext.Session.GetInt32("userid") == null) return RedirectToAction("Index");
        //query the creator's id in the joiner table
        // if ()

        Scrum onetask = _context.scrumtable.SingleOrDefault(x=>x.id == taskid);
        List<Participant> allparts = _context.partstable.Where(x=>x.ScrumId == taskid).ToList();
        foreach (var row in allparts){    
            _context.partstable.Remove(row);
        }
        _context.Remove(onetask);
        _context.SaveChanges();
        return RedirectToAction("Board");
    }


    [HttpGet("/join/{taskid}")]
    public IActionResult Join(int taskid) {
        Participant partrow = new Participant();
        partrow.UserId = (int)HttpContext.Session.GetInt32("userid");
        partrow.ScrumId = taskid;

        _context.partstable.Add(partrow);
        _context.SaveChanges();
        return RedirectToAction("Board");
    }

    [HttpGet("/unjoin/{taskid}")]
    public IActionResult Un_Join(int taskid) {
        Participant partrow = _context.partstable.SingleOrDefault(x=>x.ScrumId == taskid && x.UserId == (int)HttpContext.Session.GetInt32("userid"));

        _context.partstable.Remove(partrow);
        _context.SaveChanges();
        return RedirectToAction("Board");
    }

  }
}

// _context.scrumtable.Where(s=>s.title.Contains("Yourie")).SingleOrDefault();