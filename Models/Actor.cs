using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Movies.Models
{
    [Table("Actor")]
    public partial class Actor
    {
        [Key]
        [Column(Order = 0)]
        public int ActorId { get; set; }

        [StringLength(50, ErrorMessage = "Actor cannot be longer than 50 characters")]
        [Required]
        [Display(Name = "Actor: ")]
        public string ActorName { get; set; }
        public int MovieId { get; set; }

        //public virtual Movie Movie { get; set; }
    }
}