using System.Collections.Generic;

namespace PairingStar.Models
{
    public class UpdatePairModel
    {
        public UserModel PairOne { get; set; }
        public IEnumerable<UserModel> OtherUsers { get; set; }
    }
}