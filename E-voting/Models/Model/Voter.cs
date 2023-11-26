using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_voting.Models.Model
{
    public class Voter
    {
        public int VoterId { get; set; }

        public string Name { get; set; }

        public int TC { get; set; }

        public string MobileNo { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string City { get; set; }

        public ICollection<VoteCastingInfo> VoteCastingInfos { get; set; }

        public string Authority { get; set; }
    }
}