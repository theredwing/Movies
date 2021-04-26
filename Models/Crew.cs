using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Movies.Models
{
    public class CrewTitle
    {
        public int CrewTitleID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Title: ")]
        public string CrewTitleName { get; set; }
    }
}