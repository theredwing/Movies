using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    public class Detail
    {
        public int MovieId { get; set; }
        [Key]
        [Column(Order = 0)]
        public int DetailId { get; set; }
        public int CategoryID { get; set; }

        [Display(Name = "Description: ")]
        [StringLength(255, ErrorMessage = "Description cannot be longer than 255 characters")]
        public string Description { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual Category Category { get; set; }
    }
}