using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    [Table("CrewTitle")]
    public class CrewTitle
    {
        [Key]
        [Column(Order = 0)]
        public int CrewTitleID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Title: ")]
        public string CrewTitleName { get; set; }
    }
}