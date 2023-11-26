using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using E_voting.Models.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace E_voting.Controllers
{
    public class CandidateController : Controller
    {
        private string FilePath;
        List<Candidate> candidates;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Initialize paths here
            string appPath = HttpContext.Server.MapPath("~");
            FilePath = Path.Combine(appPath, "JSONFiles", "candidates.json");

            // Load data
            candidates = LoadFromJson<List<Candidate>>(FilePath) ?? new List<Candidate>();
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

        // GET: Candidate
        public ActionResult Index()
        {
            candidates = LoadFromJson<List<Candidate>>(FilePath);
            return View(candidates);
        }

        // GET: Candidate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = candidates.Find(c => c.CandidateId == id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // GET: Candidate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Candidate/Create
        // Aşırı gönderim saldırılarından korunmak için bağlamak istediğiniz belirli özellikleri etkinleştirin. 
        // Daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "CandidateId,Name,TC,City,MobileNo,Email,PhotoPath")] Candidate candidate, HttpPostedFileBase PhotoPath)
        {
            if (ModelState.IsValid)
            {
                if (PhotoPath != null && PhotoPath.ContentLength > 0 && IsImage(PhotoPath.ContentType) && PhotoPath.InputStream.CanRead)
                {
                    // Generate a unique filename
                    string logoname = Guid.NewGuid().ToString() + Path.GetExtension(PhotoPath.FileName);

                    // Set the file path
                    string filePath = Path.Combine(Server.MapPath("~/Uploads/Candidate/"), logoname);

                    // Save the file
                    PhotoPath.SaveAs(filePath);

                    // Set the candidate's PhotoPath
                    candidate.PhotoPath = "/Uploads/Candidate/" + logoname;
                }
                else
                {
                    ModelState.AddModelError("PhotoPath", "Invalid or non-image file uploaded.");
                    return View(candidate);
                }

                candidates.Add(candidate);
                SaveToJson(candidates, FilePath);
                return RedirectToAction("Index");
            }

            return View(candidate);
        }

        private bool IsImage(string contentType)
        {
            return contentType.StartsWith("image/");
        }


        // GET: Candidate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = candidates.Find(c => c.CandidateId == id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // POST: Candidate/Edit/5
        // Aşırı gönderim saldırılarından korunmak için bağlamak istediğiniz belirli özellikleri etkinleştirin. 
        // Daha fazla bilgi için bkz. https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "CandidateId,Name,TC,City,MobileNo,Email,PhotoPath")] Candidate candidate, HttpPostedFileBase PhotoPath)
        {
            if (ModelState.IsValid)
            {
                var k = candidates.Where(x => x.CandidateId == candidate.CandidateId).SingleOrDefault();

                if (PhotoPath != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(k.PhotoPath)))
                    {
                        System.IO.File.Delete(Server.MapPath(k.PhotoPath));
                    }
                    WebImage img = new WebImage(PhotoPath.InputStream);
                    FileInfo imginfo = new FileInfo(PhotoPath.FileName);

                    string logoname = PhotoPath.FileName + imginfo.Extension;
                    img.Resize(300, 200);
                    img.Save("~/Uploads/Candidate/" + logoname);

                    k.PhotoPath = "/Uploads/Candidate/" + logoname;
                }

                k.Name = candidate.Name;
                k.MobileNo = candidate.MobileNo;
                k.TC = candidate.TC;
                k.City = candidate.City;
                k.Email = candidate.Email;

                SaveToJson(candidates, FilePath);
                return RedirectToAction("Index");
            }
            return View(candidate);
        }

        // GET: Candidate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = candidates.Find(c => c.CandidateId == id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // POST: Candidate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Candidate candidate = candidates.Find(c => c.CandidateId == id);
            if (candidate != null)
            {
                candidates.Remove(candidate);
                SaveToJson(candidates,FilePath);
            }
            return RedirectToAction("Index");
        }

       /* protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
