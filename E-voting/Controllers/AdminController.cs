using E_voting.Models;
using E_voting.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.Json;
using System.Web.Hosting;
using System.Web.Helpers;
using System.Web.Mvc;
using System.IO;
using System.Diagnostics;

namespace E_voting.Controllers
{
    public class AdminController : Controller
    {
        private List<Voter> voters;
        private List<Admin> admins;
        private List<Candidate> candidates;
        private List<Result> results;
        private List<Position> positions;

        //Paths for the voters and Admins Jsons that store the info of profiles created
        private  string VoterFilePath;
        private string AdminsFilePath;
        private string CandidatesFilePath;
        private string ResultsFilePath;
        private string PositionsFilePath;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Initialize paths here
            string appPath = HttpContext.Server.MapPath("~");
            VoterFilePath = Path.Combine(appPath, "JSONFiles", "voters.json");
            AdminsFilePath = Path.Combine(appPath, "JSONFiles", "admins.json");
            CandidatesFilePath = Path.Combine(appPath, "JSONFiles", "candidates.json");
            ResultsFilePath = Path.Combine(appPath, "JSONFiles", "results.json");
            PositionsFilePath = Path.Combine(appPath, "JSONFiles", "positions.json");

            // Load data
            voters = LoadFromJson<List<Voter>>(VoterFilePath) ?? new List<Voter>();
            admins = LoadFromJson<List<Admin>>(AdminsFilePath) ?? new List<Admin>();
            candidates = LoadFromJson<List<Candidate>>(CandidatesFilePath) ?? new List<Candidate>();
            results = LoadFromJson<List<Result>>(ResultsFilePath) ?? new List<Result>();
            positions = LoadFromJson<List<Position>>(PositionsFilePath) ?? new List<Position>();


        }

        private T LoadFromJson<T>(string filePath)
        {
            try
            {
                var virtualPath = $"~/{filePath.Replace(HttpContext.Server.MapPath("~"), string.Empty).TrimStart('\\').Replace('\\', '/')}";
                var physicalPath = HttpContext.Server.MapPath(virtualPath);
                var json = System.IO.File.ReadAllText(physicalPath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception reading file {filePath}: {ex.Message}");
                return default;
            }
        }

        private void SaveToJson<T>(T data, string filePath)
        {
            try
            {
                var virtualPath = $"~/{filePath.Replace(HttpContext.Server.MapPath("~"), string.Empty).TrimStart('\\').Replace('\\', '/')}";
                var physicalPath = HttpContext.Server.MapPath(virtualPath);
                var json = JsonSerializer.Serialize(data);
                System.IO.File.WriteAllText(physicalPath, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception writing file {filePath}: {ex.Message}");
            }
        }


        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.VoterCount = voters.Count();
            ViewBag.ResultCount = results.Count();
            ViewBag.PositionCount = positions.Count();
            ViewBag.CandidateCount = candidates.Count();
            var sorgu = voters.ToList();
            return View(sorgu);
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin admin)
        {
            try
            {
                var login = admins?.Where(x => x.Email == admin.Email).SingleOrDefault();
                if ((login != null && login.Email == admin.Email) && (login.Password == /*Crypto.Hash(admin.Password, "MD5")*/ admin.Password))
                {
                    Session["adminid"] = login.AdminId;
                    Session["email"] = login.Email;
                    return RedirectToAction("Index", "Admin");
                }
                ViewBag.Uyari = "Wrong password or name";
                return View(admin);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
        public ActionResult Logout()
        {
            Session["adminid"] = null;
            Session["email"] = null;
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }
        public ActionResult Admins()
        {
            return View(admins.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Admin admin,string password,string email)
        {
            if(ModelState.IsValid)
            {
                admin.Password = Crypto.Hash(password,"MD5");
                admins.Add(admin);
                SaveToJson(admins, AdminsFilePath);
                return RedirectToAction("Admins");
            }
            return View(admin);
        }
        public ActionResult Edit(int id)
        {
            var a = admins.Where(x => x.AdminId == id).SingleOrDefault();
            return View(a);
        }
        [HttpPost]
        public ActionResult Edit(int id,Admin admin,string password, string email)
        {            
            if(ModelState.IsValid)
            {
                var a = admins.Where(x => x.AdminId == id).SingleOrDefault();
                a.Password = Crypto.Hash(password, "MD5");
                a.Email = admin.Email;
                SaveToJson(admin, AdminsFilePath);
                return RedirectToAction("Admins");
            }
            return View(admin);
        }
        public ActionResult Delete(int id)
        {
            var a = admins.Where(x => x.AdminId == id).SingleOrDefault();
            if(a!=null)
            {
                admins.Remove(a);
                SaveToJson(admins, AdminsFilePath);
                return RedirectToAction("Admins");
            }
            return View();
        }
    }
}