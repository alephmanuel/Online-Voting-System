using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_voting.Models.Model
{
    public class Result
    {
        public int VoteCastingId { get; set; }

        public string CandidateId { get; set; }

        public string VoterId { get; set; }
    }
}