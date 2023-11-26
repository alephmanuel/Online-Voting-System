using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using E_voting.Models.Model;
using Newtonsoft.Json; // Ensure to include the Newtonsoft.Json NuGet package

namespace E_voting.Controllers
{
    public class VoteCastingInfoController : Controller
    {
        private const string VoteCastingInfoFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\vote_casting_info.json";
        private const string VotersFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\voters.json";
        private List<VoteCastingInfo> voteCastingInfoList;
        private List<Voter> voters;

        public VoteCastingInfoController()
        {
            voteCastingInfoList = LoadFromJson<List<VoteCastingInfo>>(VoteCastingInfoFilePath) ?? new List<VoteCastingInfo>();
            voters = LoadFromJson<List<Voter>>(VotersFilePath) ?? new List<Voter>();
        }

        // GET: VoteCastingInfo
        public ActionResult Index()
        {
            return View(voteCastingInfoList.OrderByDescending(x => x.VoteCastingId));
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
