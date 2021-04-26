using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movies.Models
{
    public class MovieCategory
    {
        [Key]
        [Column(Order = 0)]
        public int MovieCategoryId { get; set; }
        public int MovieId { get; set; }
        public int CategoryId { get; set; }
    }
}