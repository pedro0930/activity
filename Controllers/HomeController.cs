using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginRegistration.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LoginRegistration.Controllers
{
    public class HomeController : Controller
    {
        private LoginRegistrationContext _context;
 
        public HomeController(LoginRegistrationContext context)
    {
        _context = context;
    }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        [Route("Register")]
        
        public IActionResult Register(User user)
        {
            User findEmail = _context.users.SingleOrDefault(users => users.email == user.email);
            if(findEmail != null)
            {
                System.Console.WriteLine("###Email Already Registered###");
                TempData["EmailRegistered"] = "Email Already Registered!";
                return RedirectToAction("Index", TempData["EmailRegistered"]);
            }
            if(ModelState.IsValid){
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.password = Hasher.HashPassword(user, user.password);
                User newUser = new User
                {
                    first_name = user.first_name,
                    last_name = user.last_name,
                    email = user.email,
                    password = user.password,
                    confirm = "",
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                _context.Add(newUser);
                _context.SaveChanges();
                System.Console.WriteLine("Registration Success");
                User userData = _context.users.Last();
                HttpContext.Session.SetInt32("user_id", userData.UserId);
                return RedirectToAction("Dashboard");

            }
            else
            {   
                System.Console.WriteLine("###Validation failed###");
                return View("Index");
            };
        }
        [HttpGet]
        [Route("Dashboard")]   
        public IActionResult Dashboard()
        {
            DateTime now = DateTime.Now;
            int? check_id = HttpContext.Session.GetInt32("user_id");
            if(check_id == null)
            {
                System.Console.WriteLine("Not logged in, return to Index");
                return RedirectToAction("Index");
            }
            int user_id = HttpContext.Session.GetInt32("user_id").GetValueOrDefault();
            User userData = _context.users.SingleOrDefault(users => users.UserId == user_id);
            List<Models.Activity> allActivities = _context.activities.Include(a=>a.guests).Include(user=>user.planner).Where(activity=> activity.date > now).OrderBy(activity=> activity.date).ToList();
            ViewBag.allActivities = allActivities;
            ViewBag.userData = userData;
            return View("Dashboard");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(User LoginInfo)
        {
            User userData = _context.users.SingleOrDefault(user => user.email == LoginInfo.email);
            if (userData == null)
            {
                System.Console.WriteLine("#####Email not found#####");
                TempData["Error"] = "Email not found";
                return RedirectToAction("Index");
            }
            else
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(userData, userData.password, LoginInfo.password))
                {
                    System.Console.WriteLine("Login Success");
                    HttpContext.Session.SetInt32("user_id", userData.UserId);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    System.Console.WriteLine("#####Incorrect Password#####");
                    TempData["Error"] = "Incorrect password!";
                    return RedirectToAction("Index");
                }
            }
        }
    
        [HttpGet]
        [Route("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            int? check_id = HttpContext.Session.GetInt32("user_id");
            if(check_id == null)
            {
                System.Console.WriteLine("Not logged in, return to Index");
                return RedirectToAction("Index");
            }
            return View("New");
        }
        [HttpPost]
        [Route("Add_Activity")]
        public IActionResult Add_Activity(Models.Activity activity)
        {
            if(activity.date < DateTime.Now)
            {
                TempData["error"] = "Event date must be in the future";
                return RedirectToAction("New", TempData["error"]);
            }

            if(ModelState.IsValid)
            {
                Models.Activity newActivity = new Models.Activity
                {
                    name = activity.name,
                    timeType = activity.timeType,
                    time = activity.time,
                    date = activity.date,
                    duration = activity.duration,
                    durationtype = activity.durationtype,
                    description = activity.description,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now,
                    PlannerId = HttpContext.Session.GetInt32("user_id").GetValueOrDefault()
                };

                _context.Add(newActivity);
                _context.SaveChanges();
                Models.Activity lastAct = _context.activities.Last();
                int id = lastAct.ActivityId;
                return RedirectToAction("Activity", new {@id=id});
            }
            else
            {
                return View("New");
            }
        }
        [HttpGet]
        [Route("Activity/{id}")]
        public IActionResult Activity(int id)
        {
            int? check_id = HttpContext.Session.GetInt32("user_id");
            if(check_id == null)
            {
                System.Console.WriteLine("Not logged in, return to Index");
                return RedirectToAction("Index");
            }
            Models.Activity activity = _context.activities.Include(a => a.planner).Include(p => p.guests).ThenInclude(user => user.user).SingleOrDefault(a => a.ActivityId == id);
            User userData = _context.users.SingleOrDefault(users => users.UserId == HttpContext.Session.GetInt32("user_id").GetValueOrDefault());
            ViewBag.activity = activity;
            ViewBag.userData = userData;
            return View("Activity");
        }
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            Models.Activity Target = _context.activities.SingleOrDefault(a => a.ActivityId == id);

            _context.activities.Remove(Target);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        [Route("Join")]
        public IActionResult Join(int id)
        {
            // Models.Activity validation = _context.activities.SingleOrDefault(a => a.ActivityId == id);
            // int time = validation.time;
            // TimeSpan TimeInt = new TimeSpan();
            // TimeSpan PM = TimeSpan.FromHours(12);
            // if(validation.durationtype == "days")
            // {
            //     TimeInt = TimeSpan.FromDays(time);
            // }
            // else if(validation.durationtype == "hours")
            // {
            //     TimeInt = TimeSpan.FromHours(time);
            // }
            // else
            // {
            //     TimeInt = TimeSpan.FromMinutes(time);
            // }
            // if(validation.timeType =="PM")
            // {
            //     TimeInt += PM;
            // }
            // DateTime EventEnd = validation.date + TimeInt;
            // if(EventEnd > )
            // {
            //     TempData["busy"] = "Scheduling Conflict! You are attending other activities at the same timeframe";
            //     return RedirectToAction("Dashboard",  TempData["busy"]);
            // }
            Participant participant = new Participant
            {
                userid = HttpContext.Session.GetInt32("user_id").GetValueOrDefault(),
                activityid = id,
            };
            _context.Add(participant);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        [Route("Unjoin")]
        public IActionResult Unjoin(int id)
        {
            Participant Guest = _context.participants.SingleOrDefault(g => g.ParticipantId == id);
            _context.participants.Remove(Guest);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
