using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_voting.Models.Model
{
    public class Position
    {
        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public ICollection<VoteCastingInfo> VoteCastingInfos { get; set; }

        public ICollection<CandidatePosition> CandidatePosition { get; set; }
    }
}