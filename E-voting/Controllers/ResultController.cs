using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using E_voting.Models.Model;
using Newtonsoft.Json; // Ensure to include the Newtonsoft.Json NuGet package

namespace E_voting.Controllers
{
    public class ResultController : Controller
    {
        private const string ResultsFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\results.json";
        private const string CandidatesFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\candidates.json";
        private List<Result> results;
        private List<Candidate> candidates;

        public ResultController()
        {
            results = LoadFromJson<List<Result>>(ResultsFilePath) ?? new List<Result>();
            candidates = LoadFromJson<List<Candidate>>(CandidatesFilePath) ?? new List<Candidate>();
        }

        // GET: Result
        public ActionResult Index()
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (var item in results)
            {
                string id = item.CandidateId.ToString();
                if (counts.ContainsKey(id))
                {
                    counts[id] = counts[id] + 1;
                }
                else
                {
                    counts.Add(id, 1);
                }
            }

            int tempmax = counts.Values.Max();
            string tempkey = counts.FirstOrDefault(x => x.Value == tempmax).Key;
            string tempname = candidates.FirstOrDefault(c => c.CandidateId.ToString() == tempkey)?.Name ?? "Unknown";

            ViewBag.winnerid = tempkey;
            ViewBag.number = tempname;
            ViewBag.winnercount = tempmax.ToString();
            return View(results);
        }

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
    }
}
