using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using E_voting.Models.Model;
using System.Text.Json;

namespace E_voting.Controllers
{
    public class VoterController : Controller
    {
        private string VotersFilePath;
        private List<Voter> voters;

        public VoterController()
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Initialize paths here
            string appPath = HttpContext.Server.MapPath("~");
            VotersFilePath = Path.Combine(appPath, "JSONFiles", "voters.json");

            // Load data
            voters = LoadFromJson<List<Voter>>(VotersFilePath) ?? new List<Voter>();
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

        // GET: Voter
        public ActionResult Index()
        {
            return View(voters);
        }

        // GET: Voter/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voter voter = voters.Find(v => v.VoterId == id);
            if (voter == null)
            {
                return HttpNotFound();
            }
            return View(voter);
        }

        // GET: Voter/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Voter/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string password, [Bind(Include = "VoterId,Name,TC,MobileNo,Email,Password,City")] Voter voter)
        {
            if (ModelState.IsValid)
            {
                voter.Password = Crypto.Hash(password, "MD5");
                voters.Add(voter);
                SaveToJson(voters, VotersFilePath);
                return RedirectToAction("Index");
            }

            return View(voter);
        }

        // GET: Voter/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voter voter = voters.Find(v => v.VoterId == id);
            if (voter == null)
            {
                return HttpNotFound();
            }
            return View(voter);
        }


        // POST: Voter/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string password, [Bind(Include = "VoterId,Name,TC,MobileNo,Email,Password,City")] Voter voter)
        {
            if (ModelState.IsValid)
            {
                voter.Password = Crypto.Hash(password, "MD5");
                var existingVoter = voters.Find(x => x.VoterId == voter.VoterId);
                if (existingVoter != null)
                {
                    existingVoter.Name = voter.Name;
                    existingVoter.TC = voter.TC;
                    existingVoter.MobileNo = voter.MobileNo;
                    existingVoter.Email = voter.Email;
                    existingVoter.Password = voter.Password;
                    existingVoter.City = voter.City;

                    SaveToJson(voters, VotersFilePath);
                    return RedirectToAction("Index");
                }
            }
            return View(voter);
        }

        // GET: Voter/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voter voter = voters.Find(v => v.VoterId == id);
            if (voter == null)
            {
                return HttpNotFound();
            }
            return View(voter);
        }

        // POST: Voter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Voter voter = voters.Find(v => v.VoterId == id);
            if (voter != null)
            {
                voters.Remove(voter);
                SaveToJson(voters, VotersFilePath);
            }
            return RedirectToAction("Index");
        }

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
