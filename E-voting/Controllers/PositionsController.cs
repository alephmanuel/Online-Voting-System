using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using E_voting.Models.Model;
using System.Text.Json;

namespace E_voting.Controllers
{
    public class PositionsController : Controller
    {
        private  string PositionsFilePath;
        private List<Position> positions;

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

        // GET: Positions
        public ActionResult Index()
        {
            return View(positions);
        }

        // GET: Positions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = positions.Find(p => p.PositionId == id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // GET: Positions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Positions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PositionId,PositionName")] Position position)
        {
            if (ModelState.IsValid)
            {
                positions.Add(position);
                SaveToJson(positions, PositionsFilePath);
                return RedirectToAction("Index");
            }

            return View(position);
        }

        // GET: Positions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = positions.Find(p => p.PositionId == id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PositionId,PositionName")] Position position)
        {
            if (ModelState.IsValid)
            {
                var existingPosition = positions.Find(p => p.PositionId == position.PositionId);
                if (existingPosition != null)
                {
                    existingPosition.PositionName = position.PositionName;
                    SaveToJson(positions, PositionsFilePath);
                }

                return RedirectToAction("Index");
            }
            return View(position);
        }

        // GET: Positions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Position position = positions.Find(p => p.PositionId==id);
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Position position = positions.Find(p => p.PositionId == id);
            if (position != null)
            {
                positions.Remove(position);
                SaveToJson(positions, PositionsFilePath);
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
