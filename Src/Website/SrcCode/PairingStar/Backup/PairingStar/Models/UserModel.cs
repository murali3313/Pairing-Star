using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PairingStar.Models
{
    public class UserModel
    {
        [Required]
        [Display(Name = "Name")]
        public string UserName { get; set; }
        [Display(Name = "Role")]
        public string Role { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name="User photo")]
        public Byte[] Photo { get; set; }

        public int ID { get; set; }

    }
    public enum Role
    {
        Dev,
        QA,
        PM,
        BA,
        UX,
        Misc
    }
}