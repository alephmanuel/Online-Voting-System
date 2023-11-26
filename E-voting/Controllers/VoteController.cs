using E_voting.Models;
/*using E_voting.Models.DataContext;*/
using E_voting.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace E_voting.Controllers
{
    public class VoteController : Controller
    {
        private const string ResultsFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\results.json";
        private const string CandidatesFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\candidates.json";
        private const string VotersFilePath = "C:\\Users\\aleph\\Documents\\GitHub\\Online-Voting-System\\E-voting\\JSONFiles\\voters.json";
        private List<Result> results;
        private List<Candidate> candidates;
        private List<Voter> voters;

        public VoteController()
        {
            results = new List<Result>();
            candidates = new List<Candidate>();
            voters = new List<Voter>();
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

        private void SaveToJson<T>(T data, string filePath)
        {
            var json = JsonConvert.SerializeObject(data);
            System.IO.File.WriteAllText(Server.MapPath(filePath), json);
        }

       /* public static string Encryption(string strText)
        {
            using (var md5 = MD5.Create())
            {
                var hashed = md5.ComputeHash(Encoding.UTF8.GetBytes(strText));
                return BitConverter.ToString(hashed).Replace("-", "").ToLower();
            }
        }*/

        // GET: Vote
        [Route("")]
        [Route("Vote/Home")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult VoteNow()
        {
            return View(candidates.OrderByDescending(x => x.CandidateId));
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Voter voter)
        {
            var login = voters.Where(x => x.Email == voter.Email).SingleOrDefault();
            if(login.Email==voter.Email && login.Password== Crypto.Hash(voter.Password, "MD5"))
            {
                Session["voterid"] = login.VoterId;
                Session["eposta"] = login.Email;
                return RedirectToAction("Index", "Vote");
            }
            ViewBag.Uyari = "Wrong password or email.";
            return View(voter);

        }

        public static string Encryption(string strText)
        {
            var publicKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(publicKey.ToString());

                    var encryptedData = rsa.Encrypt(testData, true);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }


        public static string Decryption(string strText)
        {
            var privateKey = "<RSAKeyValue><Modulus>21wEnTU+mcD2w0Lfo1Gv4rtcSWsQJQTNa6gio05AOkV/Er9w3Y13Ddo5wGtjJ19402S71HUeN0vbKILLJdRSES5MHSdJPSVrOqdrll/vLXxDxWs/U0UT1c8u6k/Ogx9hTtZxYwoeYqdhDblof3E75d9n2F0Zvf6iTb4cI7j6fMs=</Modulus><Exponent>AQAB</Exponent><P>/aULPE6jd5IkwtWXmReyMUhmI/nfwfkQSyl7tsg2PKdpcxk4mpPZUdEQhHQLvE84w2DhTyYkPHCtq/mMKE3MHw==</P><Q>3WV46X9Arg2l9cxb67KVlNVXyCqc/w+LWt/tbhLJvV2xCF/0rWKPsBJ9MC6cquaqNPxWWEav8RAVbmmGrJt51Q==</Q><DP>8TuZFgBMpBoQcGUoS2goB4st6aVq1FcG0hVgHhUI0GMAfYFNPmbDV3cY2IBt8Oj/uYJYhyhlaj5YTqmGTYbATQ==</DP><DQ>FIoVbZQgrAUYIHWVEYi/187zFd7eMct/Yi7kGBImJStMATrluDAspGkStCWe4zwDDmdam1XzfKnBUzz3AYxrAQ==</DQ><InverseQ>QPU3Tmt8nznSgYZ+5jUo9E0SfjiTu435ihANiHqqjasaUNvOHKumqzuBZ8NRtkUhS6dsOEb8A2ODvy7KswUxyA==</InverseQ><D>cgoRoAUpSVfHMdYXW9nA3dfX75dIamZnwPtFHq80ttagbIe4ToYYCcyUz5NElhiNQSESgS5uCgNWqWXt5PnPu4XmCXx6utco1UVH8HGLahzbAnSy6Cj3iUIQ7Gj+9gQ7PkC434HTtHazmxVgIR5l56ZjoQ8yGNCPZnsdYEmhJWk=</D></RSAKeyValue>";

            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    var base64Encrypted = strText;

                    // server decrypting data with private key                    
                    rsa.FromXmlString(privateKey);

                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }


        public JsonResult ToVote( string candidate, string voter)
        {
            //if (candidate)
            //{
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            results.Add(new Result { CandidateId = candidate, VoterId = Encryption(voter) });
            SaveToJson(results, ResultsFilePath);

            return Json(false, JsonRequestBehavior.AllowGet);
        }
    }
  
}