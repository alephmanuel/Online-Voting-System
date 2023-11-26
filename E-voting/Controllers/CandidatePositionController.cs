/*using E_voting.Models.DataContext;
*/using E_voting.Models.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_voting.Controllers
{
    public class CandidatePositionController : Controller
    {
        private const string CandidatePositionFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\candidate_positions.json";
        private List<CandidatePosition> candidatesPositions;

        private T LoadFromJson<T>(string filePath)
        {
            try
            {
                var json = System.IO.File.ReadAllText(Server.MapPath(filePath));
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (FileNotFoundException)
            {
                return default(T);
            }
        }

        private void SaveToJson<T>(T data, string filePath)
        {
            var json = JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText(Server.MapPath(filePath), json);
        }

        public CandidatePositionController()
        {
            candidatesPositions = LoadFromJson<List<CandidatePosition>>(CandidatePositionFilePath) ?? new List<CandidatePosition>();
        }

        // GET: CandidatePosition
        public ActionResult Index()
        {
            return View(candidatesPositions.OrderByDescending(x => x.CandidatePositionId).ToList());
        }

    }
}
