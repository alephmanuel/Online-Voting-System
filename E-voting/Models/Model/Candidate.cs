using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_voting.Models.Model
{
    public class Candidate
    {
        public int CandidateId { get; set; }
        
        public string Name { get; set; }

        public string TC { get; set; }

        public string City { get; set; }

        public string MobileNo { get; set; }

        public string Email { get; set; }

        public string PhotoPath { get; set; }

        public ICollection<VoteCastingInfo> VoteCastingInfos { get; set; }

        public ICollection<CandidatePosition> CandidatePosition { get; set; }
    }
}